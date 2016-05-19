using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Mobile_Security_System.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Mobile_Security_System
{
    public partial class MainPage : PhoneApplicationPage
    {
        private PhotoCamera camera_;
        private Size cameraRes_;
        private bool cameraAvailable_;
        private bool mailAvailable_;
        private Image firstImageToCompare_;
        private readonly DispatcherTimer cameraTimer_, flashTimer_, mailTimer_;

        // Hour thresholds for deciding if we should activate the flash or not
        private const int NightHourStarting = 18;
        private const int NightHourEnding = 6;

        // If the number of different pixels of two images is greater than this, 
        // they are not equal
        private const int NumOfDifferentPixels = 10000;

        public MainPage()
        {
            InitializeComponent();

            cameraAvailable_ = false;
            mailAvailable_ = true;

            // Start the periodic event for motion detection
            cameraTimer_ = new DispatcherTimer();
            cameraTimer_.Interval = TimeSpan.FromMilliseconds(500);
            cameraTimer_.Tick += (o, arg) => ScanPreviewBuffer();
            cameraTimer_.Start();

            // Start the periodic event for deciding if we keep the flash on or off
            flashTimer_ = new DispatcherTimer();
            flashTimer_.Interval = TimeSpan.FromHours(1);
            flashTimer_.Tick += (o, arg) => UpdateFlashState();
            flashTimer_.Start();

            // Run once a minute, make the phone available for sending mail
            mailTimer_ = new DispatcherTimer();
            mailTimer_.Interval = TimeSpan.FromMinutes(1);
            mailTimer_.Tick += (o, arg) => UpdateMailState();
            mailTimer_.Start();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            camera_ = new PhotoCamera(CameraType.Primary);
            videoBrush.SetSource(camera_);

            // Event is fired when the PhotoCamera object has been initialized.
            camera_.Initialized += 
                new EventHandler<CameraOperationCompletedEventArgs>(cam_Initialized);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            camera_.Dispose();
            camera_.Initialized -= cam_Initialized;
        }

        private void cam_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
            // Turn off flash
            if (camera_.IsFlashModeSupported(FlashMode.On))
            {
                camera_.FlashMode = FlashMode.Off;
            }

            // Setting the resolution of the camera
            IEnumerator<Size> resList = camera_.AvailableResolutions.GetEnumerator();
            resList.MoveNext();
            cameraRes_ = camera_.Resolution = resList.Current;

            cameraAvailable_ = true;
            UpdateFlashState();
        }

        public static void FromByteArray(WriteableBitmap bmp, int[] buffer)
        {
            Buffer.BlockCopy(buffer, 0, bmp.Pixels, 0, buffer.Length);
        }

        private async void ScanPreviewBuffer()
        {
            if (cameraAvailable_)
            {
                int[] ARGBPx = new int[(int)cameraRes_.Width * (int)cameraRes_.Height];
                MailModel mail = new MailModel();

                // Get the current frame
                camera_.Focus();
                camera_.GetPreviewBufferArgb32(ARGBPx);

                if (firstImageToCompare_ == null)
                {
                    firstImageToCompare_ = 
                        new Image(ARGBPx, (int)cameraRes_.Width, (int)cameraRes_.Height);
                }
                // Do the comparison and detect the motion
                else
                {
                    Image newImage = 
                        new Image(ARGBPx, (int)cameraRes_.Width, (int)cameraRes_.Height);
                    int[] r1 = firstImageToCompare_.GetR();
                    int[] r2 = newImage.GetR();

                    // Update the screen if any motion has been detected
                    if (DetectMotion(firstImageToCompare_, newImage))
                    {
                        textBlock.Visibility = Visibility.Visible;

                        // Encode the preview buffer as string
                        using (MemoryStream ms = new MemoryStream())
                        {
                            WriteableBitmap wbmp = new WriteableBitmap(
                                (int)cameraRes_.Width, (int)cameraRes_.Height);
                            ARGBPx.CopyTo(wbmp.Pixels, 0);
                            wbmp.SaveJpeg(ms, 
                                (int)cameraRes_.Width, (int)cameraRes_.Height, 0, 50);
                       
                            mail.Image = Convert.ToBase64String(ms.ToArray());
                            mail.Subject = "MSS";
                            mail.Body = "Intruder detected!";
                            mail.To = App.RoamingSettings.Values["Email"].ToString();
                        }

                        // Send e-mail
                        if (App.RoamingSettings.Values["Emailactivate"].Equals(true) && mailAvailable_)
                        {
                            mailAvailable_ = false;

                            if (!await Controllers.MailController.SendMail(mail))
                            {
                                 MessageBox.Show("Email could not be sent!");
                            }
                        }
                    }
                    else
                    {
                        textBlock.Visibility = Visibility.Collapsed;
                    }

                    // Update the image
                    firstImageToCompare_ = newImage;
                }
            }
        }

        // Turn on or off the flash by hour of the day 
        private void UpdateFlashState()
        {
            if (cameraAvailable_ && camera_.IsFlashModeSupported(FlashMode.On))
            {
                if (DateTime.Now.Hour > NightHourStarting || 
                    DateTime.Now.Hour < NightHourEnding)
                {
                    camera_.FlashMode = FlashMode.On;
                }
                else
                {
                    camera_.FlashMode = FlashMode.Off;
                }
            }
        }

        // Run once a minute, make the phone available for sending mail
        private void UpdateMailState()
        {
            mailAvailable_ = true;
        }

        // Returns true if there is a difference between rhs and lhs
        private static bool DetectMotion(Image rhs, Image lhs)
        {
            int[] rValuesRhs = rhs.GetR();
            int[] rValuesLhs = lhs.GetR();
            int difference = 0;

            for (int i = 0; i < rValuesRhs.Length; ++i)
            {
                if (rValuesRhs[i] - rValuesLhs[i] > 50)
                {
                    ++difference;
                }
            }

            return difference > NumOfDifferentPixels;
        }

        private void SignoutClicked(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show
                ("Are you sure you want to sign out?", "Sign out", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                App.RoamingSettings.Values["Loggedin"] = false;
                App.Current.Terminate();
            }
        }

        private async void DeleteMembershipClicked(object sender, EventArgs e)
        {
            MessageBoxResult result = MessageBox.Show
                ("Are you sure you want to delete your membership?", 
                "Delete membership", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                await Controllers.UserController.DeleteUser
                    (App.RoamingSettings.Values["Email"].ToString());
                App.RoamingSettings.Values["Loggedin"] = false;
                App.Current.Terminate();
            }
        }

        private void MailEnableDisableClick(object sender, EventArgs e)
        {
            if (App.RoamingSettings.Values["Emailactivate"] == null || 
                App.RoamingSettings.Values["Emailactivate"].Equals(false))
            {
                App.RoamingSettings.Values["Emailactivate"] = true;
                MessageBox.Show("Email notifications enabled!");
            }
            else
            {
                App.RoamingSettings.Values["Emailactivate"] = false;
                MessageBox.Show("Email notifications disabled!");
            }
        }

        // If flash is activated, then deactivate it (or vice versa)
        private void FlashlightClicked(object sender, EventArgs e)
        {
            if (cameraAvailable_ && camera_.IsFlashModeSupported(FlashMode.On))
            {
                if (camera_.FlashMode == FlashMode.On)
                {
                    camera_.FlashMode = FlashMode.Off;
                }
                else if (camera_.FlashMode == FlashMode.Off)
                {
                    camera_.FlashMode = FlashMode.On;
                }
            }
        }
    }
}
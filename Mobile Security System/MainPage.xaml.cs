using Microsoft.Devices;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Mobile_Security_System
{
    public partial class MainPage : PhoneApplicationPage
    {
        private PhotoCamera camera_;
        private Size cameraRes_;
        private bool cameraInitialized_;
        private Image firstImageToCompare_;
        private readonly DispatcherTimer cameraTimer_, flashTimer_;

        // Hour thresholds for deciding if we should activate the flash or not
        private const int NightHourStarting = 18;
        private const int NightHourEnding = 6;

        // If the number of different pixels of two images is greater than this, 
        // they are not equal
        private const int NumOfDifferentPixels = 10000;

        public MainPage()
        {
            InitializeComponent();

            cameraInitialized_ = false;

            // Start the periodic event for motion detection
            cameraTimer_ = new DispatcherTimer();
            cameraTimer_.Interval = TimeSpan.FromMilliseconds(250);
            cameraTimer_.Tick += (o, arg) => ScanPreviewBuffer();
            cameraTimer_.Start();

            // Start the periodic event for deciding if we keep the flash on or off
            flashTimer_ = new DispatcherTimer();
            flashTimer_.Interval = TimeSpan.FromHours(1);
            flashTimer_.Tick += (o, arg) => UpdateFlashState();
            flashTimer_.Start();
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

            cameraInitialized_ = true;
            UpdateFlashState();
        }

        private void ScanPreviewBuffer()
        {
            if (cameraInitialized_)
            {
                int[] ARGBPx = new int[(int)cameraRes_.Width * (int)cameraRes_.Height];

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
            if (cameraInitialized_ && camera_.IsFlashModeSupported(FlashMode.On))
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

        // If flash is activated, then deactivate it (or vice versa)
        private void FlashlightClicked(object sender, EventArgs e)
        {
            if (cameraInitialized_ && camera_.IsFlashModeSupported(FlashMode.On))
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
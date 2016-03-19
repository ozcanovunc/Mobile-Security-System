using Microsoft.Devices;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;
using System.Windows;
using System.Windows.Navigation;

namespace Mobile_Security_System
{
    public partial class MainPage : PhoneApplicationPage
    {
        private PhotoCamera camera_;
        private MediaLibrary library_ = new MediaLibrary(); // TODO: Remove
        private bool cameraAvailable_;

        public MainPage()
        {
            InitializeComponent();
            cameraAvailable_ = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            camera_ = new PhotoCamera(CameraType.Primary);
            videoBrush.SetSource(camera_);

            // Event is fired when the PhotoCamera object has been initialized.
            camera_.Initialized += new EventHandler<CameraOperationCompletedEventArgs>(cam_Initialized);
            // Event is fired when the capture sequence is complete.
            camera_.CaptureCompleted += new EventHandler<CameraOperationCompletedEventArgs>(cam_CaptureCompleted);
            // Event is fired when the capture sequence is complete and an image is available.
            camera_.CaptureImageAvailable += new EventHandler<ContentReadyEventArgs>(cam_CaptureImageAvailable);
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            camera_.Dispose();
            camera_.Initialized -= cam_Initialized;
            camera_.CaptureCompleted -= cam_CaptureCompleted;
            camera_.CaptureImageAvailable -= cam_CaptureImageAvailable;
        }

        private void cam_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
            // Turn off flash
            if (camera_.IsFlashModeSupported(FlashMode.On))
                camera_.FlashMode = FlashMode.Off;

            cameraAvailable_ = true;
        }

        private void cam_CaptureCompleted(object sender, CameraOperationCompletedEventArgs e)
        {
            cameraAvailable_ = true;
        }

        private void cam_CaptureImageAvailable(object sender, ContentReadyEventArgs e)
        {
            library_.SavePictureToCameraRoll("x.jpg", e.ImageStream); // TODO: Remove
            byte[] image = streamToByteArray(e.ImageStream);



            e.ImageStream.Close();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (cameraAvailable_)
            {
                camera_.CaptureImage();
                cameraAvailable_ = false;
            }
        }

        private static byte[] streamToByteArray(Stream stream)
        {
            byte[] buffer = new byte[4096]; // 4KB
            int read;

            stream.Seek(0, SeekOrigin.Begin);

            using (MemoryStream ms = new MemoryStream())
            {
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
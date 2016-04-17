using Microsoft.Devices;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;

namespace Mobile_Security_System
{
    public partial class MainPage : PhoneApplicationPage
    {
        private PhotoCamera camera_;
        private Size cameraRes_;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            camera_ = new PhotoCamera(CameraType.Primary);
            videoBrush.SetSource(camera_);

            // Event is fired when the PhotoCamera object has been initialized.
            camera_.Initialized += new EventHandler<CameraOperationCompletedEventArgs>(cam_Initialized);
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
                camera_.FlashMode = FlashMode.Off;

            // Setting the resolution of the camera
            IEnumerator<Size> resList = camera_.AvailableResolutions.GetEnumerator();
            resList.MoveNext();
            cameraRes_ = camera_.Resolution = resList.Current;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            int[] ARGBPx = new int[(int)cameraRes_.Width * (int)cameraRes_.Height];
            camera_.GetPreviewBufferArgb32(ARGBPx);

            Image image = new Image(ARGBPx, (int)cameraRes_.Width, (int)cameraRes_.Height);
        }
    }
}
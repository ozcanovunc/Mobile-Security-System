namespace Mobile_Security_System
{
    public class Image
    {
        private int[] a_;
        private int[] r_;
        private int[] g_;
        private int[] b_;

        private int width_;
        private int height_;

        public Image(int[] image, int width, int height)
        {
            width_ = width;
            height_ = height;

            a_ = new int[image.Length];
            r_ = new int[image.Length];
            g_ = new int[image.Length];
            b_ = new int[image.Length];

            // Initialize A, R, G and B values for each pixel
            for (int pi = 0; pi < image.Length; ++pi)
            {
                GetARGB(image[pi], out a_[pi], out r_[pi], out g_[pi], out b_[pi]);
            }
        }

        public int[] GetA()
        {
            return a_;
        }

        public int[] GetR()
        {
            return r_;
        }

        public int[] GetG()
        {
            return g_;
        }

        public int[] GetB()
        {
            return b_;
        }

        private static void GetARGB(int color, out int a, out int r, out int g, 
            out int b)
        {
            a = color >> 24;
            r = (color & 0x00ff0000) >> 16;
            g = (color & 0x0000ff00) >> 8;
            b = (color & 0x000000ff);
        }

        private static int GetColorFromArgb(int a, int r, int g, int b)
        {
            int result = ((a & 0xFF) << 24) | ((r & 0xFF) << 16) | 
                ((g & 0xFF) << 8) | (b & 0xFF);

            return result;
        }
    }
}
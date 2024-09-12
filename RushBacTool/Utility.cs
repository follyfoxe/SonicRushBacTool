using RushBacLib;

namespace RushBacTool
{
    public static class Utility
    {
        public static Bitmap ToBitmap(this ImageResult image)
        {
            Bitmap result = new(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bitmapData = result.LockBits(new(Point.Empty, result.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, result.PixelFormat);
            System.Runtime.InteropServices.Marshal.Copy(image.ToArgb(), 0, bitmapData.Scan0, bitmapData.Stride * bitmapData.Height);
            result.UnlockBits(bitmapData);
            return result;
        }
    }
}

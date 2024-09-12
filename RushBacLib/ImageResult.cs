using System.Buffers.Binary;
using System.Drawing;

namespace RushBacLib
{
    public readonly struct ImageResult(int width, int height)
    {
        public readonly int Width = width;
        public readonly int Height = height;
        public readonly Color[] Pixels = new Color[width * height];

        public ref Color this[int i] => ref Pixels[i];
        public ref Color this[int x, int y] => ref Pixels[x + y * Width];

        public void DrawImage(ImageResult image, int x, int y)
        {
            int endX = Math.Min(Width, x + image.Width);
            int endY = Math.Min(Height, y + image.Height);
            for (int py = Math.Max(y, 0); py < endY; py++)
            {
                for (int px = Math.Max(x, 0); px < endX; px++)
                    this[px, py] = image[px - x, py - y];
            }
        }

        public byte[] ToArgb()
        {
            byte[] result = new byte[Width * Height * 4];
            Span<byte> span = result;
            for (int i = 0; i < Pixels.Length; i++)
                BinaryPrimitives.WriteInt32LittleEndian(span.Slice(i * 4, 4), Pixels[i].ToArgb());
            return result;
        }
    }
}
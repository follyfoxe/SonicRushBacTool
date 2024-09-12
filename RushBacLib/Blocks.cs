using System.Diagnostics;
using System.Drawing;

namespace RushBacLib
{
    // The BAC format is little endian.
    // This code doesn't read the file in it's entirety, and isn't fully accurate.
    // More details here: https://www.romhacking.net/documents/669/
    // And some code was taken from here: https://github.com/NotKit/sonic-rush-tools/blob/master/bac.py

    public class Header
    {
        public static readonly uint BacMagic = BitConverter.ToUInt32(BacFile.Encoding.GetBytes("BAC\x0A"));

        public string MagicString => BacFile.Encoding.GetString(BitConverter.GetBytes(MagicNumber));
        public readonly uint MagicNumber;

        // Absolute offsets, relative to the beggining of the file.
        public readonly uint AnimationMappings;
        public readonly uint AnimationFrames;
        public readonly uint FrameAssembly;
        public readonly uint Palettes;
        public readonly uint ImageData;
        public readonly uint AnimationInfo;

        public Header(BinaryReader reader)
        {
            MagicNumber = reader.ReadUInt32();
            Trace.WriteLineIf(MagicNumber != BacMagic, "Stream is not a BAC file, magic does not match.");

            AnimationMappings = reader.ReadUInt32();
            AnimationFrames = reader.ReadUInt32();
            FrameAssembly = reader.ReadUInt32();
            Palettes = reader.ReadUInt32();
            ImageData = reader.ReadUInt32();
            AnimationInfo = reader.ReadUInt32();
        }
    }

    public class AnimationInfo // Block 1
    {
        public readonly uint BlockSize;
        public readonly ushort EntryCount;
        public readonly ushort EntrySize;
        public readonly byte[] Unknown;
        public readonly AnimationInfoEntry[] Entries;

        public AnimationInfo(BinaryReader reader)
        {
            BlockSize = reader.ReadUInt32();
            EntryCount = reader.ReadUInt16();
            EntrySize = reader.ReadUInt16();
            Unknown = reader.ReadBytes(20);
            Entries = new AnimationInfoEntry[EntryCount];
            for (int i = 0; i < EntryCount; i++)
                Entries[i] = new AnimationInfoEntry(reader);
        }
    }

    public class AnimationInfoEntry(BinaryReader reader)
    {
        public readonly byte[] Unknown = reader.ReadBytes(20);
    }

    public class AnimationMappings // Block 2, acts as an animation table of some sorts.
    {
        public readonly uint BlockSize;
        public readonly AnimationMapping[] Mappings;

        public AnimationMappings(BinaryReader reader)
        {
            BlockSize = reader.ReadUInt32();
            Mappings = new AnimationMapping[(BlockSize - 4) / 8];
            for (int i = 0; i < Mappings.Length; i++)
                Mappings[i] = new AnimationMapping(reader);
        }
    }

    public class AnimationMapping(BinaryReader reader)
    {
        public readonly uint FrameOffset = reader.ReadUInt32(); // Offset to Block 3's data, relative to Header.AnimationFrames.
        public readonly uint Unknown = reader.ReadUInt32();
    }

    public class AnimationFrames
    {
        public readonly uint RestingFrame;
        public readonly List<AnimationFrame> Frames;

        public AnimationFrames(BacFile file, BinaryReader reader)
        {
            Frames = [];

            AnimationFrame frame = null;
            while (reader.BaseStream.Position < file.Header.FrameAssembly)
            {
                // ID 1 indicates a new frame and ID 4 acts as a terminator for the AnimationFrame.
                ushort blockId = reader.ReadUInt16();
                ushort blockSize = reader.ReadUInt16();

                if (blockId == 4)
                {
                    frame?.Build(file, reader);
                    RestingFrame = reader.ReadUInt32();
                    break;
                }

                switch (blockId)
                {
                    case 0: // Animation Info, not implemented
                        reader.BaseStream.Position += blockSize - 4;
                        break;
                    case 1: // Frame Assembly, usually the first frame block.
                        frame?.Build(file, reader);
                        frame = new AnimationFrame();
                        Frames.Add(frame);

                        int assemblyPartCount = (blockSize - 4) / 4;
                        frame.FrameOffsets = new AnimationFrame.FrameOffset[assemblyPartCount];
                        for (int i = 0; i < assemblyPartCount; i++)
                            frame.FrameOffsets[i] = new AnimationFrame.FrameOffset(reader);
                        break;
                    case 2: // Image Parts
                        int imagePartCount = (blockSize - 4) / 8;
                        if (frame.ImageParts != null)
                            Trace.WriteLine("Interesting...");
                        frame.ImageParts = new AnimationFrame.ImagePart[imagePartCount];
                        for (int i = 0; i < imagePartCount; i++)
                            frame.ImageParts[i] = new AnimationFrame.ImagePart(reader);
                        break;
                    case 3: // Palette Parts
                        int palettePartCount = (blockSize - 4) / 8;
                        if (frame.PaletteParts != null)
                            Trace.WriteLine("Interesting...");
                        frame.PaletteParts = new AnimationFrame.PalettePart[palettePartCount];
                        for (int i = 0; i < palettePartCount; i++)
                            frame.PaletteParts[i] = new AnimationFrame.PalettePart(reader);
                        break;
                    default:
                        Trace.WriteLine($"Unhandled Frame Block {blockId}.");
                        reader.BaseStream.Position += blockSize - 4; // The 4 accounts for the blockId and blockSize.
                        break;
                }
            }
        }
    }

    public class AnimationFrame // Custom helper thing, not in the File Specs
    {
        public FrameOffset[] FrameOffsets; // Offsets into FrameAssembly
        public ImagePart[] ImageParts;
        public PalettePart[] PaletteParts;

        public FrameAssembly FrameAssembly;
        public ImageData[] Images;
        public Palette Palette;

        // TODO: Refactor this...
        public void Build(BacFile file, BinaryReader reader)
        {
            long last = reader.BaseStream.Position;
            reader.BaseStream.Position = file.Header.FrameAssembly + FrameOffsets[0].DataOffset;
            FrameAssembly = new FrameAssembly(reader);

            if (ImageParts != null)
            {
                Images = new ImageData[ImageParts.Length];
                for (int i = 0; i < Images.Length; i++)
                {
                    reader.BaseStream.Position = file.Header.ImageData + ImageParts[i].DataOffset;
                    Images[i] = new ImageData(reader);
                }
            }
            else
                Trace.WriteLine("Warning: Frame doesn't have any Image Parts!");

            if (PaletteParts != null)
            {
                reader.BaseStream.Position = file.Header.Palettes + PaletteParts[0].DataOffset;
                Palette = new Palette(reader);
            }
            else
                Trace.WriteLine("Warning: Frame doesn't have any Palettes!");

            reader.BaseStream.Position = last;
        }

        public ImageResult GetImage(bool transparency = true)
        {
            // Perhaps Left = HotSpotX + FrameX
            // And Right = HotSpot + FrameXRight

            int width = FrameAssembly.FrameXRight - FrameAssembly.FrameX;
            int height = FrameAssembly.FrameYBottom - FrameAssembly.FrameY;
            //Trace.WriteLine($"FrameX: {FrameAssembly.FrameX}, FrameY: {FrameAssembly.FrameY}, FrameXRight: {FrameAssembly.FrameXRight}, FrameYBottom: {FrameAssembly.FrameYBottom}");
            //int width = 128;
            //int height = 128;

            ImageResult image = new(width, height);
            if (Images == null)
                return image;

            Color[] pal = Palette.GetColors(transparency);
            for (int i = 0; i < Images.Length; i++)
                FrameAssembly.DrawImage(image, pal, Images[i], i);
            return image;
        }

        public readonly struct FrameOffset(BinaryReader reader)
        {
            public readonly uint DataOffset = reader.ReadUInt32();
        }

        public readonly struct ImagePart(BinaryReader reader)
        {
            public readonly uint DataOffset = reader.ReadUInt32();
            public readonly uint DataSize = reader.ReadUInt32();
        }

        public readonly struct PalettePart(BinaryReader reader)
        {
            public readonly uint DataOffset = reader.ReadUInt32();
            public readonly uint ColorCount = reader.ReadUInt32();
        }
    }

    public class FrameAssembly
    {
        // Attributes from sprite.h of libnds
        // Attribute 0 consists of 8 bits of Y plus the following flags:
        public const int Attr0Square = 0 << 14;
        public const int Attr0Wide = 1 << 14;
        public const int Attr0Tall = 2 << 14;

        // Atribute 1 consists of 9 bits of X plus the following flags:
        public const int Attr1FlipX = 1 << 12;
        public const int Attr1FlipY = 1 << 13;
        public const int Attr1Size8 = 0 << 14;
        public const int Attr1Size16 = 1 << 14;
        public const int Attr1Size32 = 2 << 14;
        public const int Attr1Size64 = 3 << 14;

        public static readonly Size[] SizeSquare = [new(8, 8), new(16, 16), new(32, 32), new(64, 64)];
        public static readonly Size[] SizeWide = [new(16, 8), new(32, 8), new(32, 16), new(64, 32)];
        public static readonly Size[] SizeTall = [new(8, 16), new(8, 32), new(16, 32), new(32, 64)];

        public readonly uint FramePartCount;
        public readonly short FrameX, FrameY, FrameXRight, FrameYBottom, HotSpotX, HotSpotY;

        public ImagePartInfo[] PartInfos;

        public FrameAssembly(BinaryReader reader)
        {
            FramePartCount = reader.ReadUInt32();
            FrameX = reader.ReadInt16();
            FrameY = reader.ReadInt16();
            FrameXRight = reader.ReadInt16();
            FrameYBottom = reader.ReadInt16();
            HotSpotX = reader.ReadInt16();
            HotSpotY = reader.ReadInt16();

            PartInfos = new ImagePartInfo[FramePartCount];
            for (int i = 0; i < FramePartCount; i++)
                PartInfos[i] = new ImagePartInfo(reader);
        }

        public void DrawImage(ImageResult output, Color[] palette, ImageData image, int partIndex)
        {
            ImagePartInfo info = PartInfos[partIndex];

            int partY = info.Attr0 & 0xFF;
            int partX = info.Attr1 & 0x1FF;
            int size = info.Attr1 >> 14;

            Size partSize;
            if ((info.Attr0 & Attr0Tall) != 0) // Sprite shape is NxM with N < M (Height > Width)
                partSize = SizeTall[size];
            else if ((info.Attr0 & Attr0Wide) != 0) // Sprite shape is NxM with N > M (Height < Width)
                partSize = SizeWide[size];
            else // Sprite shape is NxN (Height == Width)
                partSize = SizeSquare[size];

            ImageResult partImage = image.GetImage(palette, partSize.Width, partSize.Height);
            output.DrawImage(partImage, partX, partY);
        }

        public readonly struct ImagePartInfo(BinaryReader reader)
        {
            public readonly ushort Attr0 = reader.ReadUInt16();
            public readonly ushort Attr1 = reader.ReadUInt16();
            public readonly ushort Attr2 = reader.ReadUInt16();
            public readonly ushort Attr3 = reader.ReadUInt16();
        }
    }

    public static class BlockUtility
    {
        public static byte[] ReadCompressed(BinaryReader reader)
        {
            // 1 byte: Compression? (0x00 = Uncompressed, 0x10 = LZSS)
            // 3 bytes: Size of data region
            uint header = reader.ReadUInt32();
            uint compression = header & 0xFF;
            uint uncompressedSize = header >> 8;

            if (compression == 0)
                return reader.ReadBytes((int)uncompressedSize);
            else if (compression == 0x10) // Compressed
                throw new Exception("Compression is not supported!");
            else
                throw new Exception("Invalid compression type: " + compression);
        }
    }

    public class Palette(BinaryReader reader) // 4bpp rgb
    {
        public readonly byte[] Data = BlockUtility.ReadCompressed(reader);

        public Color[] GetColors(bool transparency = true)
        {
            Color[] colors = new Color[Data.Length / 2];
            for (int i = 0; i < Data.Length; i += 2)
            {
                int color = BitConverter.ToUInt16(Data, i);
                int r = color & 31;
                int g = (color & 31 << 5) >> 5;
                int b = (color & 31 << 10) >> 10;
                r = (int)(r * (255 / 31.0f));
                g = (int)(g * (255 / 31.0f));
                b = (int)(b * (255 / 31.0f));
                colors[i / 2] = Color.FromArgb(r, g, b);
            }
            if (transparency)
                colors[0] = Color.FromArgb(0, colors[0]); // Index 0 is transparent. Maybe use an index instead of a bool.
            return colors;
        }
    }

    public class ImageData(BinaryReader reader) // 4bpp
    {
        public readonly byte[] Data = BlockUtility.ReadCompressed(reader);

        public ImageResult GetImage(Color[] palette, int width, int height)
        {
            ImageResult result = new(width, height);
            List<ImageResult> tiles = [];

            const int bytesPerTile = 32;
            const int tileSize = 8;

            for (int tile = 0; tile < Data.Length; tile += bytesPerTile)
            {
                byte[] pixels = new byte[bytesPerTile * 2];
                for (int i = 0; i < pixels.Length; i += 2)
                {
                    byte b = Data[tile + i / 2];
                    pixels[i] = (byte)(b & 0x0F);
                    pixels[i + 1] = (byte)((b & 0xF0) >> 4);
                }

                ImageResult tileImage = new(tileSize, tileSize);
                for (int i = 0; i < pixels.Length; i++)
                    tileImage[i] = palette[pixels[i]];
                tiles.Add(tileImage);
            }

            int tilesPerRow = width / tileSize;
            for (int i = 0; i < tiles.Count; i++)
                result.DrawImage(tiles[i], (i % tilesPerRow) * tileSize, (i / tilesPerRow) * tileSize);

            tiles.Clear();
            return result;
        }
    }
}
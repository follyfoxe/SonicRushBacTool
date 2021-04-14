using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Ekona;
using Ekona.Images;

namespace BacConvert
{
    class Program
    {
        const int attr_0_square = (0 << 14);
        const int attr_0_wide = (1 << 14);
        const int attr_0_tall = (2 << 14);

        static readonly Size[] size_square = { new Size(8, 8), new Size(16, 16), new Size(32, 32), new Size(64, 64) };
        static readonly Size[] size_wide = { new Size(16, 8), new Size(32, 8), new Size(32, 16), new Size(64, 32) };
        static readonly Size[] size_tall = { new Size(8, 16), new Size(8, 32), new Size(16, 32), new Size(32, 64) };

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("no file");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("started");

            List<RawPalette> palettes = new List<RawPalette>();

            Stream stream = File.OpenRead(args[0]);

            using (BinaryReader reader = new BinaryReader(stream))
            {
                uint header = reader.ReadUInt32();
                uint block2 = reader.ReadUInt32();
                uint block3 = reader.ReadUInt32();
                uint block4 = reader.ReadUInt32();
                uint block5 = reader.ReadUInt32();
                uint block6 = reader.ReadUInt32();
                uint block1 = reader.ReadUInt32();

                stream.Position = block1;
                uint block_size = reader.ReadUInt32();
                ushort frames_count = reader.ReadUInt16();
                ushort frames_info_size = reader.ReadUInt16();

                stream.Position = block2;
                block_size = reader.ReadUInt32();
                if (block_size == frames_count * 8 + 4) Console.WriteLine("Block2: OK");
                uint[] anim_offs = new uint[frames_count];
                //Get subblock offsets (position relative to block2's position)
                for (int i = 0; i < frames_count; i++)
                {
                    anim_offs[i] = reader.ReadUInt32();
                    reader.ReadUInt32();
                }

                List<uint>[] palette_offs = new List<uint>[frames_count];
                List<uint>[] image_offs = new List<uint>[frames_count];
                uint[] frame_offs = new uint[frames_count]; //block4

                List<List<uint>>[] subimage_offs = new List<List<uint>>[frames_count];
                List<uint>[] subframe_offs = new List<uint>[frames_count];

                //Iterate through all block3's "subblocks 1 (Single Frame)"
                for (int frame_index = 0; frame_index < frames_count; frame_index++)
                {
                    uint frame_offset = anim_offs[frame_index];
                    stream.Position = block3 + frame_offset; //Set position to the offsets 

                    ushort block_id = reader.ReadUInt16(); //ID 01: Frame Offsets
                    block_size = reader.ReadUInt16(); //Length of sub sub blocks
                    uint subblock_count = (block_size - 4) / 4;

                    if (subblock_count != 0)
                        frame_offs[frame_index] = reader.ReadUInt32(); //Get image offsets into block4
                    Console.WriteLine("Frame Assembly SubBlockCount: " + subblock_count);

                    block_id = reader.ReadUInt16(); //ID 02: Image Parts
                    block_size = reader.ReadUInt16(); //Length of sub sub blocks
                    subblock_count = (block_size - 4) / 8; //stream.Position += subblock_count * 8;

                    for (int i = 0; i < subblock_count; i++)
                    {
                        if (image_offs[frame_index] == null) image_offs[frame_index] = new List<uint>();
                        image_offs[frame_index].Add(reader.ReadUInt32()); //Get image offsets into block5
                        reader.ReadUInt32();
                    }

                    block_id = reader.ReadUInt16(); //ID 03: Palletes
                    block_size = reader.ReadUInt16(); //Length of sub sub blocks
                    subblock_count = (block_size - 4) / 8;

                    for (int i = 0; i < subblock_count; i++)
                    {
                        if (palette_offs[frame_index] == null) palette_offs[frame_index] = new List<uint>();
                        palette_offs[frame_index].Add(reader.ReadUInt32()); //Get pallete offsets into block5
                        reader.ReadUInt32();
                    }

                    subimage_offs[frame_index] = new List<List<uint>>();
                    subframe_offs[frame_index] = new List<uint>();

                    while (stream.Position < block4) //keep reading until ID 04
                    {
                        block_id = reader.ReadUInt16();
                        block_size = reader.ReadUInt16();

                        if (block_id == 1)
                        {
                            uint subframe_off_count = (block_size - 4) / 4;
                            if (subframe_off_count != 0)
                                subframe_offs[frame_index].Add(reader.ReadUInt32());
                        }
                        else if (block_id == 2)
                        {
                            uint subimage_part_count = (block_size - 4) / 8;
                            List<uint> subimage = new List<uint>();
                            for (int i = 0; i < subimage_part_count; i++)
                            {
                                subimage.Add(reader.ReadUInt32());
                                reader.ReadUInt32();
                            }
                            subimage_offs[frame_index].Add(subimage);
                            Console.WriteLine("Found {0} Subframe parts in Frame {1}, Subframes are a work in progress", subimage_part_count, frame_index);
                        }
                        else if (block_id == 4)
                            break;
                        else
                            stream.Position += block_size - 4;
                    }
                }

                //Palletes
                for (int i = 0; i < palette_offs.Length; i++) //Iterate through each frame
                {
                    for (int j = 0; j < palette_offs[i].Count; j++) //Iterate through each frame palletes
                    {
                        stream.Position = block5 + palette_offs[i][j];

                        uint pal_length = reader.ReadUInt32();
                        uint unknown = pal_length & 0xFF;
                        pal_length = pal_length >> 8;
                        ColorFormat depth = pal_length < 0x200 ? ColorFormat.colors16 : ColorFormat.colors256;

                        if (pal_length > 0)
                        {
                            int num_colors = depth == ColorFormat.colors16 ? 0x10 : 0x100;
                            Color[][] colors = new Color[pal_length / (num_colors * 2)][];
                            for (int k = 0; k < colors.Length; k++)
                                colors[k] = Actions.BGR555ToColor(reader.ReadBytes(num_colors * 2));
                            palettes.Add(new RawPalette(colors, false, depth));
                        }
                    }
                }

                Console.WriteLine("Found {0} palettes", palettes.Count);

                //Tile data
                for (int i = 0; i < image_offs.Length; i++)
                {
                    stream.Position = block4 + frame_offs[i];
                    reader.ReadUInt32();
                    short xleft = reader.ReadInt16();
                    short ytop = reader.ReadInt16();
                    short xright = reader.ReadInt16();
                    short ybottom = reader.ReadInt16();

                    Bitmap res = new Bitmap(xright - xleft, ybottom - ytop);
                    Graphics g = Graphics.FromImage(res);

                    for (int j = 0; j < image_offs[i].Count; j++)
                    {
                        stream.Position = block6 + image_offs[i][j];

                        uint tile_length = reader.ReadUInt32();
                        uint compression = (uint)palettes[i].Original.Length & 0xFF;
                        tile_length = tile_length >> 8;
                        if (compression == 0x10)
                            Console.WriteLine("Compressed; not implemented");

                        byte[] tiles = reader.ReadBytes((int)tile_length);

                        stream.Position = block4 + frame_offs[i] + 16 + 8 * j;
                        ushort attr_0 = reader.ReadUInt16();
                        ushort attr_1 = reader.ReadUInt16();
                        GetSize(attr_0, attr_1, out int width, out int height);

                        RawImage image = new RawImage(tiles, TileForm.Horizontal, palettes[i].Depth, width, height, false);

                        int ypos = attr_0 & 0xFF;
                        int xpos = attr_1 & 0x1FF;

                        Bitmap bmp = (Bitmap)image.Get_Image(palettes[i]);
                        bmp.MakeTransparent(palettes[i].Palette[0][0]);
                        g.DrawImage(bmp, xpos, ypos);
                    }
                    g.Dispose();
                    res.Save("Frame" + i + ".png");
                }

                /*for (int i = 0; i < subimage_offs.Length; i++)
                {
                    for (int j = 0; j < subimage_offs[i].Count; j++)
                    {
                        stream.Position = block6 + subimage_offs[i][j];

                        uint tile_length = reader.ReadUInt32();
                        uint compression = (uint)palettes[i].Original.Length & 0xFF;
                        tile_length = tile_length >> 8;
                        if (compression == 0x10)
                            Console.WriteLine("Compressed; not implemented");

                        byte[] tiles = reader.ReadBytes((int)tile_length);

                        stream.Position = block4 + frame_offs[i] + 16;
                        ushort attr_0 = reader.ReadUInt16();
                        ushort attr_1 = reader.ReadUInt16();
                        GetSize(attr_0, attr_1, out int width, out int height);

                        RawImage image = new RawImage(tiles, TileForm.Horizontal, palettes[i].Depth, width, height, false);

                        Bitmap bmp = (Bitmap)image.Get_Image(palettes[i]);
                        bmp.MakeTransparent(palettes[i].Palette[0][0]);

                        bmp.Save("Frame" + i + "_Subframe" + j + ".png");
                    }
                }*/

                for (int i = 0; i < subimage_offs.Length; i++) //Each frame
                {
                    for (int j = 0; j < subimage_offs[i].Count; j++) //Each subframe
                    {
                        stream.Position = block4 + subframe_offs[i][j];
                        reader.ReadUInt32();
                        short xleft = reader.ReadInt16();
                        short ytop = reader.ReadInt16();
                        short xright = reader.ReadInt16();
                        short ybottom = reader.ReadInt16();

                        Bitmap res = new Bitmap(xright - xleft, ybottom - ytop);
                        Graphics g = Graphics.FromImage(res);

                        for (int k = 0; k < subimage_offs[i][j].Count; k++) //Each subframe part
                        {
                            stream.Position = block6 + subimage_offs[i][j][k];

                            uint tile_length = reader.ReadUInt32();
                            uint compression = (uint)palettes[i].Original.Length & 0xFF;
                            tile_length = tile_length >> 8;
                            if (compression == 0x10)
                                Console.WriteLine("Compressed; not implemented");

                            byte[] tiles = reader.ReadBytes((int)tile_length);

                            stream.Position = block4 + subframe_offs[i][j] + 16 + 8 * k;
                            ushort attr_0 = reader.ReadUInt16();
                            ushort attr_1 = reader.ReadUInt16();
                            GetSize(attr_0, attr_1, out int width, out int height);

                            RawImage image = new RawImage(tiles, TileForm.Horizontal, palettes[i].Depth, width, height, false);

                            int ypos = attr_0 & 0xFF;
                            int xpos = attr_1 & 0x1FF;

                            Bitmap bmp = (Bitmap)image.Get_Image(palettes[i]);
                            bmp.MakeTransparent(palettes[i].Palette[0][0]);
                            g.DrawImage(bmp, xpos, ypos);
                        }

                        g.Dispose();
                        res.Save("Frame" + i + "_SubFrame" + j + ".png");
                    }
                }
            }

            Console.WriteLine("ended");
            Console.ReadKey();
        }

        static void GetSize(ushort attr_0, ushort attr_1, out int width, out int height)
        {
            int size = attr_1 >> 14;

            if ((attr_0 & attr_0_tall) != 0)
            {
                width = size_tall[size].Width;
                height = size_tall[size].Height;
            }
            else if ((attr_0 & attr_0_wide) != 0)
            {
                width = size_wide[size].Width;
                height = size_wide[size].Height;
            }
            else
            {
                width = size_square[size].Width;
                height = size_square[size].Height;
            }
        }
    }
}
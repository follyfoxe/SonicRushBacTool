using System.Buffers.Binary;

namespace RushBacLib
{
    // https://wiibrew.org/wiki/LZ77
    public class WiiLZ77
    {
        public const int TypeLZ77 = 1;

        public BinaryReader Reader { get; }
        public long BaseOffset { get; }

        public uint UncompressedLength { get; }
        public uint CompressionType { get; }

        public WiiLZ77(BinaryReader reader, long offset)
        {
            Reader = reader;
            BaseOffset = offset;

            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            uint header = reader.ReadUInt32();

            UncompressedLength = header >> 8;
            CompressionType = header >> 4 & 0xF;

            if (CompressionType != TypeLZ77)
                throw new Exception("Unsupported compression method " + CompressionType);
        }

        public byte[] Uncompress()
        {
            byte[] result = new byte[UncompressedLength];
            int count = 0;

            Reader.BaseStream.Seek(BaseOffset + 0x4, SeekOrigin.Begin);
            while (count < UncompressedLength)
            {
                byte flags = Reader.ReadByte();
                for (int i = 0; i < 8; i++)
                {
                    if ((flags & 0x80) != 0)
                    {
                        ushort info = BinaryPrimitives.ReadUInt16BigEndian(Reader.ReadBytes(2));
                        int num = 3 + ((info >> 12) & 0xF);
                        // int disp = info & 0xFFF;
                        int ptr = count - (info & 0xFFF) - 1;
                        for (int j = 0; j < num; j++)
                        {
                            result[count++] = result[ptr];
                            ptr++;
                            if (count >= UncompressedLength)
                                break;
                        }
                    }
                    else
                        result[count++] = Reader.ReadByte();
                    flags <<= 1;
                    if (count >= UncompressedLength)
                        break;
                }
            }
            return result;
        }
    }
}

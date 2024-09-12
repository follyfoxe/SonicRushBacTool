using System.Diagnostics;
using System.Text;

namespace RushBacLib
{
    public class BacFile
    {
        public static readonly Encoding Encoding = Encoding.UTF8;

        public Header Header;
        public AnimationInfo AnimationInfo;
        public AnimationMappings AnimationMappings;
        public AnimationFrame[] AnimationFrames;

        public BacFile(string path) : this(File.OpenRead(path), false)
        {
        }

        public BacFile(Stream stream, bool leaveOpen = false)
        {
            using BinaryReader reader = new(stream, Encoding, leaveOpen);

            Header = new Header(reader);
            if (Header.AnimationInfo != 0x1C)
                Trace.WriteLine("Header points to Block 1 not being at 0x1C! Ignoring...");
            else // This seems wrong...
                reader.BaseStream.Seek(Header.AnimationInfo, SeekOrigin.Begin);

            AnimationInfo = new AnimationInfo(reader);
            Trace.WriteLine("Frame Count: " + AnimationInfo.EntryCount);

            reader.BaseStream.Position = Header.AnimationMappings;
            AnimationMappings = new AnimationMappings(this, reader);

            // Extract frames using AnimationMappings.
            AnimationFrames = new AnimationFrame[AnimationMappings.Mappings.Length];
            for (int i = 0; i < AnimationFrames.Length; i++)
            {
                reader.BaseStream.Position = Header.AnimationFrames + AnimationMappings.Mappings[i].FrameOffset - 4;
                AnimationFrames[i] = new AnimationFrame(this, reader);
            }
        }
    }
}
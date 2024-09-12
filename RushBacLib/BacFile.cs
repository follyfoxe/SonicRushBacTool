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
        public AnimationFrames[] AnimationFrames;

        public BacFile(string path) : this(File.OpenRead(path), false)
        {
        }

        public BacFile(Stream stream, bool leaveOpen = false)
        {
            using BinaryReader reader = new(stream, Encoding, leaveOpen);

            Header = new Header(reader);
            if (Header.AnimationInfo != 0x1C)
                Trace.WriteLine("Header doesn't point to Block 1 being at 0x1C.");
            reader.BaseStream.Seek(Header.AnimationInfo, SeekOrigin.Begin);

            AnimationInfo = new AnimationInfo(reader);
            Trace.WriteLine("Info Entry Count: " + AnimationInfo.EntryCount);

            reader.BaseStream.Seek(Header.AnimationMappings, SeekOrigin.Begin);
            AnimationMappings = new AnimationMappings(reader);

            if (AnimationMappings.Mappings.Length != AnimationInfo.EntryCount)
            {
                Trace.WriteLine("Calculated mapping count doesn't match header's entry count.");
                Trace.WriteLine($"Entry Count: {AnimationInfo.EntryCount}, Calculated Count: {AnimationMappings.Mappings.Length}");
            }

            // Extract frames using AnimationMappings.
            AnimationFrames = new AnimationFrames[AnimationMappings.Mappings.Length];
            for (int i = 0; i < AnimationFrames.Length; i++)
            {
                reader.BaseStream.Seek(Header.AnimationFrames + AnimationMappings.Mappings[i].FrameOffset, SeekOrigin.Begin);
                AnimationFrames[i] = new AnimationFrames(this, reader);
            }
        }
    }
}
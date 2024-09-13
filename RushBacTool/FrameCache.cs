using RushBacLib;
using System.Diagnostics;

namespace RushBacTool
{
    public class FrameCache
    {
        public BacFile BacFile { get; }
        public int AnimationCount => _cache.Length;
        readonly Bitmap[][] _cache;

        public FrameCache(BacFile bac)
        {
            BacFile = bac;
            _cache = new Bitmap[BacFile.AnimationFrames.Length][];
            for (int i = 0; i < _cache.Length; i++)
                _cache[i] = new Bitmap[BacFile.AnimationFrames[i].Frames.Count];
        }

        public int GetFrameCount(int animation)
        {
            return _cache[animation].Length;
        }

        public void ClearCache(int animation, int frame)
        {
            _cache[animation][frame]?.Dispose();
            _cache[animation][frame] = null;
        }

        public void ClearCache(int animation)
        {
            foreach (Bitmap frame in _cache[animation])
                frame?.Dispose();
            Array.Fill(_cache[animation], null);
        }

        public void ClearCache()
        {
            for (int i = 0; i < _cache.Length; i++)
                ClearCache(i);
        }

        public Bitmap GetImage(int animation, int frame)
        {
            return GetImage(animation, frame, true, BacFile.AnimationMappings.Mappings[animation].Unknown != 0);
        }

        public Bitmap GetImage(int animation, int frame, bool transparency, bool linear)
        {
            Bitmap image = _cache[animation][frame];
            if (image == null)
            {
                image = BacFile.AnimationFrames[animation].Frames[frame].GetImage(transparency, linear).ToBitmap();
                _cache[animation][frame] = image;
                Debug.WriteLine($"Cached ({animation}, {frame})");
            }
            return image;
        }
    }
}

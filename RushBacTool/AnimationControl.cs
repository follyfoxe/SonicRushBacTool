using RushBacLib;

namespace RushBacTool
{
    public partial class AnimationControl : UserControl
    {
        public FrameCache FrameCache { get; }
        public int AnimationIndex { get; }
        public AnimationFrames Animation => FrameCache.BacFile.AnimationFrames[AnimationIndex];

        int _currentFrame;
        bool _disposed;

        public AnimationControl()
        {
            InitializeComponent();
            Disposed += AnimFrameControl_Disposed;
            Dock = DockStyle.Fill;
        }

        void AnimFrameControl_Disposed(object sender, EventArgs e)
        {
            Stop();
            animTimer.Dispose();
            _disposed = true;
        }

        public AnimationControl(FrameCache frameCache, int animation) : this()
        {
            FrameCache = frameCache;
            AnimationIndex = animation;
            frameUpDown.Maximum = Animation.Frames.Count - 1;

            SetFrame(Animation.RestingFrame);
            UpdatePreview();
        }

        void SetFrame(int index)
        {
            if (index < 0 || index >= Animation.Frames.Count)
                return;
            _currentFrame = index;
            frameUpDown.Value = index;
        }

        void UpdatePreview()
        {
            if (_disposed)
                return;
            framePreview.Preview(FrameCache, AnimationIndex, _currentFrame);
        }

        void FrameUpDown_ValueChanged(object sender, EventArgs e)
        {
            _currentFrame = (int)frameUpDown.Value;
            UpdatePreview();
        }

        void AnimTimer_Tick(object sender, EventArgs e)
        {
            _currentFrame++;
            _currentFrame %= Animation.Frames.Count;

            // Assuming duration is in frames and the game runs at 60 fps...
            animTimer.Interval = (int)Animation.Frames[_currentFrame].Info.Duration * 16;
            frameUpDown.Value = _currentFrame;
        }

        void Play()
        {
            Stop();
            SetFrame(0);
            animTimer.Interval = 100;
            animTimer.Start();
        }

        void Stop()
        {
            animTimer.Stop();
            SetFrame(Animation.RestingFrame);
        }

        void PlayButton_Click(object sender, EventArgs e) => Play();
        void StopButton_Click(object sender, EventArgs e) => Stop();
    }
}
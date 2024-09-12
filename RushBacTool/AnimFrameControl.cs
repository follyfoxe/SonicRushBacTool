using RushBacLib;

namespace RushBacTool
{
    public partial class AnimFrameControl : UserControl
    {
        public int AnimationIndex { get; }
        public AnimationFrames Animation { get; }
        readonly MainForm _mainForm;

        int _currentFrame;
        bool _disposed;

        public AnimFrameControl()
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

        public AnimFrameControl(MainForm form, int animIndex) : this()
        {
            _mainForm = form;

            AnimationIndex = animIndex;
            Animation = form.BacFile.AnimationFrames[animIndex];
            frameUpDown.Maximum = Animation.Frames.Count - 1;

            titleLabel.Text += " " + animIndex;

            //SetFrame((int)Animation.RestingFrame);
            UpdatePreview();
        }

        void SetFrame(int index)
        {
            _currentFrame = index;
            frameUpDown.Value = index;
        }

        void UpdatePreview()
        {
            if (_disposed)
                return;
            framePreview.Preview(Animation.Frames[_currentFrame], _mainForm.Bitmaps[AnimationIndex][_currentFrame]);
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
            frameUpDown.Value = _currentFrame;
        }

        void Play()
        {
            SetFrame(0);
            animTimer.Start();
        }

        void Stop()
        {
            animTimer.Stop();
            //SetFrame((int)Animation.RestingFrame);
        }

        void PlayButton_Click(object sender, EventArgs e) => Play();
        void StopButton_Click(object sender, EventArgs e) => Stop();
    }
}
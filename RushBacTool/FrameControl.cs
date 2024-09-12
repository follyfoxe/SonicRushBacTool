namespace RushBacTool
{
    public partial class FrameControl : UserControl
    {
        public int AnimationIndex { get; }
        public int FrameIndex { get; }

        public FrameControl()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
        }

        public FrameControl(MainForm form, int animIndex, int frameIndex) : this()
        {
            AnimationIndex = animIndex;
            FrameIndex = frameIndex;
            framePreview.Preview(form.BacFile.AnimationFrames[animIndex].Frames[frameIndex], form.Bitmaps[animIndex][frameIndex]);
        }
    }
}
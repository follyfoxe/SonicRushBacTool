namespace RushBacTool
{
    public partial class FrameControl : UserControl
    {
        public FrameControl()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
        }

        public FrameControl(FrameCache frameCache, int animation, int frame) : this()
        {
            framePreview.Preview(frameCache, animation, frame);
        }
    }
}
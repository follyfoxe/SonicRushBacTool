using RushBacLib;
using System.Drawing.Drawing2D;

namespace RushBacTool
{
    public partial class FramePreviewControl : UserControl
    {
        public AnimationFrame Frame { get; private set; }
        public Bitmap Bitmap { get; private set; }
        HatchBrush _backBrush;

        public FramePreviewControl()
        {
            InitializeComponent();
            DoubleBuffered = true;

            _backBrush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.LightGray, Color.White);
            Disposed += OnDisposed;
        }

        public void Preview(AnimationFrame frame, Bitmap bitmap)
        {
            Frame = frame;
            Bitmap = bitmap;
            Refresh();
        }

        void OnDisposed(object sender, EventArgs e)
        {
            _backBrush?.Dispose();
            _backBrush = null;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(_backBrush, e.ClipRectangle);
            if (Frame != null && Bitmap != null)
            {
                Point pos = e.ClipRectangle.Location + e.ClipRectangle.Size / 2;
                Point topLeft = Frame.GetTopLeft(pos);
                Point bottomRight = Frame.GetBottomRight(pos);
                g.DrawRectangle(Pens.Red, topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
                g.DrawImage(Bitmap, topLeft);
            }
            base.OnPaint(e);
        }
    }
}

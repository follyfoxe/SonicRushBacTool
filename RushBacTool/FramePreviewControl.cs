using RushBacLib;
using System.Drawing.Drawing2D;

namespace RushBacTool
{
    public partial class FramePreviewControl : UserControl
    {
        public AnimationFrame Frame { get; private set; }
        public Bitmap Bitmap { get; private set; }

        HatchBrush _backBrush;
        Point _lastMousePos;

        bool _showGizmos = true;
        float _zoom = 2f;
        PointF _viewOffset = PointF.Empty;

        public FramePreviewControl()
        {
            InitializeComponent();
            DoubleBuffered = true;

            _backBrush = new HatchBrush(HatchStyle.LargeCheckerBoard, Color.LightGray, Color.White);
            Disposed += OnDisposed;
        }

        public void Preview(FrameCache frameCache, int animation, int frame)
        {
            Preview(frameCache.BacFile.AnimationFrames[animation].Frames[frame], frameCache.GetImage(animation, frame));
        }

        public void Preview(AnimationFrame frame, Bitmap bitmap)
        {
            Frame = frame;
            Bitmap = bitmap;
            Refresh();
        }

        public void SetZoom(float zoom)
        {
            _zoom = Math.Clamp(zoom, 0.1f, 100f);
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

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button != MouseButtons.None)
            {
                _viewOffset += ((Size)e.Location - (Size)_lastMousePos) / _zoom;
                Refresh();
            }
            _lastMousePos = e.Location;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            int scroll = Math.Sign(e.Delta);
            SetZoom(_zoom * (1f + scroll * 0.2f));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            Size size = ClientSize;
            Point center = ClientRectangle.Location + size / 2;
            g.FillRectangle(_backBrush, e.ClipRectangle);

            if (Frame != null && Bitmap != null)
            {
                g.TranslateTransform(center.X, center.Y);
                g.ScaleTransform(_zoom, _zoom);
                g.TranslateTransform(-center.X + _viewOffset.X, -center.Y + _viewOffset.Y);

                Point topLeft = Frame.GetTopLeft(center);
                Point bottomRight = Frame.GetBottomRight(center);
                if (_showGizmos)
                {
                    Pen gizmoPen = Pens.Red;
                    g.DrawRectangle(gizmoPen, topLeft.X, topLeft.Y, bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
                    g.DrawLine(gizmoPen, center.X - 8, center.Y, center.X + 8, center.Y);
                    g.DrawLine(gizmoPen, center.X, center.Y - 8, center.X, center.Y + 8);
                }
                g.DrawImage(Bitmap, topLeft);
            }

            base.OnPaint(e);
        }

        void ShowGizmosMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            _showGizmos = showGizmosMenuItem.Checked;
            Refresh();
        }

        void ResetZoomMenuItem_Click(object sender, EventArgs e)
        {
            _viewOffset = PointF.Empty;
            SetZoom(1f);
        }
    }
}

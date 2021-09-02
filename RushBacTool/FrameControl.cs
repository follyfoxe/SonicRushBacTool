using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RushBacTool
{
    public partial class FrameControl : UserControl
    {
        public int AnimIndex { get; }
        public int FrameIndex { get; }

        readonly MainForm mainForm;

        public FrameControl()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
        }

        public FrameControl(MainForm form, int animIndex, int frameIndex) : this()
        {
            mainForm = form;
            AnimIndex = animIndex;
            FrameIndex = frameIndex;

            previewBox.Image = form.bitmaps[animIndex][frameIndex];
        }
    }
}
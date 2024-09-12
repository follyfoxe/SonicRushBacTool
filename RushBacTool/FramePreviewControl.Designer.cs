namespace RushBacTool
{
    partial class FramePreviewControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            contextMenuStrip = new ContextMenuStrip(components);
            showGizmosMenuItem = new ToolStripMenuItem();
            resetZoomMenuItem = new ToolStripMenuItem();
            contextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenuStrip
            // 
            contextMenuStrip.ImageScalingSize = new Size(20, 20);
            contextMenuStrip.Items.AddRange(new ToolStripItem[] { showGizmosMenuItem, resetZoomMenuItem });
            contextMenuStrip.Name = "contextMenuStrip";
            contextMenuStrip.Size = new Size(168, 56);
            // 
            // showGizmosMenuItem
            // 
            showGizmosMenuItem.Checked = true;
            showGizmosMenuItem.CheckOnClick = true;
            showGizmosMenuItem.CheckState = CheckState.Checked;
            showGizmosMenuItem.Name = "showGizmosMenuItem";
            showGizmosMenuItem.Size = new Size(167, 26);
            showGizmosMenuItem.Text = "Show Gizmos";
            showGizmosMenuItem.CheckedChanged += ShowGizmosMenuItem_CheckedChanged;
            // 
            // resetZoomMenuItem
            // 
            resetZoomMenuItem.Name = "resetZoomMenuItem";
            resetZoomMenuItem.Size = new Size(167, 26);
            resetZoomMenuItem.Text = "Reset Zoom";
            resetZoomMenuItem.Click += ResetZoomMenuItem_Click;
            // 
            // FramePreviewControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BorderStyle = BorderStyle.FixedSingle;
            ContextMenuStrip = contextMenuStrip;
            Name = "FramePreviewControl";
            Size = new Size(148, 148);
            contextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem showGizmosMenuItem;
        private ToolStripMenuItem resetZoomMenuItem;
    }
}

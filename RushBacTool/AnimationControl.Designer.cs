
namespace RushBacTool
{
    partial class AnimationControl
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
            groupBox1 = new GroupBox();
            framePreview = new FramePreviewControl();
            playButton = new Button();
            stopButton = new Button();
            splitContainer1 = new SplitContainer();
            panel1 = new Panel();
            label2 = new Label();
            frameUpDown = new NumericUpDown();
            animTimer = new System.Windows.Forms.Timer(components);
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)frameUpDown).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(framePreview);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Margin = new Padding(4, 5, 4, 5);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 5, 4, 5);
            groupBox1.Size = new Size(781, 463);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Frame Preview";
            // 
            // framePreview
            // 
            framePreview.BorderStyle = BorderStyle.FixedSingle;
            framePreview.Dock = DockStyle.Fill;
            framePreview.Location = new Point(4, 25);
            framePreview.Name = "framePreview";
            framePreview.Size = new Size(773, 433);
            framePreview.TabIndex = 0;
            // 
            // playButton
            // 
            playButton.Location = new Point(4, 5);
            playButton.Margin = new Padding(4, 5, 4, 5);
            playButton.Name = "playButton";
            playButton.Size = new Size(80, 30);
            playButton.TabIndex = 5;
            playButton.Text = "Play";
            playButton.UseVisualStyleBackColor = true;
            playButton.Click += PlayButton_Click;
            // 
            // stopButton
            // 
            stopButton.Location = new Point(92, 5);
            stopButton.Margin = new Padding(4, 5, 4, 5);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(80, 30);
            stopButton.TabIndex = 6;
            stopButton.Text = "Stop";
            stopButton.UseVisualStyleBackColor = true;
            stopButton.Click += StopButton_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Margin = new Padding(4, 5, 4, 5);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(panel1);
            splitContainer1.Panel2.Controls.Add(playButton);
            splitContainer1.Panel2.Controls.Add(stopButton);
            splitContainer1.Size = new Size(781, 591);
            splitContainer1.SplitterDistance = 463;
            splitContainer1.SplitterWidth = 6;
            splitContainer1.TabIndex = 7;
            // 
            // panel1
            // 
            panel1.Controls.Add(label2);
            panel1.Controls.Add(frameUpDown);
            panel1.Dock = DockStyle.Right;
            panel1.Location = new Point(552, 0);
            panel1.Margin = new Padding(4, 5, 4, 5);
            panel1.Name = "panel1";
            panel1.Size = new Size(229, 122);
            panel1.TabIndex = 9;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(4, 8);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(102, 20);
            label2.TabIndex = 7;
            label2.Text = "Current Frame";
            // 
            // frameUpDown
            // 
            frameUpDown.Location = new Point(109, 5);
            frameUpDown.Margin = new Padding(4, 5, 4, 5);
            frameUpDown.Name = "frameUpDown";
            frameUpDown.Size = new Size(115, 27);
            frameUpDown.TabIndex = 8;
            frameUpDown.ValueChanged += FrameUpDown_ValueChanged;
            // 
            // animTimer
            // 
            animTimer.Tick += AnimTimer_Tick;
            // 
            // AnimationControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Margin = new Padding(4, 5, 4, 5);
            Name = "AnimationControl";
            Size = new Size(781, 591);
            groupBox1.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)frameUpDown).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.NumericUpDown frameUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Timer animTimer;
        private FramePreviewControl framePreview;
    }
}

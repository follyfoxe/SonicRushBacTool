
namespace RushBacTool
{
    partial class AnimFrameControl
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
            titleLabel = new Label();
            groupBox1 = new GroupBox();
            previewBox = new PictureBox();
            playButton = new Button();
            stopButton = new Button();
            splitContainer1 = new SplitContainer();
            panel1 = new Panel();
            label2 = new Label();
            frameUpDown = new NumericUpDown();
            animTimer = new System.Windows.Forms.Timer(components);
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)previewBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)frameUpDown).BeginInit();
            SuspendLayout();
            // 
            // titleLabel
            // 
            titleLabel.Dock = DockStyle.Top;
            titleLabel.Location = new Point(0, 0);
            titleLabel.Margin = new Padding(4, 0, 4, 0);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(781, 28);
            titleLabel.TabIndex = 3;
            titleLabel.Text = "Animation";
            titleLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(previewBox);
            groupBox1.Dock = DockStyle.Fill;
            groupBox1.Location = new Point(0, 0);
            groupBox1.Margin = new Padding(4, 5, 4, 5);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(4, 5, 4, 5);
            groupBox1.Size = new Size(771, 335);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Preview";
            // 
            // previewBox
            // 
            previewBox.Dock = DockStyle.Fill;
            previewBox.Location = new Point(4, 25);
            previewBox.Margin = new Padding(4, 5, 4, 5);
            previewBox.Name = "previewBox";
            previewBox.Size = new Size(763, 305);
            previewBox.SizeMode = PictureBoxSizeMode.CenterImage;
            previewBox.TabIndex = 0;
            previewBox.TabStop = false;
            // 
            // playButton
            // 
            playButton.Location = new Point(4, 5);
            playButton.Margin = new Padding(4, 5, 4, 5);
            playButton.Name = "playButton";
            playButton.Size = new Size(83, 35);
            playButton.TabIndex = 5;
            playButton.Text = "Play";
            playButton.UseVisualStyleBackColor = true;
            playButton.Click += PlayButton_Click;
            // 
            // stopButton
            // 
            stopButton.Location = new Point(95, 5);
            stopButton.Margin = new Padding(4, 5, 4, 5);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(81, 35);
            stopButton.TabIndex = 6;
            stopButton.Text = "Stop";
            stopButton.UseVisualStyleBackColor = true;
            stopButton.Click += StopButton_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.BorderStyle = BorderStyle.FixedSingle;
            splitContainer1.Location = new Point(4, 32);
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
            splitContainer1.Size = new Size(773, 554);
            splitContainer1.SplitterDistance = 337;
            splitContainer1.SplitterWidth = 6;
            splitContainer1.TabIndex = 7;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            panel1.Controls.Add(label2);
            panel1.Controls.Add(frameUpDown);
            panel1.Location = new Point(537, 5);
            panel1.Margin = new Padding(4, 5, 4, 5);
            panel1.Name = "panel1";
            panel1.Size = new Size(229, 197);
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
            // AnimFrameControl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(splitContainer1);
            Controls.Add(titleLabel);
            Margin = new Padding(4, 5, 4, 5);
            Name = "AnimFrameControl";
            Size = new Size(781, 591);
            groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)previewBox).EndInit();
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

        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox previewBox;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.NumericUpDown frameUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Timer animTimer;
    }
}

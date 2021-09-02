using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using RushBacLib;

namespace RushBacTool
{
    public partial class MainForm : Form
    {
        public BacFile bacFile;
        public Bitmap[][] bitmaps;
        string fileName;

        public MainForm(string[] args) : this()
        {
            if (args.Length > 0)
                LoadBac(args[0]);
        }

        public MainForm()
        {
            InitializeComponent();
            Disposed += OnDisposed;

            //LoadBac(@"C:\Users\ACER\Desktop\dsi3dsStuff\DS\SonicRush\ac_eff_boost.bac");
        }

        void OnDisposed(object sender, EventArgs e)
        {
            DisposeBitmaps();
        }

        void LoadBac(string path)
        {
            ResetControls();
            DisposeBitmaps();

            fileName = System.IO.Path.GetFileName(path);
            Text = $"Rush Bac Tool [{fileName}]";

            Console.WriteLine("Begin load BAC {0}", fileName);
            Stopwatch stopwatch = Stopwatch.StartNew();
            bacFile = new BacFile(path);
            stopwatch.Stop();
            Console.WriteLine("Finished loading BAC in {0} ms.\n", stopwatch.ElapsedMilliseconds);

            CacheBitmaps();
            CreateTree();
        }

        void ResetControls()
        {
            treeView.Nodes.Clear();
            foreach (Control c in propGroup.Controls)
                c.Dispose();
            propGroup.Controls.Clear();
        }

        void CreateTree()
        {
            treeView.BeginUpdate();

            TreeNode root = new TreeNode { Text = fileName, Tag = "ROOT" };

            for (int i = 0; i < bacFile.AnimationFrames.Length; i++)
            {
                AnimationFrame animFrame = bacFile.AnimationFrames[i];
                TreeNode anim = new TreeNode { Text = "Animation Frame " + i, Tag = animFrame };

                for (int j = 0; j < animFrame.frames.Count; j++)
                {
                    Frame frame = animFrame.frames[j];
                    TreeNode node = new TreeNode { Text = "Frame " + j, Tag = new KeyValuePair<int, Frame>(i, frame) };
                    anim.Nodes.Add(node);
                }
                root.Nodes.Add(anim);
            }

            root.Expand();
            treeView.Nodes.Add(root);
            treeView.EndUpdate();
        }

        void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propGroup.Controls.Clear();
            TreeNode node = e.Node;

            if (node.Tag is KeyValuePair<int, Frame> framePair)
            {
                FrameControl c = new FrameControl(this, framePair.Key, node.Index);
                propGroup.Controls.Add(c);
            }
            if (node.Tag is AnimationFrame)
            {
                AnimFrameControl c = new AnimFrameControl(this, node.Index);
                propGroup.Controls.Add(c);
            }
        }

        void CacheBitmaps()
        {
            bitmaps = new Bitmap[bacFile.AnimationFrames.Length][];
            for (int i = 0; i < bitmaps.Length; i++)
            {
                AnimationFrame anim = bacFile.AnimationFrames[i];
                Bitmap[] frames = new Bitmap[anim.frames.Count];
                for (int j = 0; j < frames.Length; j++)
                    frames[j] = anim.frames[j].GetBitmap();
                bitmaps[i] = frames;
            }
        }

        void DisposeBitmaps()
        {
            if (bitmaps == null) return;
            foreach (Bitmap[] frames in bitmaps)
                foreach (Bitmap b in frames)
                    b.Dispose();
            bitmaps = null;
        }

        void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog { Filter = "Bac Files (*.bac)|*.bac" })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    LoadBac(dialog.FileName);
            }
        }
    }
}
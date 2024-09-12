using RushBacLib;
using System.Diagnostics;

namespace RushBacTool
{
    public partial class MainForm : Form
    {
        public BacFile BacFile;
        public Bitmap[][] Bitmaps;

        readonly string _baseTitle;
        string _openedFileName;

        public MainForm()
        {
            InitializeComponent();
            _baseTitle = Text;
            Disposed += OnDisposed;
        }

        public MainForm(string[] args) : this()
        {
            if (args.Length > 0)
                LoadBac(args[0]);
        }

        void OnDisposed(object sender, EventArgs e)
        {
            DisposeBitmaps();
        }

        void LoadBac(string path)
        {
            _openedFileName = Path.GetFileName(path);
            Text = $"{_baseTitle} [{_openedFileName}]";

            ResetControls();
            DisposeBitmaps();

            Trace.WriteLine($"Begin load {_openedFileName}");
            Stopwatch sw = Stopwatch.StartNew();

            BacFile = new BacFile(path);
            CacheBitmaps();
            CreateTree();

            sw.Stop();
            Trace.WriteLine($"Finished loading in {sw.ElapsedMilliseconds} ms.\n");
        }

        void ExportAll(string path)
        {
            Trace.WriteLine($"Begin export all {_openedFileName}");
            Stopwatch sw = Stopwatch.StartNew();

            Directory.CreateDirectory(path);
            for (int i = 0; i < Bitmaps.Length; i++)
            {
                Bitmap[] frames = Bitmaps[i];

                string dir = Path.Combine(path, "Animation " + i);
                Directory.CreateDirectory(dir);

                for (int j = 0; j < frames.Length; j++)
                    frames[j].Save(Path.Combine(dir, "Frame " + j + ".png"));
            }

            sw.Stop();
            Trace.WriteLine($"Finished exporting in {sw.ElapsedMilliseconds} ms.\n");

            MessageBox.Show("Export Finished!");
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

            TreeNode root = new() { Text = _openedFileName, Tag = "ROOT" };
            for (int i = 0; i < BacFile.AnimationFrames.Length; i++)
            {
                AnimationFrames animFrame = BacFile.AnimationFrames[i];
                TreeNode anim = new() { Text = "Animation " + i, Tag = animFrame };

                for (int j = 0; j < animFrame.Frames.Count; j++)
                {
                    AnimationFrame frame = animFrame.Frames[j];
                    TreeNode node = new() { Text = "Frame " + j, Tag = new KeyValuePair<int, AnimationFrame>(i, frame) };
                    anim.Nodes.Add(node);
                }
                root.Nodes.Add(anim);
            }

            root.Expand();
            treeView.Nodes.Add(root);
            treeView.EndUpdate();
        }

        void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            foreach (Control c in propGroup.Controls)
                c.Dispose();
            propGroup.Controls.Clear();
            TreeNode node = e.Node;

            if (node.Tag is KeyValuePair<int, AnimationFrame> framePair)
            {
                FrameControl c = new(this, framePair.Key, node.Index);
                propGroup.Controls.Add(c);
            }
            if (node.Tag is AnimationFrames)
            {
                AnimFrameControl c = new(this, node.Index);
                propGroup.Controls.Add(c);
            }
        }

        void CacheBitmaps()
        {
            Bitmaps = new Bitmap[BacFile.AnimationFrames.Length][];
            for (int i = 0; i < Bitmaps.Length; i++)
            {
                AnimationFrames anim = BacFile.AnimationFrames[i];
                Bitmap[] frames = new Bitmap[anim.Frames.Count];
                for (int j = 0; j < frames.Length; j++)
                {
                    ImageResult image = anim.Frames[j].GetImage(true);
                    Bitmap b = new(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    var data = b.LockBits(new(Point.Empty, b.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, b.PixelFormat);
                    System.Runtime.InteropServices.Marshal.Copy(image.ToArgb(), 0, data.Scan0, data.Stride * data.Height);
                    b.UnlockBits(data);
                    frames[j] = b;
                }
                Bitmaps[i] = frames;
            }
        }

        void DisposeBitmaps()
        {
            if (Bitmaps == null)
                return;
            foreach (Bitmap[] frames in Bitmaps)
            {
                foreach (Bitmap b in frames)
                    b.Dispose();
            }
            Bitmaps = null;
        }

        void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new() { Filter = "Bac Files (*.bac)|*.bac" };
            if (dialog.ShowDialog() == DialogResult.OK)
                LoadBac(dialog.FileName);
        }

        void ExportAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (BacFile == null)
                return;
            using SaveFileDialog dialog = new() { Title = "Select an output Folder.", FileName = "out" };
            if (dialog.ShowDialog() == DialogResult.OK)
                ExportAll(dialog.FileName);
        }
    }
}
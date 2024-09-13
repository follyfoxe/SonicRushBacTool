using RushBacLib;
using System.Diagnostics;

namespace RushBacTool
{
    public partial class MainForm : Form
    {
        public BacFile BacFile { get; private set; }
        public FrameCache FrameCache { get; private set; }

        readonly string _baseTitle;
        string _openedFileName;

        public MainForm()
        {
            InitializeComponent();

            _baseTitle = Text;
            ResetControls();

            Disposed += OnDisposed;
        }

        public MainForm(string[] args) : this()
        {
            if (args.Length > 0)
                LoadBac(args[0]);
        }

        void OnDisposed(object sender, EventArgs e)
        {
            FrameCache?.ClearCache();
            FrameCache = null;
        }

        void LoadBac(string path)
        {
            _openedFileName = Path.GetFileName(path);
            Text = $"{_baseTitle} [{_openedFileName}]";

            ResetControls();
            FrameCache?.ClearCache();

            Trace.WriteLine($"Begin load {_openedFileName}");
            Stopwatch sw = Stopwatch.StartNew();

            BacFile = new BacFile(path);
            FrameCache = new FrameCache(BacFile);
            CreateTree();

            sw.Stop();
            Trace.WriteLine($"Finished loading in {sw.ElapsedMilliseconds} ms.\n");
        }

        void ExportAll(string path)
        {
            ResetControls();
            FrameCache?.ClearCache();

            Trace.WriteLine($"Begin export all {_openedFileName}");
            Stopwatch sw = Stopwatch.StartNew();
            Directory.CreateDirectory(path);

            for (int i = 0; i < FrameCache.AnimationCount; i++)
            {
                string dir = Path.Combine(path, "Animation " + i);
                Directory.CreateDirectory(dir);

                for (int j = 0; j < FrameCache.GetFrameCount(i); j++)
                    FrameCache.GetImage(i, j).Save(Path.Combine(dir, "Frame " + j + ".png"));
            }

            sw.Stop();
            Trace.WriteLine($"Finished exporting in {sw.ElapsedMilliseconds} ms.\n");
            MessageBox.Show("Successfully exported to " + path, "Export");
        }

        void ResetControls()
        {
            selectionLabel.Text = "(Nothing)";
            treeView.Nodes.Clear();
            Inspect(null);
        }

        void Inspect(object target)
        {
            foreach (Control c in propertyGroup.Controls)
                c.Dispose();
            propertyGroup.Controls.Clear();

            Control control;
            switch (target)
            {
                case null:
                    return;
                case Control c:
                    control = c;
                    break;
                case int i:
                    control = new AnimationControl(FrameCache, i);
                    break;
                case (int i, int j):
                    control = new FrameControl(FrameCache, i, j);
                    break;
                default:
                    control = new PropertyGrid() { SelectedObject = target, PropertySort = PropertySort.Categorized };
                    break;
            }
            control.Dock = DockStyle.Fill;
            propertyGroup.Controls.Add(control);
        }

        void CreateTree()
        {
            treeView.BeginUpdate();
            TreeNode root = new() { Text = _openedFileName, Tag = BacFile };
            for (int i = 0; i < BacFile.AnimationFrames.Length; i++)
            {
                AnimationFrames animation = BacFile.AnimationFrames[i];
                TreeNode animNode = new()
                {
                    Text = $"Animation {i} - Size {animation.Frames.Count}",
                    Tag = i
                };
                for (int j = 0; j < animation.Frames.Count; j++)
                {
                    animNode.Nodes.Add(new TreeNode()
                    {
                        Text = "Frame " + j,
                        Tag = (i, j)
                    });
                }
                root.Nodes.Add(animNode);
            }

            root.Expand();
            treeView.Nodes.Add(root);
            treeView.SelectedNode = root;
            treeView.EndUpdate();
        }

        void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selectionLabel.Text = e.Node.Text;
            Inspect(e.Node.Tag);
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
            using FolderBrowserDialog dialog = new()
            {
                Description = "Select an output folder",
                UseDescriptionForTitle = true,
                AutoUpgradeEnabled = true
            };
            if (dialog.ShowDialog() == DialogResult.OK)
                ExportAll(dialog.SelectedPath);
        }

        void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
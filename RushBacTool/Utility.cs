using RushBacLib;

namespace RushBacTool
{
    public static class Utility
    {
        public static Bitmap ToBitmap(this ImageResult image)
        {
            if (image.Width == 0 || image.Height == 0)
                return null;
            Bitmap result = new(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bitmapData = result.LockBits(new(Point.Empty, result.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, result.PixelFormat);
            System.Runtime.InteropServices.Marshal.Copy(image.ToArgb(), 0, bitmapData.Scan0, bitmapData.Stride * bitmapData.Height);
            result.UnlockBits(bitmapData);
            return result;
        }

        /*public static TreeView BuildFieldTree(object target)
        {
            TreeView tree = new();
            tree.BeforeExpand += FieldTreeExpand;
            tree.BeginUpdate();

            TreeNode root = BuildFieldTreeNode("Root", target);
            root.Expand();
            tree.Nodes.Add(root);

            tree.EndUpdate();
            return tree;
        }

        static void FieldTreeExpand(object sender, TreeViewCancelEventArgs args)
        {
            TreeNode node = args.Node;
            if (node.Nodes.Count == 1 && node.Nodes[0].Tag as string == "DUMMY")
            {
                node.Nodes.Clear();
                object target = node.Tag;
                switch (target)
                {
                    case ICollection collection:
                        int index = 0;
                        foreach (object item in collection)
                        {
                            node.Nodes.Add(BuildFieldTreeNode($"[{index}]", item));
                            index++;
                        }
                        break;
                    default:
                        Type type = target.GetType();
                        if (!type.IsPrimitive)
                        {
                            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                                node.Nodes.Add(BuildFieldTreeNode(field.Name, field.GetValue(target)));
                        }
                        break;
                }
            }
        }

        public static TreeNode BuildFieldTreeNode(string name, object target)
        {
            TreeNode node = new() { Tag = target };
            if (target.GetType().IsPrimitive)
                node.Text = $"{name} = {target}";
            else
            {
                node.Text = $"{name} ({target})";
                node.Nodes.Add(new TreeNode() { Tag = "DUMMY" });
            }
            return node;
        }*/
    }
}

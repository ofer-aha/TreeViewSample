using System;
using System.Windows.Forms;
using System.Xml;

namespace WinFormsApp3
{
    public partial class Form1 : Form
    {
        private TreeNode mySelectedNode;

        public Form1()
        {
            InitializeComponent();
            treeView1.Nodes.Add("Default");
            treeView1.HideSelection = false;
            treeView1.PathSeparator = @"\";

        }
        private void AddNode()
        {
            mySelectedNode = treeView1.SelectedNode.Nodes.Add(" ");

            treeView1.SelectedNode = mySelectedNode;
            mySelectedNode.Expand();

            RenameNode();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            AddNode();
        }
        private void DelNode()
        {
            TreeNode node = treeView1.SelectedNode;

            if (node.Parent != null)
            {
                treeView1.SelectedNode.Nodes.Remove(treeView1.SelectedNode);
            }
            else
            {
                MessageBox.Show("Root node");
            }

        }
        private void Button2_Click(object sender, EventArgs e)
        {
            DelNode();
        }


        public void ListNodesPath(TreeNode oParentNode)
        {
            // Start recursion on all subnodes.
            foreach (TreeNode oSubNode in oParentNode.Nodes)
            {
                if (oSubNode.Nodes.Count == 0)
                {
                    listBox1.Items.Add(oSubNode.FullPath);
                }
                ListNodesPath(oSubNode);
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (TreeNode node in treeView1.Nodes)
            {
                if (node.Parent == null)
                {
                    ListNodesPath(node);
                }
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            listBox1.Items.Clear();
            treeView1.Nodes.Add("Default");
            treeView1.SelectedNode = treeView1.Nodes[0];
        }

        private void MenueAdd_Click(object sender, EventArgs e)
        {
            AddNode();

        }

        private void MenuDel_Click(object sender, EventArgs e)
        {
            DelNode();
        }

        /// <summary>
        /// Get the tree node under the mouse pointer and 
        /// save it in the mySelectedNode variable.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeView1_MouseDown(object sender, MouseEventArgs e)
        {
            mySelectedNode = treeView1.GetNodeAt(e.X, e.Y);
        }

        private void MenuRename_Click(object sender, EventArgs e)
        {
            RenameNode();
        }

        private void RenameNode()
        {
            if (mySelectedNode != null && mySelectedNode.Parent != null)
            {
                treeView1.SelectedNode = mySelectedNode;
                treeView1.LabelEdit = true;
                if (!mySelectedNode.IsEditing)
                {
                    mySelectedNode.BeginEdit();
                }
            }
            else
            {
                MessageBox.Show("No tree node selected or selected node is a root node.\n" +
                   "Editing of root nodes is not allowed.", "Invalid selection");
            }
        }

        private void TreeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                if (e.Label.Length > 0)
                {

                    if (e.Label.IndexOfAny(new char[] { '@', '.', ',', '!' }) == -1)
                    {
                        // Stop editing without canceling the label change.
                        e.Node.EndEdit(false);
                    }
                    else
                    {
                        /* Cancel the label edit action, inform the user, and
                           place the node in edit mode again. */
                        e.CancelEdit = true;
                        MessageBox.Show("Invalid tree node label.\n" +
                           "The invalid characters are: '@','.', ',', '!'",
                           "Node Label Edit");
                        e.Node.BeginEdit();
                    }
                }
                else
                {
                    /* Cancel the label edit action, inform the user, and
                       place the node in edit mode again. */
                    e.CancelEdit = true;
                    MessageBox.Show("Invalid tree node label.\nThe label cannot be blank",
                       "Node Label Edit");
                    e.Node.BeginEdit();
                }
            }
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode.Nodes.Add("A");
            node.Nodes.Add("A1");
            node.Nodes.Add("A2").Nodes.Add("A3");
            node = treeView1.SelectedNode.Nodes.Add("B").Nodes.Add("B4");
            node.Nodes.Add("B1");
            node.Parent.Nodes.Add("B2");
            node.Parent.Nodes.Add("B3");

            treeView1.ExpandAll();

        }

        private void Button6_Click(object sender, EventArgs e)
        {
            Close();
        }

        XmlDocument xmlDocument;

        public void TreeViewToXml(TreeView treeView, String path)
        {

            xmlDocument = new XmlDocument();
            TreeNodeCollection nodes = null;
            foreach (TreeNode treeNode in treeView.Nodes)
            {

                xmlDocument.AppendChild(xmlDocument.CreateElement(treeNode.Text));
                nodes = treeNode.Nodes;
            }

            XmlExport(xmlDocument.DocumentElement, nodes);
            xmlDocument.Save(path);
        }

        private XmlNode XmlExport(XmlNode nodeElement, TreeNodeCollection treeNodeCollection)
        {

            XmlNode xmlNode = null;

            foreach (TreeNode treeNode in treeNodeCollection)

            {

                xmlNode = xmlDocument.CreateElement(treeNode.Text);
                string[] node = xmlNode.Name.Split(':');

                if (node[0] == "ATTRIBUTE")
                {

                    if (node[0] != null && node[1] != null)
                    {

                        XmlAttribute newAttribute = xmlDocument.CreateAttribute(node[1]);
                        nodeElement.Attributes.Append(newAttribute);
                    }
                }

                else
                {
                    if (nodeElement != null) nodeElement.AppendChild(xmlNode);
                }

                if (treeNode.Nodes.Count > 0)
                {
                    XmlExport(xmlNode, treeNode.Nodes);
                }
            }
            return xmlNode;

        }
    }
}
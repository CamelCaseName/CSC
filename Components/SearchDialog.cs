using CSC.Nodestuff;
using System.Diagnostics;
using System.Media;
using System.Threading.Tasks;
using static CSC.StoryItems.StoryEnums;

namespace CSC.Components
{
    public partial class SearchDialog : Form
    {
        public SearchDialog(Dictionary<string, NodeStore> stores)
        {
            InitializeComponent();

            if (!SearchTrie.Initialized)
            {
                Cursor = Cursors.WaitCursor;
                SearchTrie.Initialize(stores);
                Text = $"Building Search index, estimated wait time is {SearchTrie.NodeCount / 7000}s";
            }

            SearchTrie.OnSearchDataChange += SearchImpl;
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs? e)
        {
            SearchTrie.OnSearchDataChange -= SearchImpl;
        }

        private void Nodetype_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Datatype_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Searchterm_TextChanged(object sender, EventArgs e)
        {
            SearchImpl();
        }

        private void SearchImpl()
        {
            if (!SearchTrie.Initialized)
            {
                SystemSounds.Beep.Play();
                return;
            }

            resultsTree.Enabled = false;

            var results = SearchTrie.Search(searchterm.Text);

            int SearchMax = 0;
            List<TreeNode> treeNodes = [];
            foreach (var result in results)
            {
                SearchMax++;
                //todo setting and toggle for limit
                if (SearchMax > 500)
                {
                    break;
                }
                var treen = new TreeNode($"{result.FileName} | {result.Type} |ID: {result.ID} | Content:({result.Text})")
                {
                    Tag = result
                };
                foreach (var file in result.DupedFileNames)
                {
                    treen.Nodes.Add("Also used in: " + file);
                }

                treeNodes.Add(treen);
            }

            resultsTree.SuspendLayout();
            resultsTree.Nodes.Clear();
            resultsTree.Nodes.AddRange([.. treeNodes]);
            resultsTree.PerformLayout();
            resultsTree.Enabled = true;
            return;
        }

        private void Modifiers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Casesensitivity_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Results_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode? sNode = resultsTree.SelectedNode;

            if (sNode is null)
            {
                return;
            }

            //is node, not a filename
            if (sNode.Text.Contains('|'))
            {
                if (sNode.Tag is Node n)
                {
                    Main.SelectFile(n.FileName);
                    Main.CenterAndSelectNode(n);
                }
            }
            else
            {
                if ((sNode.Parent?.Text?.Contains('|') ?? false)
                    && sNode.Parent.Tag is Node n)
                {
                    Main.SelectFile(sNode.Text.Split(':')[1].Trim());
                    Main.CenterAndSelectNode(n);
                }
            }
        }
    }
}

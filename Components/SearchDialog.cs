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
                SearchTrie.Initialize(stores);
            }
        }

        private void Nodetype_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Datatype_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Searchterm_TextChanged(object sender, EventArgs e)
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
                if (SearchMax > 500)
                {
                    break;
                }
                var treen = new TreeNode($"{result.FileName} | {result.Type} |ID: {result.ID} | Content:({result.Text})");
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
        }

        private void Modifiers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Casesensitivity_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void Results_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}

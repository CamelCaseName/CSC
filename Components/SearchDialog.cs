using CSC.Nodestuff;
using System.Diagnostics;

namespace CSC.Components
{
    internal record struct FileAssignedNode(string StoreName, Node ValueNode)
    {
        public static implicit operator (string storeName, Node node)(FileAssignedNode value) => (value.StoreName, value.ValueNode);
        public static implicit operator FileAssignedNode((string storeName, Node node) value) => new(value.storeName, value.node);
    }

    internal class SearchTreeLevel : Dictionary<char, Tuple<List<FileAssignedNode>, SearchTreeLevel?>> { }

    public partial class SearchDialog : Form
    {
        SearchTreeLevel searchTree = [];

        public SearchDialog(Dictionary<string, NodeStore> stores)
        {
            InitializeComponent();
            //use lookup<T> as structure to build a TRIE
            //Debugger.Break();
            Debug.WriteLine("start");
            foreach (var store in stores)
            {
                foreach (var node in store.Value.Nodes)
                {
                    //todo fix nodes text return...
                    var nodeText = node.Text;
                    if (nodeText.Length > 0)
                    {
                        var level = searchTree;
                        for (int i = 0; i < nodeText.Length; i++)
                        {
                            if (!level!.TryGetValue(nodeText[i], out var tup))
                            {
                                tup = new([], []);
                                level.Add(nodeText[i], tup);
                            }

                            if (i < nodeText.Length - 1)
                            {
                                level = tup.Item2;
                            }
                            //Debug.WriteLine(nodeText[i]);
                        }

                        level![nodeText[^1]].Item1.Add((store.Key, node));

                    }
                    else
                    {
                        //Debugger.Break();
                    }
                }
            }

            Debug.WriteLine("end");
            //Debugger.Break();

        }

        private void nodetype_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void datatype_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void searchterm_TextChanged(object sender, EventArgs e)
        {

        }

        private void modifiers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void casesensitivity_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void results_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }
    }
}

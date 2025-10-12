using CSC.Nodestuff;
using System.Diagnostics;
using System.Xml.Linq;

namespace CSC.Components
{
    internal class FileAssignedNode : Dictionary<string, List<Node>> { }

    internal class SearchTreeNode
    {
        public FileAssignedNode list = [];
        public SearchTreeLevel tree = [];
    }

    internal class SearchTreeLevel : Dictionary<char, SearchTreeNode> { }

    public partial class SearchDialog : Form
    {
        private const int StartsWithTreeDepth = 25;
        private const int ContainsTreeDepth = 10;
        private static readonly SearchTreeLevel StartsWithTree = [];
        private static readonly SearchTreeLevel ContainsTree = [];

        public SearchDialog(Dictionary<string, NodeStore> stores)
        {
            InitializeComponent();
            //use lookup<T> as structure to build a TRIE
            //Debugger.Break();
            var start = DateTime.UtcNow;

            StartsWithTree.Clear();
            int nodecounter = 0;

            foreach (var store in stores)
            {
                nodecounter += store.Value.Nodes.Count;
                foreach (var node in store.Value.Nodes)
                {
                    //todo fix nodes text return...
                    var nodeText = node.Text;
                    if (nodeText.Length > 0)
                    {
                        //startswith() lookup up to a length
                        var level = StartsWithTree;
                        int limit = Math.Min(nodeText.Length, StartsWithTreeDepth);
                        for (int i = 0; i < limit; i++)
                        {
                            if (!level.TryGetValue(nodeText[i], out var tup))
                            {
                                tup = new();
                                level.Add(nodeText[i], tup);
                            }

                            if (i < limit - 1)
                            {
                                level = tup.tree;
                            }
                            //Debug.WriteLine(nodeText[i]);
                        }

                        //add node at last step
                        FileAssignedNode lastList = level[nodeText[limit - 1]].list;
                        if (lastList.TryGetValue(store.Key, out var list))
                        {
                            list.Add(node);
                        }
                        else
                        {
                            lastList.Add(store.Key, [node]);
                        }

                        //contains() lookup up to a certain string starting lenght
                        for (int j = 0; j < nodeText.Length; j++)
                        {
                            var container = ContainsTree;
                            int containerlimit = Math.Min(nodeText.Length - j, ContainsTreeDepth);
                            for (int i = 0; i < containerlimit; i++)
                            {
                                if (!container.TryGetValue(nodeText[i + j], out var tup))
                                {
                                    tup = new();
                                    container.Add(nodeText[i + j], tup);
                                }

                                //step in only if we want to
                                if (i < (containerlimit - 1))
                                {
                                    container = tup.tree;
                                }

                                //add node at each step
                                if (tup.list.TryGetValue(store.Key, out var list2))
                                {
                                    list2.Add(node);
                                }
                                else
                                {
                                    tup.list.Add(store.Key, [node]);
                                }
                            }
                        }
                    }
                    else
                    {
                        //Debugger.Break();
                    }
                }
            }

            var end = DateTime.UtcNow;
            Debug.WriteLine($"populated the search trees for {nodecounter} nodes with {StartsWithTreeDepth} steps for StartsWith() and {ContainsTreeDepth} steps for Contains() in {(end - start).Milliseconds}ms");

            HashSet<Node> returns = [];

            var testSearch = "od helper";

            start = DateTime.UtcNow;

            var testlevel = StartsWithTree;
            var testcontains = ContainsTree;
            int testlimit = Math.Min(testSearch.Length, StartsWithTreeDepth);
            int testcontainslimit = Math.Min(testSearch.Length, ContainsTreeDepth);

            for (int i = 0; i < testlimit; i++)
            {
                if (testlevel.TryGetValue(testSearch[i], out var tup))
                {
                    if (i < testlimit - 1)
                    {
                        testlevel = tup.tree;
                    }
                }
            }

            //add node at last step
            if (testlevel.TryGetValue(testSearch[testlimit - 1], out var res))
            {
                if (testSearch.Length <= StartsWithTreeDepth)
                {
                    foreach (var file in res.list)
                    {
                        foreach (var node in file.Value)
                        {
                            returns.Add(node);
                        }
                    }
                }
                else
                {
                    foreach (var file in res.list)
                    {
                        foreach (var node in file.Value)
                        {
                            if (node.Text[testlimit..].StartsWith(testSearch))
                            {
                                returns.Add(node);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < testcontainslimit; i++)
            {
                if (testcontains.TryGetValue(testSearch[i], out var tup))
                {
                    if (i < testcontainslimit - 1)
                    {
                        testcontains = tup.tree;
                    }
                }
            }

            if (testcontains.TryGetValue(testSearch[testcontainslimit - 1], out var containsres))
            {
                if (testSearch.Length <= ContainsTreeDepth)
                {
                    foreach (var file in containsres.list)
                    {
                        foreach (var node in file.Value)
                        {
                            returns.Add(node);
                        }
                    }
                }
                else
                {
                    foreach (var file in containsres.list)
                    {
                        foreach (var node in file.Value)
                        {
                            if (node.Text.Contains(testSearch[testcontainslimit..]))
                            {
                                returns.Add(node);
                            }
                        }
                    }
                }
            }

            end = DateTime.UtcNow;
            Debug.WriteLine($"TRIE: searched through {nodecounter} nodes for \"{testSearch}\" in {(end - start).Microseconds}us and found {returns.Count} results");

            returns.Clear();

            List<Node> allList = [];

            foreach (var store in stores.Values)
            {
                allList.AddRange(store.Nodes);
            }
            start = DateTime.UtcNow;

            for (int k = 0; k < allList.Count; k++)
            {
                Node? node = allList[k];
                var nodeText = node.Text;
                if (nodeText.StartsWith(testSearch))
                {
                    returns.Add(node);
                }
                else if (nodeText.Contains(testSearch))
                {
                    returns.Add(node);
                }
            }

            end = DateTime.UtcNow;
            Debug.WriteLine($"NAIVE: searched through {nodecounter} nodes for \"{testSearch}\" in {(end - start).Microseconds}us and found {returns.Count} results");

            Debugger.Break();

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

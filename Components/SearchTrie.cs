using CSC.Nodestuff;
using System.Diagnostics;
using static CSC.StoryItems.StoryEnums;

namespace CSC.Components
{
    internal static class SearchTrie
    {
        private const int StartsWithTreeDepth = 15;
        private const int ContainsTreeDepth = 10;
        private const StringComparison SearchCulture = StringComparison.InvariantCultureIgnoreCase;

        private static readonly Dictionary<string, int> FileToIndex = new(22);
        private static readonly SearchTreeLevel StartsWithTree = [];
        private static readonly SearchTreeLevel ContainsTree = [];
        private static int nodecounter = 0;
        private static bool BuiltTree = false;
        public static bool Initialized => BuiltTree;

        static SearchTrie()
        {
            List<string> files = [.. Enum.GetNames<Characters>().Cast<string>()];
            int counter = 0;
            foreach (string file in files)
            {
                FileToIndex.Add(file, counter++);
            }
        }

        public static void Reset()
        {
            StartsWithTree.Clear();
            ContainsTree.Clear();
            BuiltTree = false;
            nodecounter = 0;
        }

        public static async void Initialize(Dictionary<string, NodeStore> stores)
        {
            await Task.Factory.StartNew(() =>
            {
                var start = DateTime.UtcNow;

                Reset();

                foreach (var store in stores)
                {
                    if (store.Key == Main.NoCharacter)
                    {
                        continue;
                    }

                    List<Node> nodes = store.Value.Nodes;
                    nodecounter += nodes.Count;

                    //todo somehow parallelize?
                    for (int ni = 0; ni < nodes.Count; ni++)
                    {
                        //todo check whether to lower or not here
                        var nodeText = nodes[ni].Text.ToLower().AsSpan();
                        if (nodeText.Length > 0)
                        {
                            AddToTree(store, nodes[ni], nodeText);
                        }

                        nodeText = nodes[ni].Type.ToString().ToLower().AsSpan();
                        AddToTree(store, nodes[ni], nodeText);
                        //todo add else clause with adding to 0 length, or fix node.Text return to always return at least node type...
                    }
                }

                BuiltTree = true;

                var end = DateTime.UtcNow;
                Debug.WriteLine($"populated the search trees for {nodecounter} nodes with {SearchTrie.StartsWithTreeDepth} steps for StartsWith() and {SearchTrie.ContainsTreeDepth} steps for Contains() in {(end - start).TotalMilliseconds}ms");

                static void AddToTree(KeyValuePair<string, NodeStore> store, Node node, ReadOnlySpan<char> nodeText)
                {
                    //disable garbage collection for now
                    GC.TryStartNoGCRegion(251658240); //240MB

                    //startswith() lookup up to a length
                    var level = StartsWithTree;
                    int limit = Math.Min(nodeText.Length, SearchTrie.StartsWithTreeDepth);

                    for (int j = 0; j < nodeText.Length; j++)
                    {
                        //contains() lookup up to a certain string starting lenght
                        var container = ContainsTree;
                        int containerlimit = Math.Min(nodeText.Length - j, SearchTrie.ContainsTreeDepth);
                        int loopLimit = Math.Max(limit, containerlimit);
                        for (int i = 0; i < loopLimit; i++)
                        {
                            if (i < containerlimit)
                            {
                                if (!container.TryGetValue(nodeText[i + j], out var tup))
                                {
                                    tup = new();
                                    container.Add(nodeText[i + j], tup);
                                }

                                container = tup.tree;

                                //add node at each step
                                tup.list.Add(node);
                            }

                            if (j == 0 && i < limit)
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
                            }
                        }
                    }
                    //add node at last step
                    level[nodeText[limit - 1]].list.Add(node);

                    //reenable garbage collection to keep the GC happy and not get booted out of the small noGC region
                    GC.EndNoGCRegion();
                }
            });
        }

        public static HashSet<Node> Search(ReadOnlySpan<char> searchValue)
        {
            if (!Initialized)
            {
                return [];
            }

            HashSet<Node> results = [];

            if (searchValue.Length > 0)
            {
                //startswith search
                StartsWith(searchValue, results);

                //contains search
                Contains(searchValue, results);
            }

            return results;
        }

        private static void Contains(ReadOnlySpan<char> searchValue, HashSet<Node> results)
        {
            if (!Initialized)
            {
                return;
            }

            var testcontains = ContainsTree;
            int testcontainslimit = Math.Min(searchValue.Length, ContainsTreeDepth) - 1;
            //walk down the tree as far as we need to
            for (int i = 0; i < testcontainslimit; i++)
            {
                if (testcontains.TryGetValue(searchValue[i], out var tup))
                {
                    testcontains = tup.tree;
                }
                else
                {
                    break;
                }
            }

            //check last tree level's results
            if (testcontains.TryGetValue(searchValue[testcontainslimit], out var containsres))
            {
                if (searchValue.Length <= ContainsTreeDepth)
                {
                    foreach (var node in containsres.list)
                    {
                        results.Add(node);
                    }
                }
                else
                {
                    foreach (var node in containsres.list)
                    {
                        if (node.Text.AsSpan().Contains(searchValue, SearchCulture))
                        {
                            results.Add(node);
                        }
                    }
                }
            }
        }

        private static void StartsWith(ReadOnlySpan<char> searchValue, HashSet<Node> results)
        {
            if (!Initialized)
            {
                return;
            }
            var testlevel = StartsWithTree;
            int testlimit = Math.Min(searchValue.Length, StartsWithTreeDepth) - 1;
            for (int i = 0; i < testlimit; i++)
            {
                if (testlevel.TryGetValue(searchValue[i], out var tup))
                {
                    testlevel = tup.tree;
                }
                else
                {
                    break;
                }
            }

            //add node at last step
            if (testlevel.TryGetValue(searchValue[testlimit], out var res))
            {
                if (searchValue.Length <= StartsWithTreeDepth)
                {
                    foreach (var node in res.list)
                    {
                        results.Add(node);
                    }
                }
                else
                {
                    foreach (var node in res.list)
                    {
                        if (node.Text.AsSpan()[testlimit..].StartsWith(searchValue, SearchCulture))
                        {
                            results.Add(node);
                        }
                    }
                }
            }
        }
    }

    internal sealed class FileAssignedNode : List<Node> { }

    internal record struct SearchTreeNode
    {
        public FileAssignedNode list;
        public SearchTreeLevel tree;

        public SearchTreeNode()
        {
            list = [];
            tree = [];
        }
    }

    internal sealed class SearchTreeLevel : Dictionary<char, SearchTreeNode>
    {
        //internal SearchTreeLevel() : base(7) { }
    }

}
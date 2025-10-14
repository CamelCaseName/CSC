using CSC.Nodestuff;
using System;
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
        private static bool doFuzzy = true;
        private static readonly Range[] ranges = new Range[20];

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
                        //add both case sensitive and not here, can seperate during search later
                        var nodeText = nodes[ni].Text.ToLower().AsSpan();
                        if (nodeText.Length > 0)
                        {
                            AddToTree(store, nodes[ni], nodeText);
                        }

                        nodeText = nodes[ni].Text.AsSpan();
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
                //cannot be with case! mustbe case insensetive if fuzzing
                if (!doFuzzy)
                {
                    SearchImpl(searchValue, ref results);
                    return results;
                }
                else
                {
                    //split value fuzzing
                    int count = searchValue.Split(ranges, ' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    for (int x = 0; x < count; x++)
                    {
                        SearchImpl(searchValue[ranges[x]], ref results);

                        var range = searchValue[ranges[x]];

                        //character duplication typo fuzzing
                        char[] chars = range.ToArray();
                        int length = range.Length;
                        for (int i = 1; i < length; i++)
                        {
                            range.CopyTo(chars);

                            for (int j = i; j < length - 1; j++)
                            {
                                chars[j] = chars[j + 1];
                            }
                            chars[^1] = '\0';

                            SearchImpl(chars.AsSpan()[..^1], ref results);
                        }

                        //typo fuzzing
                        for (int i = 0; i < length; i++)
                        {
                            foreach (char replacement in GetTypoLetters(range[i]))
                            {
                                range.CopyTo(chars);

                                chars[i] = replacement;

                                SearchImpl(chars, ref results);
                            }
                        }
                    }
                    return results;
                }
            }
            return results;

            static void SearchImpl(ReadOnlySpan<char> searchValue, ref HashSet<Node> results)
            {
                //startswith search
                StartsWith(searchValue, ref results);

                //contains search
                Contains(searchValue, ref results);
            }
        }

        private static IEnumerable<char> GetTypoLetters(char v)
        {
            return v switch
            {
                'q' => ['a', 'w'],
                'w' => ['q', 'w', 'e'],
                'e' => ['s', 'w', 'r', 'd'],
                'r' => ['e', 'f', 't'],
                't' => ['r', 'g', 'y'],
                'y' => ['t', 'h', 'u'],
                'u' => ['y', 'j', 'i'],
                'i' => ['u', 'k', 'o'],
                'o' => ['i', 'l', 'p'],
                'p' => ['o', 'l'],
                'a' => ['q', 's', 'y'],
                's' => ['w', 'a', 'x', 'd'],
                'd' => ['s', 'e', 'c', 'f', 'x'],
                'f' => ['d', 'r', 'c', 'g', 'c'],
                'g' => ['f', 't', 'b', 'h', 'v'],
                'h' => ['g', 'z', 'n', 'j', 'b'],
                'j' => ['h', 'u', 'm', 'n', 'k'],
                'k' => ['j', 'm', 'i', 'l'],
                'l' => ['o', 'k'],
                'z' => [' ', 'a', 's', 'x'],
                'x' => [' ', 'z', 's', 'd', 'c'],
                'c' => [' ', 'x', 'd', 'f', 'v'],
                'v' => [' ', 'c', 'f', 'g', 'b'],
                'b' => [' ', 'v', 'h', 'g', 'n'],
                'n' => [' ', 'b', 'h', 'j', 'm'],
                'm' => [' ', 'n', 'j', 'k'],
                _ => [],
            };
        }

        private static void Contains(ReadOnlySpan<char> searchValue, ref HashSet<Node> results)
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
                    return;
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

        private static void StartsWith(ReadOnlySpan<char> searchValue, ref HashSet<Node> results)
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
                    return;
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
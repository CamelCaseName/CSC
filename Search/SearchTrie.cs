using CSC.Nodestuff;
using System.Diagnostics;
using static CSC.StoryItems.StoryEnums;

namespace CSC.Search
{
    internal static class SearchTrie
    {
        private const int ContainsTreeDepth = 9;
        private const StringComparison SearchCulture = StringComparison.InvariantCultureIgnoreCase;

        private static readonly Dictionary<string, int> FileToIndex = new(22);
        private static readonly SearchTreeLevel ContainsTree = [];
        private static int nodecounter = 0;
        private static bool BuiltTree = false;
        private static readonly Range[] ranges = new Range[20];
        private static readonly List<Node> contains = [];

        public static Action? OnSearchDataChange;

        public static bool Initialized => BuiltTree;

        public static int NodeCount => nodecounter;

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
            ContainsTree.Clear();
            BuiltTree = false;
            nodecounter = 0;
        }

        public static async void Initialize(Dictionary<string, NodeStore> stores)
        {
            Reset();

            foreach (var store in stores)
            {
                if (store.Key == Main.NoCharacter)
                {
                    continue;
                }

                nodecounter += store.Value.Nodes.Count;
            }

            await Task.Factory.StartNew(() =>
            {
                var start = DateTime.UtcNow;

                foreach (var store in stores)
                {
                    if (store.Key == Main.NoCharacter)
                    {
                        continue;
                    }

                    var nodes = store.Value.Nodes;

                    //todo somehow parallelize?
                    for (int ni = 0; ni < nodes.Count; ni++)
                    {
                        AddNode(nodes[ni]);
                    }
                }

                BuiltTree = true;

                var end = DateTime.UtcNow;
                Debug.WriteLine($"populated the search trees for {nodecounter} nodes with and {ContainsTreeDepth} steps for Contains() in {(end - start).TotalMilliseconds}ms");

                Main.SetSearchWindowTitle("Search:");
            });
        }

        public static void RemoveNode(Node node)
        {
            if (!contains.Contains(node))
            {
                return;
            }
            contains.Remove(node);

            var nodeText = node.Text.ToLower().AsSpan();
            if (nodeText.Length > 0)
            {
                RemoveFromTree(node, nodeText);
            }

            nodeText = node.Text.AsSpan();
            if (nodeText.Length > 0)
            {
                RemoveFromTree(node, nodeText);
            }

            nodeText = node.Type.ToString().ToLower().AsSpan();
            RemoveFromTree(node, nodeText);
        }

        private static void RemoveFromTree(Node node, ReadOnlySpan<char> nodeText)
        {
            for (int j = 0; j < nodeText.Length; j++)
            {
                //contains() lookup up to a certain string starting lenght
                var container = ContainsTree;
                int containerlimit = Math.Min(nodeText.Length - j, ContainsTreeDepth);
                for (int i = 0; i < containerlimit; i++)
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
                        tup.list.Remove(node);
                    }
                }
            }
        }

        public static void AddNode(Node node)
        {
            if (!contains.Contains(node))
            {
                contains.Add(node);
            }
            else
            {
                return;
            }

            //add both case sensitive and not here, can seperate during search later
            var nodeText = node.Text.ToLower().AsSpan();
            if (nodeText.Length > 0)
            {
                AddToTree(node, nodeText);
            }

            nodeText = node.ID.ToLower().AsSpan();
            if (nodeText.Length > 0)
            {
                AddToTree(node, nodeText);
            }

            //so we can do searches like "amy dialogue hello" and we can then check the search results and remove all that doesnt fit
            nodeText = node.OrigFileName.ToLower().AsSpan();
            AddToTree(node, nodeText);

            if (Initialized)
            {
                OnSearchDataChange?.Invoke();
            }
        }

        private static void AddToTree(Node node, ReadOnlySpan<char> nodeText)
        {
            //disable garbage collection for now
            GC.TryStartNoGCRegion(251658240); //240MB

            for (int j = 0; j < nodeText.Length; j++)
            {
                //contains() lookup up to a certain string starting lenght
                var container = ContainsTree;
                int containerlimit = Math.Min(nodeText.Length - j, ContainsTreeDepth);
                for (int i = 0; i < containerlimit; i++)
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
                }
            }

            //reenable garbage collection to keep the GC happy and not get booted out of the small noGC region
            GC.EndNoGCRegion();
        }

        public static HashSet<Node> Search(ReadOnlySpan<char> searchValue, SearchSettings settings = SearchSettings.None)
        {
            if (!Initialized)
            {
                return [];
            }

            HashSet<Node> results = [];

            if (searchValue.Length > 0)
            {
                //cannot be with case! mustbe case insensetive if fuzzing
                if (settings.HasFlag(SearchSettings.OneWord))
                {
                    Contains(searchValue, ref results, settings);

                    if (!settings.HasFlag(SearchSettings.Strict))
                    {
                        FuzzySearch(searchValue, ref results, settings);
                    }

                    return results;
                }
                else
                {
                    //split value fuzzing
                    int count = searchValue.Split(ranges, ' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    for (int x = 0; x < count; x++)
                    {
                        Contains(searchValue[ranges[x]], ref results, settings);

                        var range = searchValue[ranges[x]];

                        if (!settings.HasFlag(SearchSettings.Strict))
                        {
                            FuzzySearch(range, ref results, settings);
                        }
                    }
                    return results;
                }
            }
            return results;
        }

        private static void FuzzySearch(ReadOnlySpan<char> range, ref HashSet<Node> results, SearchSettings settings)
        {
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

                Contains(chars.AsSpan()[..^1], ref results, settings);
            }

            //typo fuzzing
            for (int i = 0; i < length; i++)
            {
                foreach (char replacement in GetTypoLetters(range[i]))
                {
                    range.CopyTo(chars);

                    chars[i] = replacement;

                    Contains(chars, ref results, settings);
                }
            }
        }

        private static IEnumerable<char> GetTypoLetters(char v)
        {
            return v switch
            {
                'q' => ['a', 'w'],
                'w' => ['q', 'w', 'e', 'a'],
                'e' => ['s', 'w', 'r', 'd'],
                'r' => ['e', 'f', 't', 'd'],
                't' => ['r', 'g', 'y', 'f'],
                'y' => ['t', 'h', 'u', 'g'],
                'u' => ['y', 'j', 'i', 'h'],
                'i' => ['u', 'k', 'o', 'j'],
                'o' => ['i', 'l', 'p', 'k'],
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

        private static void Contains(ReadOnlySpan<char> searchValue, ref HashSet<Node> results, SearchSettings settings)
        {
            if (!Initialized)
            {
                return;
            }

            var searchlowered = new char[searchValue.Length];
            searchValue.ToLower(searchlowered, System.Globalization.CultureInfo.InvariantCulture);

            var testcontains = ContainsTree;
            int testcontainslimit = Math.Min(searchlowered.Length, ContainsTreeDepth) - 1;
            //walk down the tree as far as we need to
            for (int i = 0; i < testcontainslimit; i++)
            {
                if (testcontains.TryGetValue(searchlowered[i], out var tup))
                {
                    testcontains = tup.tree;
                }
                else
                {
                    return;
                }
            }

            //check last tree level's results
            if (testcontains.TryGetValue(searchlowered[testcontainslimit], out var containsres))
            {
                if (searchlowered.Length <= ContainsTreeDepth)
                {
                    foreach (var node in containsres.list)
                    {
                        if (settings.HasFlag(SearchSettings.CaseSensitive))
                        {
                            if (node.Text.AsSpan().Contains(searchValue, StringComparison.InvariantCulture))
                            {
                                results.Add(node);
                            }
                            else if (node.ID.AsSpan().Contains(searchValue, StringComparison.InvariantCulture))
                            {
                                results.Add(node);
                            }
                        }
                        else
                        {
                            results.Add(node);
                        }
                    }
                }
                else
                {
                    foreach (var node in containsres.list)
                    {
                        if (settings.HasFlag(SearchSettings.CaseSensitive))
                        {
                            if (node.Text.AsSpan().Contains(searchValue, StringComparison.InvariantCulture))
                            {
                                results.Add(node);
                            }
                            else if (node.ID.AsSpan().Contains(searchValue, StringComparison.InvariantCulture))
                            {
                                results.Add(node);
                            }
                        }
                        else
                        {
                            if (node.Text.AsSpan().Contains(searchlowered, SearchCulture))
                            {
                                results.Add(node);
                            }
                            else if (node.ID.AsSpan().Contains(searchlowered, SearchCulture))
                            {
                                results.Add(node);
                            }
                        }
                    }
                }
            }
        }
    }

    [Flags]
    public enum SearchSettings
    {
        //none
        None = 0,
        //case sensitive, case insensentive by default
        CaseSensitive = 1,
        //treat whole query as one word, dont split on space
        OneWord = 2,
        //dont do any fuzzing
        Strict = 4,
        //constrain search to the currently selected file
        SingleFile = 8,
        //treat first word as file
        FirstWordFile = 16,
        //search content only
        NodeContentOnly = 32,
        //treat first word as node type
        FirstWordNodeType = 64,
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

    internal sealed class SearchTreeLevel : Dictionary<char, SearchTreeNode> { }
}
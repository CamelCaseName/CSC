using CSC.Nodestuff;
using CSC.Search;
using System.Diagnostics;
using System.Media;
using System.Threading.Tasks;
using static CSC.StoryItems.StoryEnums;

namespace CSC.Components
{
    public partial class SearchDialog : Form
    {
        private SearchSettings settings = SearchSettings.None;
        private bool NodeTypeSet = false;
        private bool ignoreModifierCheckState = false;

        public SearchDialog(Dictionary<string, NodeStore> stores)
        {
            InitializeComponent();

            if (!SearchTrie.Initialized)
            {
                Cursor = Cursors.WaitCursor;
                SearchTrie.Initialize(stores);
                Text = $"Building Search index, estimated wait time is {SearchTrie.NodeCount / 5000}s";
            }

            SearchTrie.OnSearchDataChange += SearchImpl;

            modifiers.Items.Add(SearchSettings.None.ToString());
            modifiers.Items.Add(SearchSettings.OneWord.ToString());
            modifiers.Items.Add(SearchSettings.Strict.ToString());
            modifiers.Items.Add(SearchSettings.SingleFile.ToString());
            modifiers.Items.Add(SearchSettings.FirstWordFile.ToString());
            modifiers.Items.Add(SearchSettings.NodeContentOnly.ToString());
            modifiers.SelectedIndex = 0;

            nodetype.Items.Add("Any");
            nodetype.Items.AddRange(Enum.GetNames<NodeType>());
            nodetype.SelectedIndex = 0;
        }

        private void OnFormClosing(object? sender, FormClosingEventArgs? e)
        {
            SearchTrie.OnSearchDataChange -= SearchImpl;
        }

        private void Nodetype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (nodetype.SelectedIndex > 0)
            {
                NodeTypeSet = true;
            }
            else
            {
                NodeTypeSet = false;
            }
            SearchImpl();
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
            resultsTree.AfterSelect -= Results_AfterSelect!;

            var selectedBefore = resultsTree.SelectedNode;

            var results = SearchTrie.Search(searchterm.Text, settings);

            List<Node> toRemoveResults = [.. results];

            string character = Main.SelectedCharacter;
            if (settings.HasFlag(SearchSettings.FirstWordFile))
            {
                if (settings.HasFlag(SearchSettings.CaseSensitive))
                {
                    foreach (var _char in Node.AllowedFileNames)
                    {
                        if (searchterm.Text.StartsWith(_char, StringComparison.InvariantCulture))
                        {
                            character = _char;
                        }
                    }
                }
                else
                {
                    foreach (var _char in Node.AllowedFileNames)
                    {
                        if (searchterm.Text.StartsWith(_char, StringComparison.InvariantCultureIgnoreCase))
                        {
                            character = _char.ToLower(System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                }
            }

            var nType = NodeType.Null;
            if (NodeTypeSet)
            {
                nType = Enum.Parse<NodeType>(nodetype.SelectedItem!.ToString()!);
            }

            //string filters
            foreach (var item in toRemoveResults)
            {
                if (NodeTypeSet)
                {
                    if (item.Type != nType)
                    {
                        results.Remove(item);
                    }
                }
                if (settings.HasFlag(SearchSettings.SingleFile))
                {
                    if (settings.HasFlag(SearchSettings.CaseSensitive))
                    {
                        if (item.OrigFileName != character && !item.DupedFileNames.Contains(character))
                        {
                            results.Remove(item);
                        }
                    }
                    else
                    {
                        if (!item.OrigFileName.Equals(character, StringComparison.CurrentCultureIgnoreCase) && !item.DupedFileNames.Contains(character, StringComparer.InvariantCultureIgnoreCase))
                        {
                            results.Remove(item);
                        }
                    }
                }
                if (settings.HasFlag(SearchSettings.NodeContentOnly))
                {
                    if (settings.HasFlag(SearchSettings.CaseSensitive))
                    {
                        if (!item.Text.AsSpan().Contains(character, StringComparison.InvariantCulture))
                        {
                            results.Remove(item);
                        }
                    }
                    else
                    {
                        if (!item.Text.ToLower().AsSpan().Contains(character, StringComparison.InvariantCultureIgnoreCase))
                        {
                            results.Remove(item);
                        }
                    }
                }
            }

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

            if (selectedBefore is not null)
            {
                foreach (TreeNode node in resultsTree.Nodes)
                {
                    foreach (TreeNode child in node.Nodes)
                    {
                        if (child.Text == selectedBefore.Text)
                        {
                            resultsTree.SelectedNode = child;
                            resultsTree.AfterSelect += Results_AfterSelect!;
                            return;
                        }
                    }
                    if (node.Text == selectedBefore.Text)
                    {
                        resultsTree.SelectedNode = node;
                        resultsTree.AfterSelect += Results_AfterSelect!;
                        return;
                    }
                }
            }

            resultsTree.AfterSelect += Results_AfterSelect!;
            return;
        }

        private void Modifiers_SelectedIndexChanged(object sender, EventArgs e)
        {
            ignoreModifierCheckState = true;
            switch (modifiers.SelectedIndex)
            {
                case 0:
                    modifierCheck.Checked = false;
                    modifierCheck.Visible = false;
                    settings &= ~SearchSettings.OneWord;
                    settings &= ~SearchSettings.Strict;
                    settings &= ~SearchSettings.SingleFile;
                    settings &= ~SearchSettings.FirstWordFile;
                    settings &= ~SearchSettings.NodeContentOnly;
                    Filterlabel.Text = "Modifiers: " + settings.ToString();
                    SearchImpl();
                    break;
                case 1:
                    modifierCheck.Checked = settings.HasFlag(SearchSettings.OneWord);
                    break;
                case 2:
                    modifierCheck.Checked = settings.HasFlag(SearchSettings.Strict);
                    break;
                case 3:
                    modifierCheck.Checked = settings.HasFlag(SearchSettings.SingleFile);
                    break;
                case 4:
                    modifierCheck.Checked = settings.HasFlag(SearchSettings.FirstWordFile);
                    break;
                case 5:
                    modifierCheck.Checked = settings.HasFlag(SearchSettings.NodeContentOnly);
                    break;
            }
            if (modifiers.SelectedIndex != 0)
            {
                modifierCheck.Visible = true;
                ignoreModifierCheckState = false;
            }
        }

        private void Casesensitivity_CheckedChanged(object sender, EventArgs e)
        {
            if (casesensitivity.Checked)
            {
                settings |= SearchSettings.CaseSensitive;
            }
            else
            {
                settings &= ~SearchSettings.CaseSensitive;
            }
            SearchImpl();
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

        private void ModifierCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (ignoreModifierCheckState)
            {
                return;
            }

            if (modifierCheck.Checked)
            {
                switch (modifiers.SelectedIndex)
                {
                    case 1:
                        settings |= SearchSettings.OneWord;
                        break;
                    case 2:
                        settings |= SearchSettings.Strict;
                        break;
                    case 3:
                        settings |= SearchSettings.SingleFile;
                        break;
                    case 4:
                        settings |= SearchSettings.FirstWordFile;
                        break;
                    case 5:
                        settings |= SearchSettings.NodeContentOnly;
                        break;
                }
            }
            else
            {
                switch (modifiers.SelectedIndex)
                {
                    case 1:
                        settings &= ~SearchSettings.OneWord;
                        break;
                    case 2:
                        settings &= ~SearchSettings.Strict;
                        break;
                    case 3:
                        settings &= ~SearchSettings.SingleFile;
                        break;
                    case 4:
                        settings &= ~SearchSettings.FirstWordFile;
                        break;
                    case 5:
                        settings &= ~SearchSettings.NodeContentOnly;
                        break;
                }
            }

            //update filter display
            Filterlabel.Text = "Modifiers: " + settings.ToString();
            SearchImpl();
        }
    }
}

using CSC.Nodestuff;

namespace CSC.Components
{
    public partial class NodeGraphFilter : Form
    {
        static public readonly NodeGraphFilter Instance = new();

        public NodeGraphFilter()
        {
            InitializeComponent();

            Visible = false;

            foreach (var type in Enum.GetNames<NodeType>())
            {
                typelist.Items.Add(type);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            //we dont want this instance to be closed...
            e.Cancel = true;
            Visible = false;
        }

        private void TypeListCheckedChanged(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                Main.HiddenTypes.Add(Enum.Parse<NodeType>((string)typelist.Items[e.Index]));
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                Main.HiddenTypes.Remove(Enum.Parse<NodeType>((string)typelist.Items[e.Index]));
            }

            Main.ForceRedrawGraph();
        }
    }
}

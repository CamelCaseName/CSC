using CSC.Components;

namespace CSC
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            TreeNode treeNode3 = new TreeNode("Characters");
            TreeNode treeNode4 = new TreeNode("Story Root", new TreeNode[] { treeNode3 });
            Menu = new ToolStrip();
            StartButton = new ToolStripButton();
            ResetButton = new ToolStripButton();
            Add = new ToolStripButton();
            AddChild = new ToolStripButton();
            AddParent = new ToolStripButton();
            OpenButton = new ToolStripButton();
            StoryTree = new TreeView();
            HierarchyAndRest = new SplitContainer();
            GraphAndProperties = new SplitContainer();
            Graph = new DoubleBufferedPanel();
            NodeSpawnBox = new ComboBox();
            PropertyInspector = new TableLayoutPanel();
            Menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)HierarchyAndRest).BeginInit();
            HierarchyAndRest.Panel1.SuspendLayout();
            HierarchyAndRest.Panel2.SuspendLayout();
            HierarchyAndRest.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)GraphAndProperties).BeginInit();
            GraphAndProperties.Panel1.SuspendLayout();
            GraphAndProperties.Panel2.SuspendLayout();
            GraphAndProperties.SuspendLayout();
            Graph.SuspendLayout();
            SuspendLayout();
            // 
            // Menu
            // 
            Menu.BackColor = Color.DimGray;
            Menu.Items.AddRange(new ToolStripItem[] { StartButton, ResetButton, Add, AddChild, AddParent, OpenButton });
            Menu.Location = new Point(0, 0);
            Menu.Name = "Menu";
            Menu.Size = new Size(1431, 25);
            Menu.TabIndex = 0;
            Menu.Text = "toolStrip1";
            // 
            // StartButton
            // 
            StartButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            StartButton.ForeColor = Color.FromArgb(224, 224, 224);
            StartButton.Image = (Image)resources.GetObject("StartButton.Image");
            StartButton.ImageTransparentColor = Color.Magenta;
            StartButton.Name = "StartButton";
            StartButton.Size = new Size(35, 22);
            StartButton.Text = "Start";
            StartButton.Click += Start_Click;
            // 
            // ResetButton
            // 
            ResetButton.BackColor = Color.DimGray;
            ResetButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            ResetButton.Enabled = false;
            ResetButton.ForeColor = Color.FromArgb(224, 224, 224);
            ResetButton.Image = (Image)resources.GetObject("ResetButton.Image");
            ResetButton.ImageTransparentColor = Color.Magenta;
            ResetButton.Name = "ResetButton";
            ResetButton.Size = new Size(39, 22);
            ResetButton.Text = "Reset";
            ResetButton.Click += ResetButton_Click;
            // 
            // Add
            // 
            Add.DisplayStyle = ToolStripItemDisplayStyle.Text;
            Add.Enabled = false;
            Add.ForeColor = Color.FromArgb(224, 224, 224);
            Add.Image = (Image)resources.GetObject("Add.Image");
            Add.ImageTransparentColor = Color.Magenta;
            Add.Name = "Add";
            Add.Size = new Size(33, 22);
            Add.Text = "Add";
            Add.Click += Add_Click;
            // 
            // AddChild
            // 
            AddChild.DisplayStyle = ToolStripItemDisplayStyle.Text;
            AddChild.Enabled = false;
            AddChild.ForeColor = Color.FromArgb(224, 224, 224);
            AddChild.Image = (Image)resources.GetObject("AddChild.Image");
            AddChild.ImageTransparentColor = Color.Magenta;
            AddChild.Name = "AddChild";
            AddChild.Size = new Size(64, 22);
            AddChild.Text = "Add Child";
            AddChild.Click += AddChild_Click;
            // 
            // AddParent
            // 
            AddParent.DisplayStyle = ToolStripItemDisplayStyle.Text;
            AddParent.Enabled = false;
            AddParent.ForeColor = Color.FromArgb(224, 224, 224);
            AddParent.Image = (Image)resources.GetObject("AddParent.Image");
            AddParent.ImageTransparentColor = Color.Magenta;
            AddParent.Name = "AddParent";
            AddParent.Size = new Size(70, 22);
            AddParent.Text = "Add Parent";
            AddParent.Click += AddParent_Click;
            // 
            // OpenButton
            // 
            OpenButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            OpenButton.ForeColor = Color.FromArgb(224, 224, 224);
            OpenButton.Image = (Image)resources.GetObject("OpenButton.Image");
            OpenButton.ImageTransparentColor = Color.Magenta;
            OpenButton.Name = "OpenButton";
            OpenButton.Size = new Size(40, 22);
            OpenButton.Text = "Open";
            OpenButton.Click += OpenButton_Click;
            // 
            // StoryTree
            // 
            StoryTree.BackColor = Color.FromArgb(64, 64, 64);
            StoryTree.Dock = DockStyle.Fill;
            StoryTree.ForeColor = Color.White;
            StoryTree.Location = new Point(0, 0);
            StoryTree.Name = "StoryTree";
            treeNode3.Name = "Characters";
            treeNode3.Text = "Characters";
            treeNode3.ToolTipText = "You'll find all your Characters here";
            treeNode4.Checked = true;
            treeNode4.Name = "Story Name";
            treeNode4.Text = "Story Root";
            treeNode4.ToolTipText = "The Story itself and the Characters are in here";
            StoryTree.Nodes.AddRange(new TreeNode[] { treeNode4 });
            StoryTree.ShowNodeToolTips = true;
            StoryTree.Size = new Size(235, 537);
            StoryTree.TabIndex = 1;
            StoryTree.AfterSelect += StoryTree_AfterSelect;
            // 
            // HierarchyAndRest
            // 
            HierarchyAndRest.Dock = DockStyle.Fill;
            HierarchyAndRest.Location = new Point(0, 25);
            HierarchyAndRest.Name = "HierarchyAndRest";
            // 
            // HierarchyAndRest.Panel1
            // 
            HierarchyAndRest.Panel1.Controls.Add(StoryTree);
            // 
            // HierarchyAndRest.Panel2
            // 
            HierarchyAndRest.Panel2.Controls.Add(GraphAndProperties);
            HierarchyAndRest.Size = new Size(1431, 537);
            HierarchyAndRest.SplitterDistance = 235;
            HierarchyAndRest.TabIndex = 2;
            // 
            // GraphAndProperties
            // 
            GraphAndProperties.Dock = DockStyle.Fill;
            GraphAndProperties.Location = new Point(0, 0);
            GraphAndProperties.Name = "GraphAndProperties";
            GraphAndProperties.Orientation = Orientation.Horizontal;
            // 
            // GraphAndProperties.Panel1
            // 
            GraphAndProperties.Panel1.Controls.Add(Graph);
            // 
            // GraphAndProperties.Panel2
            // 
            GraphAndProperties.Panel2.Controls.Add(PropertyInspector);
            GraphAndProperties.Size = new Size(1192, 537);
            GraphAndProperties.SplitterDistance = 384;
            GraphAndProperties.TabIndex = 0;
            // 
            // Graph
            // 
            Graph.Controls.Add(NodeSpawnBox);
            Graph.Dock = DockStyle.Fill;
            Graph.Location = new Point(0, 0);
            Graph.Name = "Graph";
            Graph.Size = new Size(1192, 384);
            Graph.TabIndex = 0;
            Graph.Paint += Main_Paint;
            Graph.MouseClick += HandleMouseEvents;
            Graph.MouseDoubleClick += HandleMouseEvents;
            Graph.MouseWheel += HandleMouseEvents;
            // 
            // NodeSpawnBox
            // 
            NodeSpawnBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            NodeSpawnBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            NodeSpawnBox.AutoCompleteSource = AutoCompleteSource.ListItems;
            NodeSpawnBox.Enabled = false;
            NodeSpawnBox.FormattingEnabled = true;
            NodeSpawnBox.Location = new Point(264, 62);
            NodeSpawnBox.MinimumSize = new Size(200, 0);
            NodeSpawnBox.Name = "NodeSpawnBox";
            NodeSpawnBox.Size = new Size(674, 23);
            NodeSpawnBox.TabIndex = 0;
            NodeSpawnBox.Visible = false;
            NodeSpawnBox.SelectionChangeCommitted += SpawnNodeFromSpaceSpawner;
            NodeSpawnBox.SelectedValueChanged += SpawnNodeFromSpaceSpawner;
            NodeSpawnBox.SelectedIndexChanged += SpawnNodeFromSpaceSpawner;
            // 
            // PropertyInspector
            // 
            PropertyInspector.BackColor = Color.FromArgb(64, 64, 64);
            PropertyInspector.ColumnCount = 3;
            PropertyInspector.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            PropertyInspector.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 142F));
            PropertyInspector.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            PropertyInspector.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            PropertyInspector.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            PropertyInspector.Dock = DockStyle.Fill;
            PropertyInspector.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
            PropertyInspector.Location = new Point(0, 0);
            PropertyInspector.Name = "PropertyInspector";
            PropertyInspector.RowCount = 2;
            PropertyInspector.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            PropertyInspector.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            PropertyInspector.Size = new Size(1192, 149);
            PropertyInspector.TabIndex = 0;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(40, 40, 40);
            ClientSize = new Size(1431, 562);
            Controls.Add(HierarchyAndRest);
            Controls.Add(Menu);
            DoubleBuffered = true;
            KeyPreview = true;
            MinimumSize = new Size(400, 200);
            Name = "Main";
            ShowIcon = false;
            Text = "Custom Custom Story Creator";
            KeyDown += HandleKeyBoard;
            KeyUp += HandleKeyBoard;
            Menu.ResumeLayout(false);
            Menu.PerformLayout();
            HierarchyAndRest.Panel1.ResumeLayout(false);
            HierarchyAndRest.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)HierarchyAndRest).EndInit();
            HierarchyAndRest.ResumeLayout(false);
            GraphAndProperties.Panel1.ResumeLayout(false);
            GraphAndProperties.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)GraphAndProperties).EndInit();
            GraphAndProperties.ResumeLayout(false);
            Graph.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip Menu;
        private ToolStripButton StartButton;
        private ToolStripButton Add;
        private ToolStripButton AddChild;
        private ToolStripButton AddParent;
        private ToolStripButton ResetButton;
        private ToolStripButton OpenButton;
        private TreeView StoryTree;
        private SplitContainer HierarchyAndRest;
        private SplitContainer GraphAndProperties;
        private DoubleBufferedPanel Graph;
        private TableLayoutPanel PropertyInspector;
        private ComboBox NodeSpawnBox;
    }
}

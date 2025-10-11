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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            TreeNode treeNode1 = new TreeNode("Characters");
            TreeNode treeNode2 = new TreeNode("Story Root", new TreeNode[] { treeNode1 });
            Menu = new ToolStrip();
            OpenButton = new ToolStripButton();
            SaveButton = new ToolStripButton();
            NewStory = new ToolStripButton();
            Add = new ToolStripButton();
            cursorPos = new ToolStripLabel();
            StoryTree = new TreeView();
            HierarchyAndRest = new SplitContainer();
            GraphAndProperties = new SplitContainer();
            Graph = new DoubleBufferedPanel();
            NodeSpawnBox = new ComboBox();
            PropertyInspector = new TableLayoutPanel();
            NodeContext = new ContextMenuStrip(components);
            PullChildsMenu = new ToolStripMenuItem();
            PullParentsMenu = new ToolStripMenuItem();
            SortConnectedMenu = new ToolStripMenuItem();
            SortSelectedMenu = new ToolStripMenuItem();
            SortSelectedConnectedMenu = new ToolStripMenuItem();
            Seperator1 = new ToolStripSeparator();
            toolStripButton1 = new ToolStripButton();
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
            Menu.BackColor = Color.FromArgb(50, 50, 50);
            Menu.Items.AddRange(new ToolStripItem[] { OpenButton, SaveButton, NewStory, Add, cursorPos, toolStripButton1 });
            Menu.Location = new Point(0, 0);
            Menu.Name = "Menu";
            Menu.Size = new Size(1431, 25);
            Menu.TabIndex = 0;
            Menu.Text = "toolStrip1";
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
            // SaveButton
            // 
            SaveButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            SaveButton.ForeColor = Color.FromArgb(224, 224, 224);
            SaveButton.Image = (Image)resources.GetObject("SaveButton.Image");
            SaveButton.ImageTransparentColor = Color.Magenta;
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(50, 22);
            SaveButton.Text = "Save all";
            SaveButton.Click += Save_Click;
            // 
            // NewStory
            // 
            NewStory.DisplayStyle = ToolStripItemDisplayStyle.Text;
            NewStory.ForeColor = Color.FromArgb(224, 224, 224);
            NewStory.Image = (Image)resources.GetObject("NewStory.Image");
            NewStory.ImageTransparentColor = Color.Magenta;
            NewStory.Name = "NewStory";
            NewStory.Size = new Size(65, 22);
            NewStory.Text = "New Story";
            NewStory.Click += NewStory_Click;
            // 
            // Add
            // 
            Add.DisplayStyle = ToolStripItemDisplayStyle.Text;
            Add.ForeColor = Color.FromArgb(224, 224, 224);
            Add.Image = (Image)resources.GetObject("Add.Image");
            Add.ImageTransparentColor = Color.Magenta;
            Add.Name = "Add";
            Add.Size = new Size(63, 22);
            Add.Text = "Add Story";
            Add.Click += Add_Click;
            // 
            // cursorPos
            // 
            cursorPos.Alignment = ToolStripItemAlignment.Right;
            cursorPos.ForeColor = Color.White;
            cursorPos.Name = "cursorPos";
            cursorPos.Size = new Size(59, 22);
            cursorPos.Text = "cursorPos";
            // 
            // StoryTree
            // 
            StoryTree.BackColor = Color.FromArgb(50, 50, 50);
            StoryTree.Dock = DockStyle.Fill;
            StoryTree.ForeColor = Color.White;
            StoryTree.FullRowSelect = true;
            StoryTree.HideSelection = false;
            StoryTree.Location = new Point(0, 0);
            StoryTree.Name = "StoryTree";
            treeNode1.Name = "Characters";
            treeNode1.Text = "Characters";
            treeNode1.ToolTipText = "You'll find all your Characters here";
            treeNode2.Checked = true;
            treeNode2.Name = "Story Name";
            treeNode2.Text = "Story Root";
            treeNode2.ToolTipText = "The Story itself and the Characters are in here";
            StoryTree.Nodes.AddRange(new TreeNode[] { treeNode2 });
            StoryTree.ShowNodeToolTips = true;
            StoryTree.Size = new Size(186, 537);
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
            HierarchyAndRest.SplitterDistance = 186;
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
            GraphAndProperties.Size = new Size(1241, 537);
            GraphAndProperties.SplitterDistance = 417;
            GraphAndProperties.TabIndex = 0;
            // 
            // Graph
            // 
            Graph.Controls.Add(NodeSpawnBox);
            Graph.Dock = DockStyle.Fill;
            Graph.Location = new Point(0, 0);
            Graph.Name = "Graph";
            Graph.Size = new Size(1241, 417);
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
            NodeSpawnBox.BackColor = Color.FromArgb(64, 64, 64);
            NodeSpawnBox.Enabled = false;
            NodeSpawnBox.ForeColor = SystemColors.ScrollBar;
            NodeSpawnBox.FormattingEnabled = true;
            NodeSpawnBox.Location = new Point(264, 62);
            NodeSpawnBox.MinimumSize = new Size(200, 0);
            NodeSpawnBox.Name = "NodeSpawnBox";
            NodeSpawnBox.Size = new Size(723, 23);
            NodeSpawnBox.TabIndex = 0;
            NodeSpawnBox.Visible = false;
            NodeSpawnBox.SelectedIndexChanged += SpawnNodeFromSpaceSpawner;
            NodeSpawnBox.SelectionChangeCommitted += SpawnNodeFromSpaceSpawner;
            NodeSpawnBox.SelectedValueChanged += SpawnNodeFromSpaceSpawner;
            // 
            // PropertyInspector
            // 
            PropertyInspector.BackColor = Color.FromArgb(50, 50, 50);
            PropertyInspector.Dock = DockStyle.Fill;
            PropertyInspector.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
            PropertyInspector.Location = new Point(0, 0);
            PropertyInspector.Name = "PropertyInspector";
            PropertyInspector.RowCount = 2;
            PropertyInspector.RowStyles.Add(new RowStyle());
            PropertyInspector.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            PropertyInspector.Size = new Size(1241, 116);
            PropertyInspector.TabIndex = 0;
            // 
            // NodeContext
            // 
            NodeContext.BackColor = Color.FromArgb(64, 64, 64);
            NodeContext.Name = "contextMenuStrip1";
            NodeContext.ShowImageMargin = false;
            NodeContext.Size = new Size(36, 4);
            NodeContext.Text = "Spawn new Node:";
            // 
            // PullChildsMenu
            // 
            PullChildsMenu.ForeColor = Color.LightGray;
            PullChildsMenu.Name = "PullChildsMenu";
            PullChildsMenu.Size = new Size(155, 22);
            PullChildsMenu.Text = "Pull Childs close";
            PullChildsMenu.Click += PullChildsClose;
            // 
            // PullParentsMenu
            // 
            PullParentsMenu.ForeColor = Color.LightGray;
            PullParentsMenu.Name = "PullParentsMenu";
            PullParentsMenu.Size = new Size(155, 22);
            PullParentsMenu.Text = "Pull Parents close";
            PullParentsMenu.Click += PullParentsClose;
            // 
            // SortConnectedMenu
            // 
            SortConnectedMenu.ForeColor = Color.LightGray;
            SortConnectedMenu.Name = "SortConnectedMenu";
            SortConnectedMenu.Size = new Size(155, 22);
            SortConnectedMenu.Text = "Sort Connected nodes";
            SortConnectedMenu.Click += SortConnected;
            // 
            // SortSelectedMenu
            // 
            SortSelectedMenu.ForeColor = Color.LightGray;
            SortSelectedMenu.Name = "SortSelectedMenu";
            SortSelectedMenu.Size = new Size(155, 22);
            SortSelectedMenu.Text = "Sort Selected nodes";
            SortSelectedMenu.Click += SortSelected;
            // 
            // SortSelectedConnectedMenu
            // 
            SortSelectedConnectedMenu.ForeColor = Color.LightGray;
            SortSelectedConnectedMenu.Name = "SortSelectedConnectedMenu";
            SortSelectedConnectedMenu.Size = new Size(155, 22);
            SortSelectedConnectedMenu.Text = "Sort nodes connected to the selected";
            SortSelectedConnectedMenu.Click += SortSelectedConnected;
            // 
            // Seperator1
            // 
            Seperator1.Name = "Seperator1";
            Seperator1.Size = new Size(152, 6);
            // 
            // toolStripButton1
            // 
            toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripButton1.ForeColor = Color.FromArgb(224, 224, 224);
            toolStripButton1.Image = (Image)resources.GetObject("toolStripButton1.Image");
            toolStripButton1.ImageTransparentColor = Color.Magenta;
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new Size(46, 22);
            toolStripButton1.Text = "Search";
            toolStripButton1.Click += toolStripButton1_Click;
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
            FormClosing += OnFormClosing;
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
        private ToolStripButton SaveButton;
        private ToolStripButton Add;
        private ToolStripButton NewStory;
        private ToolStripButton OpenButton;
        private TreeView StoryTree;
        private SplitContainer HierarchyAndRest;
        private SplitContainer GraphAndProperties;
        private DoubleBufferedPanel Graph;
        private TableLayoutPanel PropertyInspector;
        private ComboBox NodeSpawnBox;
        private ContextMenuStrip NodeContext;
        private ToolStripMenuItem PullChildsMenu;
        private ToolStripMenuItem PullParentsMenu;
        private ToolStripMenuItem SortConnectedMenu;
        private ToolStripMenuItem SortSelectedMenu;
        private ToolStripMenuItem SortSelectedConnectedMenu;
        private ToolStripSeparator Seperator1;
        private ToolStripLabel cursorPos;
        private ToolStripButton toolStripButton1;
    }
}

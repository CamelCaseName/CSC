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
            Menu = new ToolStrip();
            StartButton = new ToolStripButton();
            Add = new ToolStripButton();
            AddChild = new ToolStripButton();
            AddParent = new ToolStripButton();
            Details = new PropertyGrid();
            Menu.SuspendLayout();
            SuspendLayout();
            // 
            // Menu
            // 
            Menu.Items.AddRange(new ToolStripItem[] { StartButton, Add, AddChild, AddParent });
            Menu.Location = new Point(0, 0);
            Menu.Name = "Menu";
            Menu.Size = new Size(1327, 25);
            Menu.TabIndex = 0;
            Menu.Text = "toolStrip1";
            // 
            // StartButton
            // 
            StartButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            StartButton.Image = (Image)resources.GetObject("StartButton.Image");
            StartButton.ImageTransparentColor = Color.Magenta;
            StartButton.Name = "StartButton";
            StartButton.Size = new Size(35, 22);
            StartButton.Text = "Start";
            StartButton.Click += Start_Click;
            // 
            // Add
            // 
            Add.DisplayStyle = ToolStripItemDisplayStyle.Text;
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
            AddParent.Image = (Image)resources.GetObject("AddParent.Image");
            AddParent.ImageTransparentColor = Color.Magenta;
            AddParent.Name = "AddParent";
            AddParent.Size = new Size(70, 22);
            AddParent.Text = "Add Parent";
            AddParent.Click += AddParent_Click;
            // 
            // Details
            // 
            Details.AllowDrop = true;
            Details.Anchor = AnchorStyles.None;
            Details.HelpVisible = false;
            Details.Location = new Point(864, 54);
            Details.MaximumSize = new Size(600, 900);
            Details.MinimumSize = new Size(100, 50);
            Details.Name = "Details";
            Details.Size = new Size(414, 438);
            Details.TabIndex = 0;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(1327, 623);
            Controls.Add(Details);
            Controls.Add(Menu);
            MinimumSize = new Size(400, 200);
            Name = "Main";
            ShowIcon = false;
            Text = "Form1";
            Click += Main_Click;
            Paint += Main_Paint;
            DoubleClick += Main_DoubleClick;
            MouseMove += Main_MouseMove;
            Menu.ResumeLayout(false);
            Menu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip Menu;
        private ToolStripButton StartButton;
        private ToolStripButton Add;
        private PropertyGrid Details;
        private ToolStripButton AddChild;
        private ToolStripButton AddParent;
    }
}

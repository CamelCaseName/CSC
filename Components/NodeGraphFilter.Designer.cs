namespace CSC.Components
{
    partial class NodeGraphFilter
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            typelist = new CheckedListBox();
            SuspendLayout();
            // 
            // typelist
            // 
            typelist.BackColor = Color.FromArgb(40, 40, 40);
            typelist.BorderStyle = BorderStyle.None;
            typelist.Dock = DockStyle.Fill;
            typelist.ForeColor = Color.Silver;
            typelist.FormattingEnabled = true;
            typelist.Location = new Point(0, 0);
            typelist.Name = "typelist";
            typelist.Size = new Size(334, 311);
            typelist.TabIndex = 0;
            typelist.ItemCheck += TypeListCheckedChanged;
            // 
            // NodeGraphFilter
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(40, 40, 40);
            ClientSize = new Size(334, 311);
            Controls.Add(typelist);
            MinimumSize = new Size(350, 150);
            Name = "NodeGraphFilter";
            ShowIcon = false;
            Text = "Tick what node types to hide";
            ResumeLayout(false);
        }

        #endregion

        private CheckedListBox typelist;
    }
}
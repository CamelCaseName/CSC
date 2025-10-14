namespace CSC.Components
{
    partial class SearchDialog
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
            tableLayoutPanel1 = new TableLayoutPanel();
            panel2 = new Panel();
            label2 = new Label();
            datatype = new ComboBox();
            panel1 = new Panel();
            label1 = new Label();
            nodetype = new ComboBox();
            resultsTree = new TreeView();
            label4 = new Label();
            panel4 = new Panel();
            label5 = new Label();
            modifiers = new ComboBox();
            casesensitivity = new CheckBox();
            panel3 = new Panel();
            searchterm = new TextBox();
            label3 = new Label();
            tableLayoutPanel1.SuspendLayout();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            panel4.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = Color.FromArgb(40, 40, 40);
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 142F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 195F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(panel2, 1, 0);
            tableLayoutPanel1.Controls.Add(panel1, 0, 0);
            tableLayoutPanel1.Controls.Add(resultsTree, 0, 3);
            tableLayoutPanel1.Controls.Add(label4, 0, 2);
            tableLayoutPanel1.Controls.Add(panel4, 3, 0);
            tableLayoutPanel1.Controls.Add(casesensitivity, 2, 0);
            tableLayoutPanel1.Controls.Add(panel3, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.ForeColor = Color.White;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 16F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Size = new Size(722, 370);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.BackColor = Color.FromArgb(40, 40, 40);
            panel2.Controls.Add(label2);
            panel2.Controls.Add(datatype);
            panel2.Dock = DockStyle.Fill;
            panel2.ForeColor = Color.White;
            panel2.Location = new Point(145, 3);
            panel2.Name = "panel2";
            panel2.Size = new Size(189, 52);
            panel2.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.FromArgb(40, 40, 40);
            label2.ForeColor = Color.White;
            label2.Location = new Point(3, 6);
            label2.Name = "label2";
            label2.Size = new Size(58, 15);
            label2.TabIndex = 2;
            label2.Text = "Data Type";
            // 
            // datatype
            // 
            datatype.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            datatype.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            datatype.AutoCompleteSource = AutoCompleteSource.ListItems;
            datatype.BackColor = Color.FromArgb(64, 64, 64);
            datatype.DropDownStyle = ComboBoxStyle.DropDownList;
            datatype.ForeColor = Color.Black;
            datatype.FormattingEnabled = true;
            datatype.Location = new Point(3, 24);
            datatype.Name = "datatype";
            datatype.Size = new Size(183, 23);
            datatype.TabIndex = 0;
            datatype.SelectedIndexChanged += Datatype_SelectedIndexChanged;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(40, 40, 40);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(nodetype);
            panel1.Dock = DockStyle.Fill;
            panel1.ForeColor = Color.White;
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(136, 52);
            panel1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.FromArgb(40, 40, 40);
            label1.ForeColor = Color.White;
            label1.Location = new Point(9, 6);
            label1.Name = "label1";
            label1.Size = new Size(63, 15);
            label1.TabIndex = 1;
            label1.Text = "Node Type";
            // 
            // nodetype
            // 
            nodetype.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            nodetype.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            nodetype.AutoCompleteSource = AutoCompleteSource.ListItems;
            nodetype.BackColor = Color.FromArgb(64, 64, 64);
            nodetype.DropDownStyle = ComboBoxStyle.DropDownList;
            nodetype.ForeColor = Color.Black;
            nodetype.FormattingEnabled = true;
            nodetype.Location = new Point(3, 24);
            nodetype.Name = "nodetype";
            nodetype.Size = new Size(130, 23);
            nodetype.TabIndex = 0;
            nodetype.SelectedIndexChanged += Nodetype_SelectedIndexChanged;
            // 
            // resultsTree
            // 
            resultsTree.BackColor = Color.FromArgb(40, 40, 40);
            tableLayoutPanel1.SetColumnSpan(resultsTree, 4);
            resultsTree.Dock = DockStyle.Fill;
            resultsTree.ForeColor = Color.White;
            resultsTree.FullRowSelect = true;
            resultsTree.HideSelection = false;
            resultsTree.Location = new Point(3, 157);
            resultsTree.Name = "resultsTree";
            resultsTree.Size = new Size(716, 210);
            resultsTree.TabIndex = 5;
            resultsTree.AfterSelect += Results_AfterSelect;
            // 
            // label4
            // 
            label4.BackColor = Color.FromArgb(40, 40, 40);
            tableLayoutPanel1.SetColumnSpan(label4, 4);
            label4.Dock = DockStyle.Fill;
            label4.ForeColor = Color.White;
            label4.Location = new Point(3, 138);
            label4.Name = "label4";
            label4.Size = new Size(716, 16);
            label4.TabIndex = 6;
            label4.Text = "Results:";
            label4.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel4
            // 
            panel4.BackColor = Color.FromArgb(40, 40, 40);
            panel4.Controls.Add(label5);
            panel4.Controls.Add(modifiers);
            panel4.Dock = DockStyle.Fill;
            panel4.ForeColor = Color.White;
            panel4.Location = new Point(420, 3);
            panel4.Name = "panel4";
            panel4.Size = new Size(299, 52);
            panel4.TabIndex = 7;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.FromArgb(40, 40, 40);
            label5.ForeColor = Color.White;
            label5.Location = new Point(3, 6);
            label5.Name = "label5";
            label5.Size = new Size(60, 15);
            label5.TabIndex = 1;
            label5.Text = "Modifiers:";
            label5.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // modifiers
            // 
            modifiers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            modifiers.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            modifiers.AutoCompleteSource = AutoCompleteSource.ListItems;
            modifiers.BackColor = Color.FromArgb(64, 64, 64);
            modifiers.DropDownStyle = ComboBoxStyle.DropDownList;
            modifiers.ForeColor = Color.Black;
            modifiers.FormattingEnabled = true;
            modifiers.Location = new Point(3, 24);
            modifiers.Name = "modifiers";
            modifiers.Size = new Size(293, 23);
            modifiers.TabIndex = 0;
            modifiers.SelectedIndexChanged += Modifiers_SelectedIndexChanged;
            // 
            // casesensitivity
            // 
            casesensitivity.AutoSize = true;
            casesensitivity.BackColor = Color.FromArgb(40, 40, 40);
            casesensitivity.CheckAlign = ContentAlignment.MiddleRight;
            casesensitivity.Dock = DockStyle.Fill;
            casesensitivity.ForeColor = Color.White;
            casesensitivity.Location = new Point(340, 3);
            casesensitivity.Name = "casesensitivity";
            casesensitivity.Size = new Size(74, 52);
            casesensitivity.TabIndex = 4;
            casesensitivity.Text = "Case sensitive";
            casesensitivity.TextAlign = ContentAlignment.MiddleRight;
            casesensitivity.UseVisualStyleBackColor = false;
            casesensitivity.CheckedChanged += Casesensitivity_CheckedChanged;
            // 
            // panel3
            // 
            panel3.BackColor = Color.FromArgb(40, 40, 40);
            tableLayoutPanel1.SetColumnSpan(panel3, 4);
            panel3.Controls.Add(searchterm);
            panel3.Controls.Add(label3);
            panel3.Dock = DockStyle.Fill;
            panel3.ForeColor = Color.White;
            panel3.Location = new Point(3, 61);
            panel3.Name = "panel3";
            panel3.Size = new Size(716, 74);
            panel3.TabIndex = 8;
            // 
            // searchterm
            // 
            searchterm.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            searchterm.BackColor = Color.FromArgb(64, 64, 64);
            searchterm.ForeColor = Color.White;
            searchterm.Location = new Point(3, 24);
            searchterm.Multiline = true;
            searchterm.Name = "searchterm";
            searchterm.ScrollBars = ScrollBars.Both;
            searchterm.Size = new Size(710, 47);
            searchterm.TabIndex = 3;
            searchterm.TextChanged += Searchterm_TextChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.FromArgb(40, 40, 40);
            label3.ForeColor = Color.White;
            label3.Location = new Point(3, 6);
            label3.Name = "label3";
            label3.Size = new Size(75, 15);
            label3.TabIndex = 2;
            label3.Text = "Search terms";
            // 
            // SearchDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(722, 370);
            Controls.Add(tableLayoutPanel1);
            MinimumSize = new Size(530, 260);
            Name = "SearchDialog";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Search:";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private ComboBox nodetype;
        private Panel panel2;
        private ComboBox datatype;
        private Label label1;
        private Label label2;
        private CheckBox casesensitivity;
        private TreeView resultsTree;
        private Label label4;
        private Panel panel4;
        private Label label5;
        private ComboBox modifiers;
        private Panel panel3;
        private TextBox searchterm;
        private Label label3;
    }
}
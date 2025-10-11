namespace CSC.Components
{
    partial class SaveOnCloseDialog
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
            label1 = new Label();
            SaveAll = new Button();
            SaveStories = new Button();
            SavePositions = new Button();
            Cancel = new Button();
            button1 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.ForeColor = Color.White;
            label1.Location = new Point(-1, 0);
            label1.Name = "label1";
            label1.Size = new Size(446, 56);
            label1.TabIndex = 0;
            label1.Text = "Your changes may have not been saved, do you want to save now?";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // SaveAll
            // 
            SaveAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SaveAll.BackColor = Color.FromArgb(64, 64, 64);
            SaveAll.DialogResult = DialogResult.Yes;
            SaveAll.ForeColor = Color.White;
            SaveAll.Location = new Point(12, 70);
            SaveAll.Name = "SaveAll";
            SaveAll.Size = new Size(75, 23);
            SaveAll.TabIndex = 1;
            SaveAll.Text = "Save &All";
            SaveAll.UseVisualStyleBackColor = false;
            // 
            // SaveStories
            // 
            SaveStories.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SaveStories.BackColor = Color.FromArgb(64, 64, 64);
            SaveStories.DialogResult = DialogResult.OK;
            SaveStories.ForeColor = Color.White;
            SaveStories.Location = new Point(93, 70);
            SaveStories.Name = "SaveStories";
            SaveStories.Size = new Size(90, 23);
            SaveStories.TabIndex = 2;
            SaveStories.Text = "Save &Stories";
            SaveStories.UseVisualStyleBackColor = false;
            // 
            // SavePositions
            // 
            SavePositions.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SavePositions.BackColor = Color.FromArgb(64, 64, 64);
            SavePositions.DialogResult = DialogResult.Continue;
            SavePositions.ForeColor = Color.White;
            SavePositions.Location = new Point(189, 70);
            SavePositions.Name = "SavePositions";
            SavePositions.Size = new Size(92, 23);
            SavePositions.TabIndex = 3;
            SavePositions.Text = "Save &Positions";
            SavePositions.UseVisualStyleBackColor = false;
            // 
            // Cancel
            // 
            Cancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Cancel.BackColor = Color.FromArgb(64, 64, 64);
            Cancel.DialogResult = DialogResult.Cancel;
            Cancel.ForeColor = Color.White;
            Cancel.Location = new Point(363, 70);
            Cancel.Name = "Cancel";
            Cancel.Size = new Size(75, 23);
            Cancel.TabIndex = 4;
            Cancel.Text = "&Cancel";
            Cancel.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            button1.BackColor = Color.FromArgb(64, 64, 64);
            button1.DialogResult = DialogResult.Ignore;
            button1.ForeColor = Color.White;
            button1.Location = new Point(287, 70);
            button1.Name = "button1";
            button1.Size = new Size(70, 23);
            button1.TabIndex = 5;
            button1.Text = "&Dont save";
            button1.UseVisualStyleBackColor = false;
            // 
            // SaveOnCloseDialog
            // 
            AcceptButton = SaveAll;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            CancelButton = Cancel;
            ClientSize = new Size(445, 105);
            Controls.Add(button1);
            Controls.Add(Cancel);
            Controls.Add(SavePositions);
            Controls.Add(SaveStories);
            Controls.Add(SaveAll);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SaveOnCloseDialog";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Unsaved changes...";
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Button SaveAll;
        private Button SaveStories;
        private Button SavePositions;
        private Button Cancel;
        private Button button1;
    }
}
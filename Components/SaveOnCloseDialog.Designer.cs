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
            SaveAll.BackColor = Color.FromArgb(64, 64, 64);
            SaveAll.DialogResult = DialogResult.Yes;
            SaveAll.ForeColor = Color.White;
            SaveAll.Location = new Point(12, 92);
            SaveAll.Name = "SaveAll";
            SaveAll.Size = new Size(75, 23);
            SaveAll.TabIndex = 1;
            SaveAll.Text = "Save &All";
            SaveAll.UseVisualStyleBackColor = false;
            // 
            // SaveStories
            // 
            SaveStories.BackColor = Color.FromArgb(64, 64, 64);
            SaveStories.DialogResult = DialogResult.OK;
            SaveStories.ForeColor = Color.White;
            SaveStories.Location = new Point(104, 92);
            SaveStories.Name = "SaveStories";
            SaveStories.Size = new Size(111, 23);
            SaveStories.TabIndex = 2;
            SaveStories.Text = "Save &Stories";
            SaveStories.UseVisualStyleBackColor = false;
            // 
            // SavePositions
            // 
            SavePositions.BackColor = Color.FromArgb(64, 64, 64);
            SavePositions.DialogResult = DialogResult.Continue;
            SavePositions.ForeColor = Color.White;
            SavePositions.Location = new Point(233, 92);
            SavePositions.Name = "SavePositions";
            SavePositions.Size = new Size(108, 23);
            SavePositions.TabIndex = 3;
            SavePositions.Text = "Save &Positions";
            SavePositions.UseVisualStyleBackColor = false;
            // 
            // Cancel
            // 
            Cancel.BackColor = Color.FromArgb(64, 64, 64);
            Cancel.DialogResult = DialogResult.Cancel;
            Cancel.ForeColor = Color.White;
            Cancel.Location = new Point(358, 92);
            Cancel.Name = "Cancel";
            Cancel.Size = new Size(75, 23);
            Cancel.TabIndex = 4;
            Cancel.Text = "&Cancel";
            Cancel.UseVisualStyleBackColor = false;
            // 
            // SaveOnCloseDialog
            // 
            AcceptButton = SaveAll;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            CancelButton = Cancel;
            ClientSize = new Size(445, 127);
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
            Text = "SaveOnCloseDialog";
            TopMost = true;
            ResumeLayout(false);
        }

        #endregion

        private Label label1;
        private Button SaveAll;
        private Button SaveStories;
        private Button SavePositions;
        private Button Cancel;
    }
}
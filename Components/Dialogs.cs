using System.Text.RegularExpressions;

namespace CSC
{
    public static partial class Dialogs
    {

        public static DialogResult ShowTextBox(ref string input, string prompt, string title = "Title")
        {
            int width = 300, height = 90;
            Size size = new(width, height);
            Form inputBox = new()
            {
                FormBorderStyle = FormBorderStyle.FixedDialog,
                TopMost = true,
                ClientSize = size,
                Text = title,
                SizeGripStyle = SizeGripStyle.Hide,
                WindowState = FormWindowState.Normal,
                MaximizeBox = false,
                MinimizeBox = false,
            };

            Label label = new()
            {
                Text = prompt,
                Location = new Point(5, 5),
                Width = size.Width - 10
            };
            inputBox.Controls.Add(label);

            TextBox textBox = new()
            {
                Size = new Size(size.Width - 10, 23),
                Location = new Point(5, label.Location.Y + 25),
                Text = input,
            };
            textBox.Validating += (_, e) =>
            {
                if (textBox.Text.Length == 0)
                {
                    MessageBox.Show("Name cannot be empty", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
                if (!regex().IsMatch(textBox.Text))
                {
                    MessageBox.Show("Name cannot contain other symbols as english letters, numbers, spaces and underscores", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            };
            inputBox.Controls.Add(textBox);

            Button okButton = new()
            {
                DialogResult = DialogResult.OK,
                Name = "okButton",
                Size = new Size(75, 23),
                Text = "OK",
                Location = new Point(size.Width - 80 - 80, size.Height - 30)
            };
            inputBox.Controls.Add(okButton);

            Button cancelButton = new()
            {
                DialogResult = DialogResult.Cancel,
                Name = "cancelButton",
                Size = new Size(75, 23),
                Text = "Cancel",
                Location = new Point(size.Width - 80, size.Height - 30)
            };
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;

            return result;
        }

        internal static DialogResult ShowDropdownBox(ref string selectedChoice, object[] choices, string prompt, string title)
        {
            int width = 300, height = 90;
            Size size = new(width, height);
            Form inputBox = new()
            {
                FormBorderStyle = FormBorderStyle.FixedDialog,
                TopMost = true,
                ClientSize = size,
                Text = title,
                SizeGripStyle = SizeGripStyle.Hide,
                WindowState = FormWindowState.Normal,
                MaximizeBox = false,
                MinimizeBox = false,
            };

            Label label = new()
            {
                Text = prompt,
                Location = new Point(5, 5),
                Width = size.Width - 10
            };
            inputBox.Controls.Add(label);

            ComboBox comboBox = new()
            {
                Size = new Size(size.Width - 10, 23),
                Location = new Point(5, label.Location.Y + 25),
                AutoCompleteMode = AutoCompleteMode.SuggestAppend,
                AutoCompleteSource = AutoCompleteSource.ListItems,
                DropDownStyle = ComboBoxStyle.DropDownList,
                CausesValidation = true,
            };
            comboBox.Items.AddRange(choices);
            comboBox.Validating += (_, e) =>
            {
                if (comboBox.SelectedItem is null)
                {
                    MessageBox.Show("You need to choose a name!", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            };
            inputBox.Controls.Add(comboBox);

            Button okButton = new()
            {
                DialogResult = DialogResult.OK,
                Name = "okButton",
                Size = new Size(75, 23),
                Text = "OK",
                Location = new Point(size.Width - 80 - 80, size.Height - 30)
            };
            inputBox.Controls.Add(okButton);

            Button cancelButton = new()
            {
                DialogResult = DialogResult.Cancel,
                Name = "cancelButton",
                Size = new Size(75, 23),
                Text = "Cancel",
                Location = new Point(size.Width - 80, size.Height - 30)
            };
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            selectedChoice = comboBox.SelectedItem?.ToString()!;

            return result;
        }

        [GeneratedRegex(@"^[a-zA-Z0-9_ \-]+$")]
        private static partial Regex regex();
    }
}
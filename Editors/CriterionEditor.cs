using CSC.StoryItems;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace CSC.Editors
{
    internal class CriterionEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        {

            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            Criterion criterion = value as Criterion;
            if (svc != null && criterion != null)
            {
                using (CriterionEdit form = new CriterionEdit())
                {
                    form.Value = criterion.Value;
                    if (svc.ShowDialog(form) == DialogResult.OK)
                    {
                        criterion.Value = form.Value; // update object
                    }
                }
            }
            return value; // can also replace the wrapper object here
        }
    }

    class CriterionEdit : Form
    {
        private TextBox textbox;
        private Button okButton;
        public CriterionEdit()
        {
            textbox = new TextBox();
            Controls.Add(textbox);
            okButton = new Button();
            okButton.Text = "OK";
            okButton.Dock = DockStyle.Bottom;
            okButton.DialogResult = DialogResult.OK;
            Controls.Add(okButton);
        }
        public string Value
        {
            get { return textbox.Text; }
            set { textbox.Text = value; }
        }
    }
}

using System;
using System.Windows.Forms;

namespace FlowScriptPrototype
{
    public partial class NewCategoryForm : Form
    {
        public String CategoryName
        {
            get { return _catNameTextBox.Text ?? ""; }
        }

        public bool IsCategoryValid
        {
            get
            {
                return CategoryName.Length > 0;
            }
        }
        
        public NewCategoryForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            _addCatBtn.Enabled = false;

            CenterToParent();
        }

        private void _catNameTextBox_TextChanged(object sender, EventArgs e)
        {
            _addCatBtn.Enabled = IsCategoryValid;
        }

        private void _addCatBtn_Click(object sender, EventArgs e)
        {
            if (IsCategoryValid) {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void _cancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}

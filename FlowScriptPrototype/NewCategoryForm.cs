using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowScriptPrototype
{
    public partial class NewCategoryForm : Form
    {
        public String CategoryName
        {
            get { return _catNameTextBox.Text ?? ""; }
        }

        public bool IsInputValid
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
            _addCatBtn.Enabled = IsInputValid;
        }

        private void _addCatBtn_Click(object sender, EventArgs e)
        {
            if (IsInputValid) {
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

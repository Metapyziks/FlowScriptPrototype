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
    public partial class NewNodeForm : Form
    {
        public String NodeIdentifier
        {
            get { return _nodeNameTextBox.Text ?? ""; }
        }

        public int NodeInputCount
        {
            get { return (int) _inputCountNUD.Value; }
        }

        public int NodeOutputCount
        {
            get { return (int) _outputCountNUD.Value; }
        }

        public bool IsIdentifierValid
        {
            get
            {
                return NodeIdentifier.Length > 0;
            }
        }

        public NewNodeForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            _addNodeBtn.Enabled = false;

            CenterToParent();
        }

        private void _nodeNameTextBox_TextChanged(object sender, EventArgs e)
        {
            _addNodeBtn.Enabled = IsIdentifierValid;
        }

        private void _addNodeBtn_Click(object sender, EventArgs e)
        {
            if (IsIdentifierValid) {
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

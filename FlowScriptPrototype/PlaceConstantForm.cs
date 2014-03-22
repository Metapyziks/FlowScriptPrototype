using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FlowScriptPrototype
{
    public partial class PlaceConstantForm : Form
    {
        private static readonly Regex _sStringRegex = new Regex("^\"(\\\\.|[^\"])*\"$");

        public Signal ConstValue
        {
            get
            {
                var str = _constValTextBox.Text;
                long intVal; double doubleVal;

                if (_sStringRegex.IsMatch(str)) {
                    return new StringSignal(str.Substring(1, str.Length - 2));
                } else if (long.TryParse(str, out intVal)) {
                    return new IntSignal(intVal);
                } else if (double.TryParse(str, out doubleVal)) {
                    return new RealSignal(doubleVal);
                } else {
                    return null;
                }
            }
        }

        public bool IsValueValid
        {
            get { return ConstValue != null; }
        }

        public PlaceConstantForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            _createBtn.Enabled = false;

            CenterToParent();
        }

        private void _constValTextBox_TextChanged(object sender, EventArgs e)
        {
            _createBtn.Enabled = IsValueValid;
        }

        private void _createBtn_Click(object sender, EventArgs e)
        {
            if (IsValueValid) {
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

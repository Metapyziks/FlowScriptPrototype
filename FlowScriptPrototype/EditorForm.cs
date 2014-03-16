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
    public partial class EditorForm : Form
    {
        public PrototypeNode Prototype { get; private set; }

        public EditorForm(PrototypeNode prototype = null)
        {
            Prototype = prototype ?? new PrototypeNode("Example", "Test", 0, 0);

            InitializeComponent();
            
            Text = Prototype.ToString();
        }

        private void UpdateNodeMenu()
        {
            _nodeMenu.Items.Clear();

            foreach (var category in Node.Categories) {
                var item = new ToolStripMenuItem(category);

                foreach (var identifier in Node.GetIdentifiers(category)) {
                    item.DropDownItems.Add(identifier);
                }

                var newItem = item.DropDownItems.Add("New...");
                
                _nodeMenu.Items.Add(item);
            }

            var addNewItem = _nodeMenu.Items.Add("New...");

            addNewItem.Click += (sender, e) => {
                var dialog = new NewCategoryForm();
                var result = dialog.ShowDialog();

                if (result == DialogResult.OK) {
                    CustomNode.CreateCategory(dialog.CategoryName);
                    UpdateNodeMenu();
                }
            };
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _nodeMenu.Items.Clear();

            UpdateNodeMenu();
        }
    }
}

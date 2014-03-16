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
        private PlacedNode[] _dragging;
        private Point _dragStart;

        public PrototypeNode Prototype { get; private set; }

        public EditorForm(PrototypeNode prototype = null)
        {
            Prototype = prototype ?? new PrototypeNode("Example", "Test", 1, 1);

            InitializeComponent();
            
            Text = Prototype.ToString();
        }

        private bool IsDragging(PlacedNode node)
        {
            return _dragging != null && _dragging.Contains(node);
        }

        private void StartDragging(params PlacedNode[] nodes)
        {
            _dragging = nodes;
            _dragStart = Cursor.Position;
        }

        private void StopDragging()
        {
            var diff = new Point(
                Cursor.Position.X - _dragStart.X,
                Cursor.Position.Y - _dragStart.Y);

            foreach (var node in _dragging) {
                node.Offset(diff);
            }

            _dragging = null;
            _viewPanel.Invalidate();
        }

        private void UpdateNodeMenu()
        {
            _nodeMenu.Items.Clear();

            foreach (var category in Node.Categories) {
                var item = new ToolStripMenuItem(category);

                foreach (var identifier in Node.GetIdentifiers(category)) {
                    item.DropDownItems.Add(identifier).MouseDown += (sender, e) => {
                        var node = Prototype.AddNode(category, identifier);
                        node.Offset(_viewPanel.PointToClient(Cursor.Position));
                        node.Offset(-node.Size.Width / 2, -node.Size.Height / 2);
                        StartDragging(node);

                        item.HideDropDown();
                        _viewPanel.Invalidate();
                    };
                }

                item.DropDownItems.Add("New...").Click += (sender, e) => {
                    var dialog = new NewNodeForm();
                    var result = dialog.ShowDialog();

                    if (result == DialogResult.OK) {
                        CustomNode.CreatePrototype(category,
                            dialog.NodeIdentifier,
                            dialog.NodeInputCount,
                            dialog.NodeOutputCount, node => {
                                var form = new EditorForm(node);
                                form.Show();
                            });

                        UpdateNodeMenu();
                    }
                };
                
                _nodeMenu.Items.Add(item);
            }

            _nodeMenu.Items.Add("New...").Click += (sender, e) => {
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

        private void _viewPanel_Paint(object sender, PaintEventArgs e)
        {
            var diffAdd = new Point(
                Cursor.Position.X - _dragStart.X,
                Cursor.Position.Y - _dragStart.Y);

            var diffSub = new Point(-diffAdd.X, -diffAdd.Y);

            foreach (var node in Prototype.InnerNodes) {
                bool dragging = IsDragging(node);

                if (dragging) node.Offset(diffAdd);

                node.Draw(e.Graphics);

                if (dragging) node.Offset(diffSub);
            }
        }

        private void _viewPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (_dragging != null) {
                StopDragging();
            }
        }

        private void _viewPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging != null) {
                _viewPanel.Invalidate();
            }
        }
    }
}

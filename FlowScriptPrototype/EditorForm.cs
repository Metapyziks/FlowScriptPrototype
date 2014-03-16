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
        private List<PlacedNode> _selection;
        private bool _dragging;
        private Point _dragStart;

        public PrototypeNode Prototype { get; private set; }

        public EditorForm(PrototypeNode prototype = null)
        {
            Prototype = prototype ?? new PrototypeNode("Example", "Test", 1, 1);
            _selection = new List<PlacedNode>();

            InitializeComponent();
            
            Text = Prototype.ToString();
        }

        private bool IsSelected(PlacedNode node)
        {
            return _selection.Contains(node);
        }

        private bool IsDragging(PlacedNode node)
        {
            return _dragging && IsSelected(node);
        }

        private void ClearSelection()
        {
            _selection.Clear();
        }

        private void SelectNode(params PlacedNode[] node)
        {
            _selection.AddRange(node);
        }

        private void StartDragging()
        {
            _dragging = true;
            _dragStart = Cursor.Position;
        }

        private void StopDragging()
        {
            if (_dragging) {
                var diff = new Point(
                    Cursor.Position.X - _dragStart.X,
                    Cursor.Position.Y - _dragStart.Y);

                foreach (var node in _selection) {
                    node.Offset(diff);
                }

                _dragging = false;
                _viewPanel.Invalidate();
            }
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

                        ClearSelection();
                        SelectNode(node);
                        StartDragging();

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

                node.Draw(e.Graphics, IsSelected(node));

                if (dragging) node.Offset(diffSub);
            }
        }

        private PlacedNode GetIntersectingNode(Point pos)
        {
            return Prototype.InnerNodes.LastOrDefault(x => x.Bounds.Contains(pos));
        }

        private void _viewPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (_dragging) {
                StopDragging();
            }
        }

        private void _viewPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging) {
                _viewPanel.Invalidate();
            }
        }

        private void _viewPanel_MouseDown(object sender, MouseEventArgs e)
        {
            var node = GetIntersectingNode(e.Location);

            if (node != null) {
                if (!IsSelected(node)) {
                    if (!ModifierKeys.HasFlag(Keys.Shift)) ClearSelection();
                    SelectNode(node);
                }

                StartDragging();
            } else {
                ClearSelection();
            }

            _viewPanel.Invalidate();
        }
    }
}

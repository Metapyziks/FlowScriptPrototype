using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlowScriptPrototype
{
    public partial class EditorForm : Form
    {
        private static List<EditorForm> _sOpenForms = new List<EditorForm>();

        private List<PlacedNode> _selection;
        private bool _dragging;
        private Point _dragStart;

        private bool _wiring;
        private Socket _wireStart;

        private bool _modified;

        public PrototypeNode Prototype { get; private set; }

        public EditorForm(PrototypeNode prototype = null)
        {
            Prototype = prototype ?? new PrototypeNode("Example", "Test", 1, 1);
            _selection = new List<PlacedNode>();

            InitializeComponent();

            KeyPreview = true;
            
            SetModified(prototype == null);

            _sOpenForms.Add(this);
        }

        private void SetModified(bool modified)
        {
            _modified = modified;

            if (_modified) {
                Text = String.Format("{0} [modified]", Prototype.ToString());
            } else {
                Text = Prototype.ToString();
            }
        }

        private void Save()
        {
            var path = Path.Combine("Data", Prototype.Category, String.Format("{0}.json", Prototype.Identifier));
            var dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }

            ((PrototypeNodeSave) Prototype.GetSave()).Save(path);

            SetModified(false);
        }

        protected override void OnClosed(EventArgs e)
        {
            _sOpenForms.Remove(this);
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

                SetModified(true);

                _dragging = false;
                _viewPanel.Invalidate();
            }
        }

        private void StartWiring(Socket socket)
        {
            if (!socket.IsNull) {
                _wiring = true;
                _wireStart = socket;
            }
        }

        private void StopWiring(Socket socket)
        {
            if (!socket.IsNull) {
                _wireStart.ConnectToInput(socket);
                Prototype.ClearRecycledInstances();

                SetModified(true);
            }

            _wiring = false;
        }

        private void UpdateNodeMenu()
        {
            _nodeMenu.Items.Clear();

            var fileMenu = new ToolStripMenuItem("File");

            fileMenu.DropDownItems.Add("Save").Click += (sender, e) => {
                Save();
            };

            fileMenu.DropDownItems.Add("Run Test").Click += (sender, e) => {
                Prototype.PulseInput(0, new IntSignal(1));

                while (Node.Step());
            };

            _nodeMenu.Items.Add(fileMenu);

            _nodeMenu.Items.Add("Constant").Click += (sender, e) => {
                var dialog = new PlaceConstantForm();
                var result = dialog.ShowDialog();

                if (result == DialogResult.OK) {
                    var node = Prototype.AddConstant(dialog.ConstValue);
                    node.Offset(_viewPanel.PointToClient(Cursor.Position));
                    node.Offset(-node.Size.Width / 2, -node.Size.Height / 2);

                    ClearSelection();
                    SelectNode(node);

                    _viewPanel.Invalidate();
                }
            };

            foreach (var category in Node.Categories) {
                var item = new ToolStripMenuItem(category);

                foreach (var identifier in Node.GetIdentifiers(category)) {
                    var nodeItem = item.DropDownItems.Add(identifier);
                    
                    nodeItem.MouseDown += (sender, e) => {
                        if (e.Button != MouseButtons.Left) return;

                        var node = Prototype.AddNode(category, identifier);
                        node.Offset(_viewPanel.PointToClient(Cursor.Position));
                        node.Offset(-node.Size.Width / 2, -node.Size.Height / 2);

                        ClearSelection();
                        SelectNode(node);
                        StartDragging();

                        item.HideDropDown();
                        _viewPanel.Invalidate();
                    };

                    var inst = Node.GetInstance(category, identifier);

                    if (inst is ReferenceNode) {
                        nodeItem.MouseUp += (sender, e) => {
                            if (e.Button != MouseButtons.Right) return;

                            var form = _sOpenForms.FirstOrDefault(x =>
                                x.Prototype.Category == category &&
                                x.Prototype.Identifier == identifier);

                            if (form != null) {
                                form.BringToFront();
                                form.Focus();
                            } else {
                                form = new EditorForm(((ReferenceNode) inst).Prototype);
                                form.Show();
                            }
                        };
                    }
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

            var output = _dragging || _wiring ? new Socket(null, 0)
                : GetIntersectingOutput(_viewPanel.PointToClient(Cursor.Position));

            if (_wiring) {
                PlacedNode.DrawConnection(e.Graphics,
                    ((PlacedNode) _wireStart.Node).GetOutputLocation(_wireStart.Index),
                    _viewPanel.PointToClient(Cursor.Position));
            }

            if (_dragging) {
                foreach (var node in Prototype.Nodes) {
                    if (IsSelected(node)) node.Offset(diffAdd);
                }
            }

            foreach (var node in Prototype.Nodes) {
                node.Draw(e.Graphics, IsSelected(node), output);
            }

            if (_dragging) {
                foreach (var node in Prototype.Nodes) {
                    if (IsSelected(node)) node.Offset(diffSub);
                }
            }
        }

        private PlacedNode GetIntersectingNode(Point pos)
        {
            return Prototype.Nodes.LastOrDefault(x => x.Bounds.Contains(pos));
        }

        private Socket GetIntersectingInput(Point pos)
        {
            foreach (var node in Prototype.Nodes) {
                if (node.IsInput) continue;

                for (int i = 0; i < node.InputCount; ++i) {
                    var outPos = node.GetInputLocation(i);
                    var diff = Point.Subtract(pos, new Size(outPos));

                    if (diff.X * diff.X + diff.Y * diff.Y < 64) {
                        return new Socket(node, i);
                    }
                }
            }

            return new Socket(null, 0);
        }

        private Socket GetIntersectingOutput(Point pos)
        {
            foreach (var node in Prototype.Nodes) {
                if (node.IsOutput) continue;

                for (int i = 0; i < node.OutputCount; ++i) {
                    var outPos = node.GetOutputLocation(i);
                    var diff = Point.Subtract(pos, new Size(outPos));

                    if (diff.X * diff.X + diff.Y * diff.Y < 64) {
                        return new Socket(node, i);
                    }
                }
            }

            return new Socket(null, 0);
        }

        private void _viewPanel_MouseDown(object sender, MouseEventArgs e)
        {
            var socket = GetIntersectingOutput(e.Location);

            if (!socket.IsNull) {
                if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                    StartWiring(socket);
                } else if (e.Button == System.Windows.Forms.MouseButtons.Right) {
                    socket.ClearOutputs();

                    SetModified(true);
                }

                return;
            }

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

        private void _viewPanel_MouseMove(object sender, MouseEventArgs e)
        {
            _viewPanel.Invalidate();
        }

        private void _viewPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (_wiring) {
                StopWiring(GetIntersectingInput(e.Location));
            } else if (_dragging) {
                StopDragging();
            }
        }

        private void EditorForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode) {
                case Keys.Delete: {
                    var deleted = _selection.Where(x => !x.IsInput && !x.IsOutput).ToArray();

                    if (deleted.Length > 0) {
                        foreach (var node in deleted) {
                            Prototype.RemoveNode(node);
                            _selection.Remove(node);

                            SetModified(true);
                        }

                        Prototype.ClearRecycledInstances();
                        _viewPanel.Invalidate();
                    }
                } break;
                case Keys.S: {
                    if (ModifierKeys.HasFlag(Keys.Control)) {
                        Save();
                    }
                } break;
            }
        }
    }
}

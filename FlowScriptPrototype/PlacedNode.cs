using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowScriptPrototype
{
    public class PlacedNode : Node
    {
        private static readonly Bitmap _sBlank = new Bitmap(1, 1);

        private static readonly Pen _sOutlinePen = new Pen(Color.FromArgb(0x68, 0x68, 0x68));
        private static readonly Brush _sFillBrush = new SolidBrush(Color.FromArgb(0x3e, 0x3e, 0x42));
        private static readonly Brush _sInOutBrush = new SolidBrush(Color.FromArgb(0x68, 0x68, 0x68));
        private static readonly Pen _sConnectionPen = new Pen(Color.FromArgb(0x68, 0x68, 0x68), 4f);

        private static readonly Pen _sSelectedPen = new Pen(Color.FromArgb(0x26, 0x4F, 0x78));
        private static readonly Pen _sSelectedConnectionPen = new Pen(Color.FromArgb(0x26, 0x4F, 0x78), 4f);
        private static readonly Brush _sSelectedBrush = new SolidBrush(Color.FromArgb(0x26, 0x4F, 0x78));

        private static readonly Font _sLabelFont = new Font(FontFamily.GenericSansSerif, 12f);
        private static readonly Brush _sLabelBrush = new SolidBrush(Color.FromArgb(0xdc, 0xdc, 0xdc));

        public static void DrawConnection(Graphics context, Point s, Point e)
        {
            DrawConnection(context, s, e, _sConnectionPen);
        }

        public static void DrawConnection(Graphics context, Point s, Point e, Pen pen)
        {
            var offset = Math.Max(16, Math.Abs(s.X - e.X) / 2);

            context.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            context.DrawBezier(pen,
                s.X, s.Y,
                s.X + offset, s.Y,
                e.X - offset, e.Y,
                e.X, e.Y);
            context.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
        }

        private Node _instance;

        public int Index { get; private set; }

        public Point Location { get; set; }

        public Size Size { get; private set; }

        public virtual bool IsInput { get { return false; } }

        public virtual bool IsOutput { get { return false; } }

        public Rectangle Bounds
        {
            get { return new Rectangle(Location, Size); }
        }

        public virtual String Text { get { return _instance.ToString(); } }

        internal PlacedNode(int index, Node instance)
            : base(instance.InputCount, instance.OutputCount)
        {
            Index = index;

            _instance = instance;

            Location = new Point();
            
            using (var ctx = Graphics.FromImage(_sBlank)) {
                var size = ctx.MeasureString(Text, _sLabelFont);
                Size = new Size(Math.Max(64, (int) (size.Width + 8)), 8 + Math.Max(instance.InputCount, instance.OutputCount) * 24);
            }
        }

        public void Offset(Point p)
        {
            Location = new Point(Location.X + p.X, Location.Y + p.Y);
        }

        public void Offset(int dx, int dy)
        {
            Location = new Point(Location.X + dx, Location.Y + dy);
        }

        public override Node ConnectToInput(int index, Socket input)
        {
            return _instance.ConnectToInput(index, input);
        }

        public override Node ClearOutputs(int index)
        {
            return _instance.ClearOutputs(index);
        }

        public override IEnumerable<Socket> GetOutputs(int index)
        {
            return _instance.GetOutputs(index);
        }

        public override void Pulse(params Signal[] inputs)
        {
            _instance.Pulse(inputs);
        }

        public override Node Clone()
        {
            return _instance.Clone();
        }

        public Point GetInputLocation(int index)
        {
            return new Point(Bounds.Left, Bounds.Top + 16 + 24 * index);
        }

        public Point GetOutputLocation(int index)
        {
            return new Point(Bounds.Right, Bounds.Top + 16 + 24 * index);
        }

        public void Draw(Graphics context, bool selected, Socket selectedOutput)
        {
            context.FillRectangle(_sFillBrush, Bounds);
            context.DrawRectangle(selected ? _sSelectedPen : _sOutlinePen, Bounds);

            if (!IsInput) {
                for (int i = 0; i < InputCount; ++i) {
                    var loc = GetInputLocation(i);
                    context.FillRectangle(_sInOutBrush, loc.X - 2, loc.Y - 4, 4, 8);
                }
            }

            if (!IsOutput) {
                for (int i = 0; i < OutputCount; ++i) {
                    var loc = GetOutputLocation(i);

                    foreach (var socket in _instance.GetOutputs(i)) {
                        var other = socket.Node as PlacedNode;

                        if (other == null) continue;

                        DrawConnection(context, loc, other.GetInputLocation(socket.Index),
                            selected ? _sSelectedConnectionPen : _sConnectionPen);
                    }

                    if (selectedOutput.Node == this && selectedOutput.Index == i) {
                        context.FillRectangle(_sSelectedBrush, loc.X - 1, loc.Y - 6, 4, 12);
                    } else {
                        context.FillRectangle(_sInOutBrush, loc.X - 1, loc.Y - 4, 4, 8);
                    }
                }
            }
            
            var format = new StringFormat {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            context.DrawString(Text, _sLabelFont, _sLabelBrush, Bounds, format);
        }

        public override string Serialize(Serialization.SerializationContext ctx)
        {
            return ctx.Obj(
                index => ctx.Int(Index),
                x => ctx.Int(Location.X),
                y => ctx.Int(Location.Y),
                @type => ctx.Str(IsInput ? "Input" : IsOutput ? "Output" : "Inner"),
                outputs => ctx.Arr(Enumerable.Range(0, OutputCount)
                    .Select(x => GetOutputs(x))
                    .Select(x => ctx.Arr(
                        x.Select(y =>
                            ctx.Obj(
                                index => ctx.Int(y.Index),
                                node => ctx.Int(((PlacedNode) y.Node).Index)
                            )
                        ).ToArray()
                    )).ToArray()
                ),
                data => _instance.Serialize(ctx)
            );
        }
    }

    public class InOutPlacedNode : PlacedNode
    {
        private bool _isInput;

        public override bool IsInput { get { return _isInput; } }

        public override bool IsOutput { get { return !_isInput; } }

        public String Identifier { get; set; }

        public override string Text { get { return Identifier; } }

        internal InOutPlacedNode(int index, String ident, bool isInput)
            : base(index, new SocketNode())
        {
            Identifier = ident;

            _isInput = isInput;
        }
    }
}

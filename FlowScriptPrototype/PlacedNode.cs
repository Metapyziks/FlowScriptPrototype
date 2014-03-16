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

        private static readonly Pen _sSelectedPen = new Pen(Color.FromArgb(0x26, 0x4F, 0x78));

        private static readonly Font _sLabelFont = new Font(FontFamily.GenericSansSerif, 12f);
        private static readonly Brush _sLabelBrush = new SolidBrush(Color.FromArgb(0xdc, 0xdc, 0xdc));

        private Node _instance;

        public Point Location { get; set; }

        public Size Size { get; private set; }

        public virtual bool IsInput { get { return false; } }

        public virtual bool IsOutput { get { return false; } }

        public Rectangle Bounds
        {
            get { return new Rectangle(Location, Size); }
        }

        public virtual String Text { get { return _instance.ToString(); } }

        internal PlacedNode(Node instance)
            : base(instance.InputCount, instance.OutputCount)
        {
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

        public void Draw(Graphics context, bool selected)
        {
            if (!IsInput) {
                for (int i = 0; i < InputCount; ++i) {
                    context.FillRectangle(_sInOutBrush, Bounds.Left - 2, Bounds.Top + 12 + 24 * i, 4, 8);
                }
            }

            if (!IsOutput) {
                for (int i = 0; i < OutputCount; ++i) {
                    context.FillRectangle(_sInOutBrush, Bounds.Right - 1, Bounds.Top + 12 + 24 * i, 4, 8);
                }
            }

            context.FillRectangle(_sFillBrush, Bounds);
            context.DrawRectangle(selected ? _sSelectedPen : _sOutlinePen, Bounds);
            
            var format = new StringFormat {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };

            context.DrawString(Text, _sLabelFont, _sLabelBrush, Bounds, format);
        }
    }

    public class InOutPlacedNode : PlacedNode
    {
        private bool _isInput;

        public override bool IsInput { get { return _isInput; } }

        public override bool IsOutput { get { return !_isInput; } }

        public String Identifier { get; set; }

        public override string Text { get { return Identifier; } }

        internal InOutPlacedNode(String ident, bool isInput)
            : base(new SocketNode())
        {
            Identifier = ident;

            _isInput = isInput;
        }
    }
}

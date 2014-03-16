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
        private static readonly Pen _sOutlinePen = new Pen(Color.FromArgb(0x68, 0x68, 0x68));
        private static readonly Brush _sFillBrush = new SolidBrush(Color.FromArgb(0x3e, 0x3e, 0x42));

        private Node _instance;

        public Point Location { get; set; }

        public Size Size { get; private set; }

        public Rectangle Bounds
        {
            get { return new Rectangle(Location, Size); }
        }

        public PlacedNode(Node instance)
            : base(instance.InputCount, instance.OutputCount)
        {
            _instance = instance;

            Location = new Point();
            Size = new Size(64, 8 + Math.Max(instance.InputCount, instance.OutputCount) * 24);
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

        public void Draw(Graphics context)
        {
            context.FillRectangle(_sFillBrush, Bounds);
            context.DrawRectangle(_sOutlinePen, Bounds);
        }
    }
}

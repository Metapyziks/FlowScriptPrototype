﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace FlowScriptPrototype
{
    class NativeNodeInfoAttribute : Attribute
    {
        public String Category { get; private set; }

        public String Identifier { get; private set; }

        public String Symbol { get; private set; }

        public NativeNodeInfoAttribute(String category, String identifier, String symbol = null)
        {
            Category = category;
            Identifier = identifier;

            Symbol = symbol ?? String.Format("{0}.{1}", Category, Identifier);
        }
    }

    abstract class NativeNode : Node
    {
        private HashSet<Socket>[] _outputs;

        protected NativeNode(int inputs, int outputs)
            : base(inputs, outputs)
        {
            _outputs = new HashSet<Socket>[outputs];

            for (int i = 0; i < outputs; ++i) {
                _outputs[i] = new HashSet<Socket>();
            }
        }

        protected void PulseOutput(int index, Signal signal)
        {
            foreach (var output in _outputs[index]) {
                output.PulseInput(signal);
            }
        }

        public override Node ConnectToInput(int index, Socket input)
        {
            _outputs[index].Add(input);
            return this;
        }

        public override Node ClearOutputs(int index)
        {
            _outputs[index].Clear();
            return this;
        }

        public override IEnumerable<Socket> GetOutputs(int index)
        {
            return _outputs[index];
        }

        internal override NodeSave GetSave()
        {
            var attrib = GetType().GetCustomAttribute<NativeNodeInfoAttribute>();

            if (attrib != null) {
                return new NodeSave { category = attrib.Category, identifier = attrib.Identifier };
            } else {
                return new NodeSave { category = "Unknown", identifier = "Unknown" };
            }
        }

        public override string ToString()
        {
            var attrib = GetType().GetCustomAttribute<NativeNodeInfoAttribute>();

            if (attrib != null) return attrib.Symbol;

            return GetType().Name;
        }
    }

    [NativeNodeInfo("Utils", "Socket")]
    class SocketNode : NativeNode
    {
        public Socket Input { get { return new Socket(this, 0); } }

        public Socket Output { get { return new Socket(this, 0); } }

        public SocketNode() : base(1, 1) { }

        public SocketNode ConnectToInput(Socket input)
        {
            ConnectToInput(0, input);
            return this;
        }

        public SocketNode ClearOutputs()
        {
            ClearOutputs(0);
            return this;
        }

        public IEnumerable<Socket> GetOutputs()
        {
            return GetOutputs(0);
        }

        public override void Pulse(params Signal[] inputs)
        {
            PulseOutput(0, inputs[0]);
        }

        public override Node Clone()
        {
            return new SocketNode();
        }
    }

    class ConstNode : SocketNode
    {
        public Signal Value { get; private set; }

        public ConstNode(Signal value)
        {
            Value = value;
        }

        public override void Pulse(params Signal[] inputs)
        {
            PulseOutput(0, Value);
        }

        public override Node Clone()
        {
            return new ConstNode(Value);
        }

        internal override NodeSave GetSave()
        {
            if (Value is StringSignal) {
                return new StringNodeSave { value = ((StringSignal) Value).Value };
            }

            if (Value is IntSignal) {
                return new IntNodeSave { value = ((IntSignal) Value).Value };
            }

            if (Value is RealSignal) {
                return new RealNodeSave { value = ((RealSignal) Value).Value };
            }

            return new NaNNodeSave();
        }

        public override string ToString()
        {
            return Value is StringSignal
                ? String.Format("\"{0}\"", Value.ToString())
                : Value.ToString();
        }
    }

    abstract class BinaryNode : NativeNode
    {
        public Socket InputA { get { return new Socket(this, 0); } }
        public Socket InputB { get { return new Socket(this, 1); } }

        public Socket Output { get { return new Socket(this, 0); } }

        protected BinaryNode() : base(2, 1) { }
    }

    [NativeNodeInfo("Math", "Add", "+")]
    class AddNode : BinaryNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            PulseOutput(0, inputs[0].Add(inputs[1]));
        }

        public override Node Clone()
        {
            return new AddNode();
        }
    }

    [NativeNodeInfo("Math", "Subtract", "-")]
    class SubtractNode : BinaryNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            PulseOutput(0, inputs[0].Subtract(inputs[1]));
        }

        public override Node Clone()
        {
            return new SubtractNode();
        }
    }

    [NativeNodeInfo("Math", "Multiply", "*")]
    class MultiplyNode : BinaryNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            PulseOutput(0, inputs[0].Multiply(inputs[1]));
        }

        public override Node Clone()
        {
            return new MultiplyNode();
        }
    }

    [NativeNodeInfo("Math", "Divide", "/")]
    class DivideNode : BinaryNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            PulseOutput(0, inputs[0].Divide(inputs[1]));
        }

        public override Node Clone()
        {
            return new DivideNode();
        }
    }

    [NativeNodeInfo("Math", "Modulo", "%")]
    class ModuloNode : BinaryNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            PulseOutput(0, inputs[0].Modulo(inputs[1]));
        }

        public override Node Clone()
        {
            return new ModuloNode();
        }
    }

    abstract class PredicateNode : NativeNode
    {
        public Socket OutputPositive { get { return new Socket(this, 0); } }
        public Socket OutputNegative { get { return new Socket(this, 1); } }

        protected PredicateNode(int inputs) : base(inputs, 2) { }

        protected abstract bool Evaluate(params Signal[] inputs);
        
        public override void Pulse(params Signal[] inputs)
        {
            int index = Evaluate(inputs) ? 0 : 1;
            PulseOutput(index, inputs[InputCount > 1 ? index : 0]);
        }
    }

    abstract class BinaryPredicateNode : PredicateNode
    {
        public Socket InputA { get { return new Socket(this, 0); } }
        public Socket InputB { get { return new Socket(this, 1); } }

        protected BinaryPredicateNode() : base(2) { }
    }

    [NativeNodeInfo("Math", "EqualTo", "==")]
    class EqualToNode : BinaryPredicateNode
    {
        protected override bool Evaluate(params Signal[] inputs)
        {
            return inputs[0].EqualTo(inputs[1]);
        }

        public override Node Clone()
        {
            return new EqualToNode();
        }
    }

    [NativeNodeInfo("Math", "GreaterThan", ">")]
    class GreaterThanNode : BinaryPredicateNode
    {
        protected override bool Evaluate(params Signal[] inputs)
        {
            return inputs[0].GreaterThan(inputs[1]);
        }

        public override Node Clone()
        {
            return new GreaterThanNode();
        }
    }

    [NativeNodeInfo("Math", "GreaterThanOrEqual", ">=")]
    class GreaterThanOrEqualToNode : BinaryPredicateNode
    {
        protected override bool Evaluate(params Signal[] inputs)
        {
            return !inputs[0].GreaterThan(inputs[1]) || inputs[0].EqualTo(inputs[1]);
        }

        public override Node Clone()
        {
            return new GreaterThanOrEqualToNode();
        }
    }

    [NativeNodeInfo("Math", "LessThan", "<")]
    class LessThanNode : BinaryPredicateNode
    {
        protected override bool Evaluate(params Signal[] inputs)
        {
            return inputs[0].LessThan(inputs[1]);
        }

        public override Node Clone()
        {
            return new LessThanNode();
        }
    }

    [NativeNodeInfo("Math", "LessThanOrEqual", "<=")]
    class LessThanOrEqualToNode : BinaryPredicateNode
    {
        protected override bool Evaluate(params Signal[] inputs)
        {
            return inputs[0].LessThan(inputs[1]) || inputs[0].EqualTo(inputs[1]);
        }

        public override Node Clone()
        {
            return new LessThanOrEqualToNode();
        }
    }

    [NativeNodeInfo("Math", "Sqrt")]
    class SqrtNode : SocketNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            double val;

            if (inputs[0] is IntSignal) {
                val = ((IntSignal) inputs[0]).Value;
            } else if (inputs[0] is RealSignal) {
                val = ((RealSignal) inputs[0]).Value;
            } else {
                base.Pulse(new NaNSignal());
                return;
            }

            if (val < 0) {
                base.Pulse(new NaNSignal());
            } else {
                base.Pulse(new RealSignal(Math.Sqrt(val)));
            }
        }

        public override Node Clone()
        {
            return new SqrtNode();
        }
    }

    [NativeNodeInfo("Math", "Pow")]
    class PowNode : BinaryNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            double a, b;

            if (inputs[0] is IntSignal) {
                a = ((IntSignal) inputs[0]).Value;
            } else if (inputs[0] is RealSignal) {
                a = ((RealSignal) inputs[0]).Value;
            } else {
                PulseOutput(0, new NaNSignal());
                return;
            }

            if (inputs[1] is IntSignal) {
                b = ((IntSignal) inputs[1]).Value;
            } else if (inputs[1] is RealSignal) {
                b = ((RealSignal) inputs[1]).Value;
            } else {
                PulseOutput(0, new NaNSignal());
                return;
            }

            if ((a == 0 && b == 0) || (a < 0 && b != Math.Round(b))) {
                PulseOutput(0, new NaNSignal());
            } else if (inputs[0] is IntSignal && inputs[1] is IntSignal) {
                PulseOutput(0, new IntSignal((long) Math.Pow(a, b)));
            }
        }

        public override Node Clone()
        {
            return new PowNode();
        }
    }

    [NativeNodeInfo("Console", "ReadLine")]
    class ReadLineNode : SocketNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            base.Pulse(new StringSignal(Console.ReadLine()));
        }

        public override Node Clone()
        {
            return new ReadLineNode();
        }
    }

    [NativeNodeInfo("Console", "ReadKey")]
    class ReadKeyNode : SocketNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            base.Pulse(new StringSignal(Console.ReadKey(true).Key.ToString()));
        }

        public override Node Clone()
        {
            return new ReadKeyNode();
        }
    }

    [NativeNodeInfo("Console", "Clear")]
    class ClearNode : SocketNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            Console.Clear();

            base.Pulse(inputs);
        }

        public override Node Clone()
        {
            return new ClearNode();
        }
    }

    [NativeNodeInfo("Console", "Print")]
    class PrintNode : SocketNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            Console.Write(inputs[0].ToString());

            base.Pulse(inputs);
        }

        public override Node Clone()
        {
            return new PrintNode();
        }
    }

    [NativeNodeInfo("Console", "PrintLine")]
    class PrintLineNode : SocketNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            Console.WriteLine(inputs[0].ToString());

            base.Pulse(inputs);
        }

        public override Node Clone()
        {
            return new PrintLineNode();
        }
    }

    [NativeNodeInfo("Utils", "ParseInt")]
    class ParseIntNode : SocketNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            var str = inputs[0].ToString();
            long val;

            if (long.TryParse(str, out val)) {
                base.Pulse(new IntSignal(val));
            } else {
                base.Pulse(new NaNSignal());
            }
        }

        public override Node Clone()
        {
            return new ParseIntNode();
        }
    }

    [NativeNodeInfo("Utils", "ToString")]
    class ToStringNode : SocketNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            base.Pulse(new StringSignal(inputs[0].ToString()));
        }

        public override Node Clone()
        {
            return new ToStringNode();
        }
    }
}

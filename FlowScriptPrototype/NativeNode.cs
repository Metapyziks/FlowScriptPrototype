using System;
using System.Collections.Generic;

namespace FlowScriptPrototype
{
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
    }

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
    }

    abstract class BinaryNode : NativeNode
    {
        public Socket InputA { get { return new Socket(this, 0); } }
        public Socket InputB { get { return new Socket(this, 1); } }

        public Socket Output { get { return new Socket(this, 0); } }

        protected BinaryNode() : base(2, 1) { }
    }

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

    class GreaterThanOrEqualToNode : BinaryPredicateNode
    {
        protected override bool Evaluate(params Signal[] inputs)
        {
            return !inputs[0].LessThan(inputs[1]);
        }

        public override Node Clone()
        {
            return new GreaterThanOrEqualToNode();
        }
    }

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

    class LessThanOrEqualToNode : BinaryPredicateNode
    {
        protected override bool Evaluate(params Signal[] inputs)
        {
            return !inputs[0].GreaterThan(inputs[1]);
        }

        public override Node Clone()
        {
            return new LessThanOrEqualToNode();
        }
    }

    class PrintNode : SocketNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            Console.WriteLine(inputs[0].ToString());

            base.Pulse(inputs);
        }

        public override Node Clone()
        {
            return new PrintNode();
        }
    }
}

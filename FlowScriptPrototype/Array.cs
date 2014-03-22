using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowScriptPrototype.Array
{
    public class ArraySignal : NaNSignal
    {
        public List<Signal> Value { get; private set; }

        public ArraySignal()
        {
            Value = new List<Signal>();
        }

        public ArraySignal(ArraySignal clone)
        {
            Value = clone.Value;
        }

        public override bool EqualTo(Signal other)
        {
            return other is ArraySignal && ((ArraySignal) other).Value == Value;
        }

        public override string ToString()
        {
            return String.Format("[Array {0:x}]", Value.GetHashCode());
        }
    }


    [NativeNodeInfo("Array", "New")]
    class NewNode : SocketNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            PulseOutput(0, new ArraySignal());
        }

        public override Node Clone()
        {
            return new NewNode();
        }
    }

    [NativeNodeInfo("Array", "Length")]
    class LengthNode : SocketNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            var input = inputs[0] as ArraySignal;

            if (input == null) return;

            PulseOutput(0, new IntSignal(input.Value.Count));
        }

        public override Node Clone()
        {
            return new LengthNode();
        }
    }

    [NativeNodeInfo("Array", "Index", "[]")]
    class IndexNode : BinaryNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            var input = inputs[0] as ArraySignal;
            var index = inputs[1] as IntSignal;

            if (input == null || index == null) return;

            if (index.Value < 0 || index.Value >= input.Value.Count) return;
            
            PulseOutput(0, input.Value[(int) index.Value]);
        }

        public override Node Clone()
        {
            return new IndexNode();
        }
    }

    [NativeNodeInfo("Array", "Insert")]
    class InsertNode : NativeNode
    {
        public InsertNode() : base(3, 1) { }

        public override void Pulse(params Signal[] inputs)
        {
            var input = inputs[0] as ArraySignal;
            var index = inputs[1] as IntSignal;
            var value = inputs[2];

            if (input == null || index == null) return;

            if (index.Value < 0 || index.Value > input.Value.Count) return;

            input.Value.Insert((int) index.Value, value);

            PulseOutput(0, input);
        }

        public override Node Clone()
        {
            return new InsertNode();
        }
    }

    [NativeNodeInfo("Array", "Contains")]
    class ContainsNode : BinaryPredicateNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            var input = inputs[0] as ArraySignal;
            var value = inputs[1];

            if (input == null) return;

            foreach (var val in input.Value) {
                if (val.EqualTo(value)) {
                    PulseOutput(0, value);
                    return;
                }
            }

            PulseOutput(1, value);
        }

        protected override bool Evaluate(params Signal[] inputs)
        {
            throw new NotImplementedException();
        }

        public override Node Clone()
        {
            return new ContainsNode();
        }
    }

    [NativeNodeInfo("Array", "IndexOf")]
    class IndexOfNode : BinaryNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            var input = inputs[0] as ArraySignal;
            var value = inputs[1];

            if (input == null) return;

            int index = 0;
            foreach (var val in input.Value) {
                if (val.EqualTo(value)) {
                    PulseOutput(0, new IntSignal(index));
                    return;
                }
            }
        }

        public override Node Clone()
        {
            return new IndexOfNode();
        }
    }

    [NativeNodeInfo("Array", "Push")]
    class PushNode : BinaryNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            var input = inputs[0] as ArraySignal;

            if (input == null) return;

            input.Value.Add(inputs[1]);

            PulseOutput(0, input);
        }

        public override Node Clone()
        {
            return new PushNode();
        }
    }

    [NativeNodeInfo("Array", "Pop")]
    class PopNode : NativeNode
    {
        public PopNode() : base(1, 2) { }

        public override void Pulse(params Signal[] inputs)
        {
            var input = inputs[0] as ArraySignal;

            if (input == null) return;

            if (input.Value.Count == 0) return;

            var last = input.Value.Last();
            input.Value.RemoveAt(input.Value.Count);

            PulseOutput(0, input);
            PulseOutput(1, last);
        }

        public override Node Clone()
        {
            return new PopNode();
        }
    }

    [NativeNodeInfo("Array", "Unshift")]
    class UnshiftNode : BinaryNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            var input = inputs[0] as ArraySignal;

            if (input == null) return;

            input.Value.Insert(0, inputs[1]);

            PulseOutput(0, input);
        }

        public override Node Clone()
        {
            return new UnshiftNode();
        }
    }

    [NativeNodeInfo("Array", "Shift")]
    class ShiftNode : NativeNode
    {
        public ShiftNode() : base(1, 2) { }

        public override void Pulse(params Signal[] inputs)
        {
            var input = inputs[0] as ArraySignal;

            if (input == null) return;

            if (input.Value.Count == 0) return;

            var first = input.Value.First();
            input.Value.RemoveAt(0);

            PulseOutput(0, input);
            PulseOutput(1, first);
        }

        public override Node Clone()
        {
            return new ShiftNode();
        }
    }

    [NativeNodeInfo("Array", "Join")]
    class JoinNode : BinaryNode
    {
        public override void Pulse(params Signal[] inputs)
        {
            var input = inputs[0] as ArraySignal;
            var separator = inputs[1].ToString();

            if (input == null) return;

            PulseOutput(0, new StringSignal(String.Join(separator, input.Value)));
        }

        public override Node Clone()
        {
            return new JoinNode();
        }
    }
}

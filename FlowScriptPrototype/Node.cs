using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowScriptPrototype
{
    struct Socket
    {
        public readonly Node Node;
        public readonly int Index;

        public Socket(Node node, int index)
        {
            Node = node;
            Index = index;
        }

        public Socket ConnectToInput(Socket input)
        {
            Node.ConnectToInput(Index, input);

            return this;
        }

        public Socket ClearOutputs()
        {
            Node.ClearOutputs(Index);

            return this;
        }

        public IEnumerable<Socket> GetOutputs()
        {
            return Node.GetOutputs(Index);
        }

        public void PulseInput(Signal signal)
        {
            Node.PulseInput(Index, signal);
        }
    }

    abstract class Node
    {
        private static List<Node> _sToPulse = new List<Node>();

        public static bool Step()
        {
            var pulsing = _sToPulse.ToArray();
            _sToPulse.Clear();

            var inputs = new Signal[pulsing.Length][];

            for (int i = 0; i < pulsing.Length; ++i) {
                var node = pulsing[i];
                node._toPulse = false;
                inputs[i] = (Signal[]) node._inputs.Clone();
                Array.Clear(node._inputs, 0, node.InputCount);
            }

            for (int i = 0; i < pulsing.Length; ++i) {
                pulsing[i].Pulse(inputs[i]);
            }

            return _sToPulse.Count > 0;
        }

        private Signal[] _inputs;
        protected bool _toPulse;

        public virtual bool Active { get { return _toPulse; } }

        public int InputCount { get; private set; }
        public int OutputCount { get; private set; }

        public Node(int inputs, int outputs)
        {
            InputCount = inputs;
            OutputCount = outputs;

            _inputs = new Signal[inputs];

            _toPulse = false;
        }

        public abstract Node ConnectToInput(int index, Socket input);

        public abstract Node ClearOutputs(int index);

        public abstract IEnumerable<Socket> GetOutputs(int index);

        public abstract void Pulse(params Signal[] inputs);

        public void PulseInput(int index, Signal signal)
        {
            _inputs[index] = signal;

            if (_inputs.All(x => x != null) && !_toPulse) {
                _toPulse = true;
                _sToPulse.Add(this);
            }
        }

        public abstract Node Clone();
    }
}

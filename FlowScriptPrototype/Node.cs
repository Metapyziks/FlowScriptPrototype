using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FlowScriptPrototype
{
    public struct Socket
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

        public Socket ConnectToInput(Node node, int index)
        {
            return ConnectToInput(new Socket(node, index));
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

    public abstract class Node
    {
        private static List<Node> _sToPulse = new List<Node>();

        private static Dictionary<String, Dictionary<String, NativeNode>> _sPrototypes
            = new Dictionary<string, Dictionary<string, NativeNode>>();

        static Node()
        {
            FindNativeNodeDefinitions(Assembly.GetExecutingAssembly());
        }

        public static void FindNativeNodeDefinitions(Assembly assembly)
        {
            var nodeType = typeof(NativeNode);

            foreach (var type in assembly.GetTypes()) {
                if (!nodeType.IsAssignableFrom(type)) continue;

                var attrib = type.GetCustomAttribute<NativeNodeInfoAttribute>();
                if (attrib == null) continue;

                if (!_sPrototypes.ContainsKey(attrib.Category)) {
                    _sPrototypes.Add(attrib.Category, new Dictionary<string, NativeNode>());
                }

                var ctor = type.GetConstructor(new Type[0]);

                if (ctor == null) continue;

                _sPrototypes[attrib.Category].Add(attrib.Identifier, (NativeNode) ctor.Invoke(new Object[0]));
            }
        }

        public static IEnumerable<String> Categories
        {
            get { return _sPrototypes.Keys.Union(CustomNode.CustomCategories); }
        }

        public static IEnumerable<String> GetIdentifiers(String category)
        {
            return _sPrototypes.ContainsKey(category)
                ? _sPrototypes[category].Keys.Union(CustomNode.GetCustomIdentifiers(category))
                : CustomNode.GetCustomIdentifiers(category);
        }

        public static Node GetInstance(String category, String identifier)
        {
            if (CustomNode.GetCustomIdentifiers(category).Contains(identifier)) {
                return CustomNode.GetCustomReference(category, identifier);
            } else {
                return _sPrototypes[category][identifier].Clone();
            }
        }

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

            CustomNode.CollectGarbage();

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

        public Node ConnectToInput(int outputIndex, Node node, int inputIndex)
        {
            return ConnectToInput(outputIndex, new Socket(node, inputIndex));
        }

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

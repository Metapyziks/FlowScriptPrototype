using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowScriptPrototype
{
    abstract class CustomNode : Node
    {
        private static Dictionary<String, Dictionary<String, PrototypeNode>> _sPrototypes =
            new Dictionary<string, Dictionary<string, PrototypeNode>>();

        private static Dictionary<PrototypeNode, Stack<PrototypeNode>> _sRecycledInstances =
            new Dictionary<PrototypeNode, Stack<PrototypeNode>>();

        private static List<ReferenceNode> _sWatchedReferences = new List<ReferenceNode>();
        
        public static void CreatePrototype(String category, String identifier, int inputs, int outputs, Action<PrototypeNode> constructor)
        {
            if (!_sPrototypes.ContainsKey(category)) {
                _sPrototypes.Add(category, new Dictionary<string, PrototypeNode>());
            }

            var node = new PrototypeNode(category, identifier, inputs, outputs);
            _sPrototypes[category].Add(identifier, node);

            _sRecycledInstances.Add(node, new Stack<PrototypeNode>());

            constructor(node);
        }

        public static void CollectGarbage()
        {
            var inactive = _sWatchedReferences.Where(x => !x.Active).ToArray();

            foreach (var reference in inactive) {
                _sRecycledInstances[reference.Prototype].Push(reference.Recycle());
                _sWatchedReferences.Remove(reference);
            }
        }

        protected static void WatchReference(ReferenceNode node)
        {
            _sWatchedReferences.Add(node);
        }

        public static IEnumerable<String> CustomCategories { get { return _sPrototypes.Keys; } }

        public static IEnumerable<String> GetCustomIdentifiers(String category)
        {
            return _sPrototypes.ContainsKey(category) ? _sPrototypes[category].Keys : Enumerable.Empty<String>();
        }

        public static ReferenceNode GetCustomReference(String category, String identifier)
        {
            return new ReferenceNode(_sPrototypes[category][identifier]);
        }

        public static PrototypeNode GetCustomInstance(PrototypeNode prototype)
        {
            var recycled = _sRecycledInstances[prototype];

            if (recycled.Count > 0) {
                return recycled.Pop();
            } else {
                return (PrototypeNode) prototype.Clone();
            }
        }

        protected CustomNode(int inputs, int outputs)
            : base(inputs, outputs) { }
    }

    class ReferenceNode : CustomNode
    {
        private PrototypeNode _instance;
        private HashSet<Socket>[] _outputs;

        public PrototypeNode Prototype { get; private set; }

        public override bool Active { get { return base.Active || (_instance != null && _instance.Active); } }

        internal ReferenceNode(PrototypeNode prototype)
            : base(prototype.InputCount, prototype.OutputCount)
        {
            Prototype = prototype;
            _instance = null;

            _outputs = new HashSet<Socket>[prototype.OutputCount];

            for (int i = 0; i < prototype.OutputCount; ++i) {
                _outputs[i] = new HashSet<Socket>();
            }
        }

        internal PrototypeNode Recycle()
        {
            for (int i = 0; i < OutputCount; ++i) {
                _instance.GetOutput(i).ClearOutputs();
            }

            var instance = _instance;
            _instance = null;

            return instance;
        }

        public override void Pulse(params Signal[] inputs)
        {
            bool newRef = _instance == null;
            if (newRef) {
                _instance = GetCustomInstance(Prototype);

                for (int i = 0; i < OutputCount; ++i) {
                    foreach (var socket in GetOutputs(i)) {
                        _instance.ConnectToInput(i, socket);
                    }
                }
            }

            _instance.Pulse(inputs);

            if (newRef) {
                WatchReference(this);
            }
        }

        public override Node ConnectToInput(int index, Socket input)
        {
            _outputs[index].Add(input);

            return this;
        }

        public override IEnumerable<Socket> GetOutputs(int index)
        {
            return _outputs[index];
        }

        public override Node ClearOutputs(int index)
        {
            throw new NotImplementedException();
        }

        public override Node Clone()
        {
            return new ReferenceNode(Prototype);
        }

        public override string ToString()
        {
            return Prototype.ToString();
        }
    }

    class PrototypeNode : CustomNode
    {
        private List<Node> _inner;

        private SocketNode[] _inputs;
        private SocketNode[] _outputs;

        public String Category { get; private set; }

        public String Identifier { get; private set; }

        public override bool Active { get { return base.Active || _inner.Any(x => x.Active) || _outputs.Any(x => x.Active); } }

        private PrototypeNode(PrototypeNode clone)
            : base(clone.InputCount, clone.OutputCount)
        {
            Category = clone.Category;
            Identifier = clone.Identifier;

            _inner = clone._inner
                .Select(x => x.Clone())
                .ToList();

            SetupSockets();

            Action<Node, Node> copyOutputs = (node, orig) => {
                for (int j = 0; j < node.OutputCount; ++j) {
                    foreach (var socket in orig.GetOutputs(j)) {
                        var index = clone._inner.IndexOf(socket.Node);

                        if (index != -1) {
                            node.ConnectToInput(j, new Socket(_inner[index], socket.Index));
                            continue;
                        }

                        index = Array.IndexOf(clone._outputs, socket.Node);

                        node.ConnectToInput(j, new Socket(_outputs[index], socket.Index));
                    }
                }
            };

            for (int i = 0; i < InputCount; ++i) {
                copyOutputs(_inputs[i], clone._inputs[i]);
            }

            for (int i = 0; i < _inner.Count; ++i) {
                copyOutputs(_inner[i], clone._inner[i]);
            }
        }

        internal PrototypeNode(String category, String identifier, int inputs, int outputs)
            : base(inputs, outputs)
        {
            _inner = new List<Node>();

            Category = category;
            Identifier = identifier;

            SetupSockets();
        }

        private void SetupSockets()
        {
            _inputs = new SocketNode[InputCount];
            _outputs = new SocketNode[OutputCount];

            for (int i = 0; i < InputCount; ++i) {
                _inputs[i] = new SocketNode();
            }

            for (int i = 0; i < OutputCount; ++i) {
                _outputs[i] = new SocketNode();
            }
        }

        public Socket GetInput(int index)
        {
            return _inputs[index].Output;
        }

        public Socket GetOutput(int index)
        {
            return _outputs[index].Input;
        }

        public override IEnumerable<Socket> GetOutputs(int index)
        {
            return _outputs[index].GetOutputs();
        }

        public override Node ClearOutputs(int index)
        {
            _outputs[index].ClearOutputs(0);
            return this;
        }

        public ConstNode AddConstant(Signal constant)
        {
            var node = new ConstNode(constant);
            _inner.Add(node);

            return node;
        }

        public Node AddNode(String category, String identifier)
        {
            var node = Node.GetInstance(category, identifier);
            _inner.Add(node);

            return node;
        }

        public override void Pulse(params Signal[] inputs)
        {
            for (int i = 0; i < InputCount; ++i) {
                _inputs[i].Pulse(inputs[i]);
            }
        }

        public override Node Clone()
        {
            return new PrototypeNode(this);
        }

        public override Node ConnectToInput(int index, Socket input)
        {
            _outputs[index].ConnectToInput(input);
            return this;
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}", Category, Identifier);
        }
    }
}

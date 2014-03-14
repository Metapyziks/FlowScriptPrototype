using System;
using System.Collections.Generic;
using System.Linq;

namespace FlowScriptPrototype
{
    abstract class CustomNode : Node
    {
        private static Dictionary<String, Dictionary<String, PrototypeNode>> _prototypes =
            new Dictionary<string, Dictionary<string, PrototypeNode>>();

        public static void CreatePrototype(String category, String identifier, int inputs, int outputs, Action<PrototypeNode> constructor)
        {
            if (!_prototypes.ContainsKey(category)) {
                _prototypes.Add(category, new Dictionary<string, PrototypeNode>());
            }

            var node = new PrototypeNode(inputs, outputs);
            _prototypes[category].Add(identifier, node);

            constructor(node);
        }

        public static IEnumerable<String> Categories { get { return _prototypes.Keys; } }

        public static IEnumerable<String> GetIdentifiers(String category)
        {
            return _prototypes[category].Keys;
        }

        public static ReferenceNode GetInstance(String category, String identifier)
        {
            return new ReferenceNode(_prototypes[category][identifier]);
        }

        protected CustomNode(int inputs, int outputs)
            : base(inputs, outputs) { }
    }

    class ReferenceNode : CustomNode
    {
        private PrototypeNode _prototype;
        private PrototypeNode _instance;

        private HashSet<Socket>[] _outputs;

        public override bool Active { get { return _instance != null && _instance.Active; } }

        internal ReferenceNode(PrototypeNode prototype)
            : base(prototype.InputCount, prototype.OutputCount)
        {
            _prototype = prototype;
            _instance = null;

            _outputs = new HashSet<Socket>[prototype.OutputCount];

            for (int i = 0; i < prototype.OutputCount; ++i) {
                _outputs[i] = new HashSet<Socket>();
            }
        }

        public override void Pulse(params Signal[] inputs)
        {
            if (_instance == null) {
                _instance = (PrototypeNode) _prototype.Clone();

                for (int i = 0; i < OutputCount; ++i) {
                    _instance.GetOutput(i).ClearOutputs();

                    foreach (var socket in GetOutputs(i)) {
                        _instance.ConnectToInput(i, socket);
                    }
                }
            }

            _instance.Pulse(inputs);
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
            return new ReferenceNode(_prototype);
        }
    }

    class PrototypeNode : CustomNode
    {
        private List<Node> _inner;

        private SocketNode[] _inputs;
        private SocketNode[] _outputs;

        public override bool Active { get { return base.Active || _inner.Any(x => x.Active); } }

        private PrototypeNode(PrototypeNode clone)
            : base(clone.InputCount, clone.OutputCount)
        {
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

        internal PrototypeNode(int inputs, int outputs)
            : base(inputs, outputs)
        {
            _inner = new List<Node>();

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

        public T Add<T>(T node)
            where T : Node
        {
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
    }
}

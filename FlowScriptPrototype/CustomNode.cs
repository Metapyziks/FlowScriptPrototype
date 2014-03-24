using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlowScriptPrototype
{
    public abstract class CustomNode : Node
    {
        private static Dictionary<String, Dictionary<String, PrototypeNode>> _sPrototypes =
            new Dictionary<string, Dictionary<string, PrototypeNode>>();

        private static Dictionary<PrototypeNode, Stack<PrototypeNode>> _sRecycledInstances =
            new Dictionary<PrototypeNode, Stack<PrototypeNode>>();

        private static List<ReferenceNode> _sWatchedReferences = new List<ReferenceNode>();

        static CustomNode()
        {
            if (!Directory.Exists("Data")) return;

            GetDeclarations("Data");
            GetDefinitions("Data");
        }

        static void GetDeclarations(String dir)
        {
            foreach (var file in Directory.GetFiles(dir)) {
                if (Path.GetExtension(file) != ".json") continue;

                DeclarePrototype(PrototypeNodeSave.FromFile(file));
            }

            foreach (var subDir in Directory.GetDirectories(dir)) {
                GetDeclarations(subDir);
            }
        }

        static void GetDefinitions(String dir)
        {
            foreach (var file in Directory.GetFiles(dir)) {
                if (Path.GetExtension(file) != ".json") continue;

                DefinePrototype(PrototypeNodeSave.FromFile(file));
            }

            foreach (var subDir in Directory.GetDirectories(dir)) {
                GetDefinitions(subDir);
            }
        }
        
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

        static void ConnectInputs<T>(PrototypeNode node, PlacedNodeSave<T>[] inners)
            where T : NodeSave
        {
            foreach (var inner in inners) {
                var inst = node.GetNode(inner.index);
                for (int i = 0; i < inner.outputs.Length; ++i) {
                    foreach (var socket in inner.outputs[i]) {
                        inst.ConnectToInput(i, new Socket(node.GetNode(socket.node), socket.socket));
                    }
                }
            }
        }

        static void DeclarePrototype(PrototypeNodeSave save)
        {
            CreatePrototype(save.category, save.identifier, save.inputs.Length, save.outputs.Length, node => { });
        }

        static void DefinePrototype(PrototypeNodeSave save)
        {
            var node = GetCustomReference(save.category, save.identifier).Prototype;

            node.EditorSize = new System.Drawing.Size(save.width, save.height);

            foreach (var input in save.inputs) {
                ((PlacedNode) node.GetInput(input.index).Node).Location =
                    new System.Drawing.Point(input.x, input.y);
            }

            foreach (var output in save.outputs) {
                ((PlacedNode) node.GetOutput(output.index - node.InputCount).Node).Location =
                    new System.Drawing.Point(output.x, output.y);
            }

            foreach (var inner in save.inners) {
                node.AddNode(inner);
            }

            foreach (var constant in save.ints) {
                node.AddConstant(constant);
            }

            foreach (var constant in save.reals) {
                node.AddConstant(constant);
            }

            foreach (var constant in save.strings) {
                node.AddConstant(constant);
            }

            foreach (var constant in save.nans) {
                node.AddConstant(constant);
            }

            ConnectInputs(node, save.inputs);
            ConnectInputs(node, save.inners);
            ConnectInputs(node, save.ints);
            ConnectInputs(node, save.reals);
            ConnectInputs(node, save.strings);
            ConnectInputs(node, save.nans);
        }

        public static void CreateCategory(String category)
        {
            _sPrototypes.Add(category, new Dictionary<string, PrototypeNode>());
        }

        internal static void ClearRecycledInstances(PrototypeNode prototype)
        {
            if (_sRecycledInstances.ContainsKey(prototype)) {
                _sRecycledInstances[prototype].Clear();
            }
        }

        public static void CollectGarbage()
        {
            var inactive = _sWatchedReferences.Where(x => !x.Active).ToArray();

            foreach (var reference in inactive) {
                _sRecycledInstances[reference.Prototype].Push(reference.Recycle());
                _sWatchedReferences.Remove(reference);
            }
        }

        internal static void WatchReference(ReferenceNode node)
        {
            _sWatchedReferences.Add(node);
        }

        internal static IEnumerable<String> CustomCategories { get { return _sPrototypes.Keys; } }

        internal static IEnumerable<String> GetCustomIdentifiers(String category)
        {
            return _sPrototypes.ContainsKey(category) ? _sPrototypes[category].Keys : Enumerable.Empty<String>();
        }

        internal static ReferenceNode GetCustomReference(String category, String identifier)
        {
            return new ReferenceNode(_sPrototypes[category][identifier]);
        }

        internal static PrototypeNode GetCustomInstance(PrototypeNode prototype)
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

    internal class ReferenceNode : CustomNode
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
            _outputs[index].Clear();

            if (_instance != null) {
                _instance.ClearOutputs(index);
            }

            return this;
        }

        public override Node Clone()
        {
            return new ReferenceNode(Prototype);
        }

        internal override NodeSave GetSave()
        {
            return new NodeSave { category = Prototype.Category, identifier = Prototype.Identifier };
        }

        public override string ToString()
        {
            return Prototype.ToString();
        }
    }

    public class PrototypeNode : CustomNode
    {
        private int _nextIndex;

        private List<Node> _inner;

        private Node[] _inputs;
        private Node[] _outputs;

        public IEnumerable<PlacedNode> Nodes
        {
            get
            {
                return _inner.OfType<PlacedNode>()
                    .Union(_inputs.Cast<PlacedNode>())
                    .Union(_outputs.Cast<PlacedNode>());
            }
        }

        public String Category { get; private set; }

        public String Identifier { get; private set; }

        public System.Drawing.Size EditorSize { get; set; }

        public override bool Active { get { return base.Active || _inner.Any(x => x.Active) || _outputs.Any(x => x.Active); } }

        private PrototypeNode(PrototypeNode clone)
            : base(clone.InputCount, clone.OutputCount)
        {
            Category = clone.Category;
            Identifier = clone.Identifier;

            _inner = clone._inner
                .Select(x => x.Clone())
                .ToList();

            _inputs = new SocketNode[InputCount];
            _outputs = new SocketNode[OutputCount];

            for (int i = 0; i < InputCount; ++i) {
                _inputs[i] = new SocketNode();
            }

            for (int i = 0; i < OutputCount; ++i) {
                _outputs[i] = new SocketNode();
            }

            Action<Node, Node> copyOutputs = (node, orig) => {
                for (int j = 0; j < node.OutputCount; ++j) {
                    foreach (var socket in orig.GetOutputs(j)) {
                        var index = clone._inner.IndexOf(socket.Node);

                        if (index != -1) {
                            node.ConnectToInput(j, new Socket(_inner[index], socket.Index));
                            continue;
                        }

                        index = System.Array.IndexOf(clone._outputs, socket.Node);

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

            _nextIndex = 0;

            Category = category;
            Identifier = identifier;

            _inputs = new PlacedNode[InputCount];
            _outputs = new PlacedNode[OutputCount];

            for (int i = 0; i < InputCount; ++i) {
                var input = new InOutPlacedNode(_nextIndex++, String.Format("In {0}", i + 1), true);
                input.Location = new System.Drawing.Point(8, 8 + i * (input.Size.Height + 8));
                _inputs[i] = input;
            }

            for (int i = 0; i < OutputCount; ++i) {
                var output = new InOutPlacedNode(_nextIndex++, String.Format("Out {0}", i + 1), false);
                output.Location = new System.Drawing.Point(16 + output.Size.Width, 8 + i * (output.Size.Height + 8));
                _outputs[i] = output;
            }

            EditorSize = new System.Drawing.Size(640, 480);
        }

        public void ClearRecycledInstances()
        {
            CustomNode.ClearRecycledInstances(this);
        }

        public Socket GetInput(int index)
        {
            return new Socket(_inputs[index], 0);
        }

        public Socket GetOutput(int index)
        {
            return new Socket(_outputs[index], 0);
        }

        internal PlacedNode GetNode(int index)
        {
            if (index < InputCount) {
                return (PlacedNode) GetInput(index).Node;
            } else if (index - InputCount < OutputCount) {
                return (PlacedNode) GetOutput(index - InputCount).Node;
            } else {
                return _inner.Cast<PlacedNode>().First(x => x.Index == index);
            }
        }

        public override IEnumerable<Socket> GetOutputs(int index)
        {
            return _outputs[index].GetOutputs(0);
        }

        public override Node ClearOutputs(int index)
        {
            _outputs[index].ClearOutputs(0);
            return this;
        }

        internal PlacedNode AddConstant<T>(PlacedNodeSave<T> save, Signal value)
            where T : NodeSave
        {
            var node = new PlacedNode(save.index, new ConstNode(value)) {
                Location = new System.Drawing.Point(save.x, save.y)
            };

            _inner.Add(node);

            _nextIndex = Math.Max(_nextIndex, save.index + 1);

            return node;
        }

        internal PlacedNode AddConstant(PlacedNodeSave<IntNodeSave> save)
        {
            return AddConstant(save, new IntSignal(save.data.value));
        }

        internal PlacedNode AddConstant(PlacedNodeSave<RealNodeSave> save)
        {
            return AddConstant(save, new RealSignal(save.data.value));
        }

        internal PlacedNode AddConstant(PlacedNodeSave<StringNodeSave> save)
        {
            return AddConstant(save, new StringSignal(save.data.value));
        }

        internal PlacedNode AddConstant(PlacedNodeSave<NaNNodeSave> save)
        {
            return AddConstant(save, new NaNSignal());
        }

        public PlacedNode AddConstant(Signal constant)
        {
            var node = new PlacedNode(_nextIndex++, new ConstNode(constant));
            _inner.Add(node);

            return node;
        }

        internal PlacedNode AddNode(PlacedNodeSave<NodeSave> save)
        {
            var node = new PlacedNode(save.index, Node.GetInstance(save.data.category, save.data.identifier)) {
                Location = new System.Drawing.Point(save.x, save.y)
            };

            _inner.Add(node);

            _nextIndex = Math.Max(_nextIndex, save.index + 1);

            return node;
        }

        public PlacedNode AddNode(String category, String identifier)
        {
            var node = new PlacedNode(_nextIndex++, Node.GetInstance(category, identifier));
            _inner.Add(node);

            return node;
        }

        public void RemoveNode(PlacedNode node)
        {
            _inner.Remove(node);
            
            foreach (var other in _inner.Union(_inputs)) {
                for (int i = 0; i < other.OutputCount; ++i) {
                    var outputs = other.GetOutputs(i).ToArray();

                    if (outputs.Any(x => x.Node == node)) {
                        other.ClearOutputs(i);

                        foreach (var output in outputs) {
                            if (output.Node == node) continue;
                            other.ConnectToInput(i, output);
                        }
                    }
                }
            }
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
            _outputs[index].ConnectToInput(0, input);
            return this;
        }

        internal override NodeSave GetSave()
        {
            var inner = _inner.Cast<PlacedNode>().ToArray();
            var consts = inner.Where(x => x.Instance is ConstNode);

            return new PrototypeNodeSave {
                category = Category,
                identifier = Identifier,
                width = EditorSize.Width,
                height = EditorSize.Height,
                inputs = _inputs
                    .Cast<PlacedNode>()
                    .Select(x => x.GetPlacedNodeSave<NodeSave>())
                    .ToArray(),
                outputs = _outputs
                    .Cast<PlacedNode>()
                    .Select(x => x.GetPlacedNodeSave<NodeSave>())
                    .ToArray(),
                inners = inner
                    .Where(x => !(x.Instance is ConstNode))
                    .Select(x => x.GetPlacedNodeSave<NodeSave>())
                    .ToArray(),
                ints = consts
                    .Where(x => ((ConstNode) x.Instance).Value.GetType() == typeof(IntSignal))
                    .Select(x => x.GetPlacedNodeSave<IntNodeSave>())
                    .ToArray(),
                reals = consts
                    .Where(x => ((ConstNode) x.Instance).Value.GetType() == typeof(RealSignal))
                    .Select(x => x.GetPlacedNodeSave<RealNodeSave>())
                    .ToArray(),
                strings = consts
                    .Where(x => ((ConstNode) x.Instance).Value.GetType() == typeof(StringSignal))
                    .Select(x => x.GetPlacedNodeSave<StringNodeSave>())
                    .ToArray(),
                nans = consts
                    .Where(x => ((ConstNode) x.Instance).Value is NaNSignal)
                    .Where(x => ((ConstNode) x.Instance).Value.GetType() == typeof(NaNSignal))
                    .Select(x => x.GetPlacedNodeSave<NaNNodeSave>())
                    .ToArray()
            };
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}", Category, Identifier);
        }
    }
}

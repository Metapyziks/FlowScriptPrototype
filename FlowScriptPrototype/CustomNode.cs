using System.Collections.Generic;

namespace FlowScriptPrototype
{
    class CustomNode : Node
    {
        private SocketNode[] _inputs;
        private SocketNode[] _outputs;

        public CustomNode(int inputs, int outputs)
            : base(inputs, outputs)
        {
            _inputs = new SocketNode[inputs];
            _outputs = new SocketNode[outputs];

            for (int i = 0; i < inputs; ++i) {
                _inputs[i] = new SocketNode();
            }

            for (int i = 0; i < outputs; ++i) {
                _outputs[i] = new SocketNode();
            }
        }

        public SocketNode GetInput(int index)
        {
            return _inputs[index];
        }

        public SocketNode GetOutput(int index)
        {
            return _outputs[index];
        }

        public override void Pulse(params Signal[] inputs)
        {
            for (int i = 0; i < InputCount; ++i) {
                _inputs[i].Pulse(inputs[i]);
            }
        }

        public override Node ConnectToInput(int index, Socket input)
        {
            _outputs[index].ConnectToInput(0, input);

            return this;
        }
    }
}

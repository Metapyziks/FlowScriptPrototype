using System;

namespace FlowScriptPrototype
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomNode.CreatePrototype("Increment", 1, 1, node => {
                var addc = node.Add(new ConstNode(new IntSignal(1)));
                var addo = node.Add(new AddNode());

                node.GetInput(0)
                    .ConnectToInput(addc.Input)
                    .ConnectToInput(addo.InputA);

                addc.Output
                    .ConnectToInput(addo.InputB);

                addo.Output
                    .ConnectToInput(node.GetOutput(0));
            });

            CustomNode.CreatePrototype("Loop", 2, 3, node => {
                var comp = node.Add(new LessThanNode());

                node.GetInput(0)
                    .ConnectToInput(comp.InputA);

                node.GetInput(1)
                    .ConnectToInput(comp.InputB)
                    .ConnectToInput(node.GetOutput(2));

                comp.OutputPositive
                    .ConnectToInput(node.GetOutput(0));

                comp.OutputNegative
                    .ConnectToInput(node.GetOutput(1));
            });

            var loop = CustomNode.Get("Loop");
            var prt1 = new PrintNode();
            var prt2 = new PrintNode();
            var incr = CustomNode.Get("Increment");
            var done = new ConstNode(new StringSignal("Done!"));

            loop.ConnectToInput(0, prt1.Input)
                .ConnectToInput(1, done.Input)
                .ConnectToInput(2, new Socket(loop, 1));

            prt1.Output
                .ConnectToInput(new Socket(incr, 0));

            incr.ConnectToInput(0, new Socket(loop, 0));

            done.Output
                .ConnectToInput(prt2.Input);

            loop.PulseInput(0, new IntSignal(0));
            loop.PulseInput(1, new IntSignal(10));

            while (Node.Step());
            
            Console.ReadKey(true);
        }
    }
}

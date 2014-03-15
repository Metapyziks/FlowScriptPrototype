using System;

namespace FlowScriptPrototype
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomNode.CreatePrototype("Tools", "Increment", 1, 1, node => {
                var addc = node.AddConstant(new IntSignal(1));
                var addo = node.AddNode("Math", "Add");

                node.GetInput(0)
                    .ConnectToInput(addc, 0)
                    .ConnectToInput(addo, 0);

                addc.Output
                    .ConnectToInput(addo, 1);

                addo.ConnectToInput(0, node.GetOutput(0));
            });

            CustomNode.CreatePrototype("Tools", "Loop", 2, 3, node => {
                var comp = node.AddNode("Math", "LessThan");

                node.GetInput(0)
                    .ConnectToInput(comp, 0);

                node.GetInput(1)
                    .ConnectToInput(comp, 1)
                    .ConnectToInput(node.GetOutput(2));

                comp.ConnectToInput(0, node.GetOutput(0));

                comp.ConnectToInput(1, node.GetOutput(1));
            });

            var loop = CustomNode.GetCustomReference("Tools", "Loop");
            var prt1 = Node.GetInstance("Utils", "PrintLine");
            var prt2 = Node.GetInstance("Utils", "PrintLine");
            var incr = CustomNode.GetCustomReference("Tools", "Increment");
            var done = new ConstNode(new StringSignal("Done!"));

            loop.ConnectToInput(0, prt1, 0)
                .ConnectToInput(1, done, 0)
                .ConnectToInput(2, loop, 1);

            prt1.ConnectToInput(0, incr, 0);

            incr.ConnectToInput(0, loop, 0);

            done.ConnectToInput(0, prt2, 0);

            loop.PulseInput(0, new IntSignal(0));
            loop.PulseInput(1, new IntSignal(10));

            while (Node.Step());
            
            Console.ReadKey(true);
        }
    }
}

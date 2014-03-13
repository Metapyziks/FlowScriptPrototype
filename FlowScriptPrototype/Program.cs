using System;

namespace FlowScriptPrototype
{
    class Program
    {
        static Node IncrementNode(int amount)
        {
            var node = new CustomNode(1, 1);
            var addc = new ConstNode(new IntSignal(amount));
            var addo = new AddNode();

            node.GetInput(0)
                .ConnectToInput(addc.Input)
                .ConnectToInput(addo.InputA);

            addc.Output
                .ConnectToInput(addo.InputB);

            addo.Output
                .ConnectToInput(node.GetOutput(0).Input);

            return node;
        }

        static Node LoopNode()
        {
            var node = new CustomNode(2, 3);
            var comp = new LessThanNode();

            node.GetInput(0)
                .ConnectToInput(comp.InputA);

            node.GetInput(1)
                .ConnectToInput(comp.InputB)
                .ConnectToInput(node.GetOutput(2).Input);

            comp.OutputPositive
                .ConnectToInput(node.GetOutput(0).Input);

            comp.OutputNegative
                .ConnectToInput(node.GetOutput(1).Input);

            return node;
        }

        static void Main(string[] args)
        {
            var loop = LoopNode();
            var prt1 = new PrintNode();
            var prt2 = new PrintNode();
            var incr = IncrementNode(1);
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

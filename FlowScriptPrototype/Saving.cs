using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace FlowScriptPrototype
{
    enum PlacedNodeType
    {
        Input = 0,
        Output = 1,
        Inner = 2
    }

    class PlacedNodeSave
    {
        public int index { get; set; }

        public int x { get; set; }

        public int y { get; set; }

        public PlacedNodeType type { get; set; }

        public OutputSave[][] outputs { get; set; }

        public NodeSave data { get; set; }
    }

    class OutputSave
    {
        public int node { get; set; }

        public int socket { get; set; }
    }

    class NodeSave
    {
        public String category { get; set; }

        public String identifier { get; set; }
    }

    class PrototypeNodeSave : NodeSave
    {
        public static PrototypeNodeSave FromFile(String path)
        {
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<PrototypeNodeSave>(File.ReadAllText(path));
        }

        public String category { get; set; }

        public String identifier { get; set; }

        public PlacedNodeSave[] nodes { get; set; }

        public void Save(String path)
        {
            var serializer = new JavaScriptSerializer();
            File.WriteAllText(path, serializer.Serialize(this));
        }
    }

    class IntNodeSave : NodeSave
    {
        public long value { get; set; }

        public IntNodeSave()
        {
            category = "Constant";
            identifier = "Int";
        }
    }

    class RealNodeSave : NodeSave
    {
        public double value { get; set; }

        public RealNodeSave()
        {
            category = "Constant";
            identifier = "Real";
        }
    }

    class StringNodeSave : NodeSave
    {
        public String value { get; set; }

        public StringNodeSave()
        {
            category = "Constant";
            identifier = "String";
        }
    }

    class NaNNodeSave : NodeSave
    {
        public NaNNodeSave()
        {
            category = "Constant";
            identifier = "NaN";
        }
    }
}

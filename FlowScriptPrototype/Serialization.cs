using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlowScriptPrototype.Serialization
{
    public class SerializationContext
    {
        public SerializationContext()
        {

        }

        public String Obj(params Expression<Func<Object, String>>[] keyVals)
        {
            var builder = new StringBuilder();

            builder.Append("{");
            foreach (var keyVal in keyVals) {
                builder.AppendFormat("\"{0}\":{1}", keyVal.Parameters.First().Name, keyVal.Compile()(null));

                if (keyVal != keyVals[keyVals.Length - 1]) {
                    builder.Append(",");
                }
            }
            builder.Append("}");

            return builder.ToString();
        }

        public String Arr(params Object[] vals)
        {
            var builder = new StringBuilder();

            builder.Append("[");
            foreach (var val in vals) {
                builder.Append(val);

                if (val != vals[vals.Length - 1]) {
                    builder.Append(",");
                }
            }
            builder.Append("]");

            return builder.ToString();
        }

        public String Str(Object value)
        {
            return String.Format("\"{0}\"", value);
        }

        public String Str(String format, params Object[] args)
        {
            return String.Format("\"{0}\"", String.Format(format, args));
        }

        public String Int(long value)
        {
            return value.ToString();
        }
    }
}

using System.Collections.Generic;
using System.Text;

namespace CppClassExporter
{
    public static class Utils
    {
        public static List<T> Aggregate<T>(List<T> aggregate, List<T> nextResult)
        {
            if (aggregate == null) return nextResult;
            if (nextResult == null) return aggregate;
            aggregate.AddRange(nextResult);
            return aggregate;
        }

        public static StringBuilder AppendLineWithIndent(this StringBuilder str, object obj, int indent, string indentStr)
        {
            Indent(str, indent, indentStr);
            str.AppendLine(obj.ToString());
            return str;
        }

        public static StringBuilder Indent(this StringBuilder str, int indent, string indentStr)
        {
            for (int i = 0; i < indent; i++)
            {
                str.Append(indentStr);
            }
            return str;
        }
    }
}
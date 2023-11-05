using System.Text;

namespace CppClassExporter.Exporter
{
    public abstract class AbstractCodeGenerateScope : IDisposable
    {
        public StringBuilder sb;

#pragma warning disable CS8618 // 在需要使用 Indent 参数时, 必须先添加缩进再 Enter, 
        // 而在下面的构造函数中已经包含 Enter, 基类的构造函数早于子类运行. 因此提供了一个没有任何操作的空构造函数, 这会导致 CS8618: 未初始化错误.
        protected AbstractCodeGenerateScope() {}
#pragma warning restore CS8618
        protected AbstractCodeGenerateScope(StringBuilder sb)
        {
            this.sb = sb;
            Enter(sb);
        }

        public abstract void Enter(StringBuilder sb);
        public abstract void Exit(StringBuilder sb);

        public void Dispose()
        {
            Exit(sb);
        }
    }

    public class ExternCScope : AbstractCodeGenerateScope
    {
        public ExternCScope(StringBuilder sb) : base(sb) { }
        public override void Enter(StringBuilder sb) => sb.AppendLine("extern \"C\"{");
        public override void Exit(StringBuilder sb) => sb.AppendLine("}");
    }

    public class ParameterListScope : AbstractCodeGenerateScope
    {
        public ParameterListScope(StringBuilder sb) : base(sb) { }
        public override void Enter(StringBuilder sb) => sb.Append("(");
        public override void Exit(StringBuilder sb) => sb.Append(")");
    }

    public class CodeBodyScope : AbstractCodeGenerateScope
    {
        public int indent;
        public string indentStr;
        public CodeBodyScope(StringBuilder sb, int indent, string indentStr)
        {
            this.sb = sb;
            this.indent = indent;
            this.indentStr = indentStr;
            Enter(sb);
        }

        public override void Enter(StringBuilder sb) => sb.Indent(indent, indentStr).AppendLine("{");

        public override void Exit(StringBuilder sb) => sb.Indent(indent, indentStr).AppendLine("}");
    }

}

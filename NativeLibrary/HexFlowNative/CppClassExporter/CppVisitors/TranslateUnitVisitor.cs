using Antlr4.Runtime.Misc;
using CppClassExporter.CppElements;

namespace CppClassExporter.CppVisitors
{
    public class TranslateUnitVisitor : CPP14ParserBaseVisitor<List<ClassDecl>>
    {
        public ClassVisitor classVisitor = new ClassVisitor();

        public override List<ClassDecl> VisitClassSpecifier([NotNull] CPP14Parser.ClassSpecifierContext context)
        {
            return new List<ClassDecl>() { classVisitor.Visit(context) };
        }

        protected override List<ClassDecl> AggregateResult(List<ClassDecl> aggregate, List<ClassDecl> nextResult)
        {
            if (aggregate == null) return nextResult;
            if (nextResult == null) return aggregate;
            aggregate.AddRange(nextResult);
            return aggregate;
        }
    }
}
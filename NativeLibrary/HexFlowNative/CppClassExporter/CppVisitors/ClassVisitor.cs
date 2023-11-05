using System.Linq;
using Antlr4.Runtime.Misc;
using CppClassExporter.CppElements;

namespace CppClassExporter.CppVisitors
{
    public class ClassVisitor : CPP14ParserBaseVisitor<ClassDecl>
    {
        public MemberConstructor memberListener = new MemberConstructor();

        public override ClassDecl VisitClassHeadName([NotNull] CPP14Parser.ClassHeadNameContext context)
        {
            return new ClassDecl { name = context.className().GetText() };
        }

        public override ClassDecl VisitMemberSpecification([NotNull] CPP14Parser.MemberSpecificationContext context)
        {
            var partialClass = new ClassDecl();
            var members = new List<MemberDecl>();
            var currentAccess = "";
            for (int i = 0; i < context.children.Count; i++)
            {
                var child = context.children[i];
                if (child is CPP14Parser.AccessSpecifierContext accessSpecifier)
                {
                    currentAccess = accessSpecifier.GetText();
                }
                else if (child is CPP14Parser.MemberdeclarationContext memberdeclaration)
                {
                    // 没访问级别的第一段成员声明是什么级别来着...
                    if (string.IsNullOrEmpty(currentAccess) || currentAccess == "public")
                    {
                        memberdeclaration.EnterRule(memberListener);
                        members.AddRange(memberListener.members);
                    }
                }
            }

            foreach (var member in members)
            {
                member.AddToClass(partialClass);
            }
            return partialClass;
        }
        protected override ClassDecl AggregateResult(ClassDecl aggregate, ClassDecl nextResult)
        {
            if (nextResult == null) return aggregate;
            if(aggregate == null) return nextResult;
            return aggregate.Aggregate(nextResult);
        }
    }
}
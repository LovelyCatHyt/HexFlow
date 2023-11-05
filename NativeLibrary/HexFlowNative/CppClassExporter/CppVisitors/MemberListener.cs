using Antlr4.Runtime.Tree;
using Antlr4.Runtime.Misc;
using CppClassExporter.CppElements;

namespace CppClassExporter.CppVisitors
{
    /// <summary>
    /// 成员监听器
    /// </summary>
    public class MemberConstructor : CPP14ParserBaseListener
    {
        public string retTypeName = "";
        public bool isStatic = false;

        protected ParseTreeWalker walker = new ParseTreeWalker();
        public List<MemberDecl> members = new List<MemberDecl>();

        public class IsFunctionChecker : CPP14ParserBaseVisitor<bool?>
        {
            public override bool? VisitParametersAndQualifiers([NotNull] CPP14Parser.ParametersAndQualifiersContext context) => true;

            protected override bool? AggregateResult(bool? aggregate, bool? nextResult)
            {
                return (aggregate ?? false) || (nextResult ?? false);
            }
        }

        public class ParameterListVisitor : CPP14ParserBaseVisitor<List<ParameterDecl>>
        {
            public override List<ParameterDecl> VisitParameterDeclaration([NotNull] CPP14Parser.ParameterDeclarationContext context)
            {
                var result = new ParameterDecl();
                result.name = GetIdExpression(context.declarator().pointerDeclarator()) ;
                result.type.baseTypeName = context.declSpecifierSeq().GetText();
                result.type.isPointer = context.declarator().pointerDeclarator().pointerOperator().Length > 0;

                return new List<ParameterDecl> { result };
            }

            protected override List<ParameterDecl> AggregateResult(List<ParameterDecl> aggregate, List<ParameterDecl> nextResult)
            {
                return Utils.Aggregate(aggregate, nextResult);
            }
        }

        public override void EnterSimpleTypeSpecifier([NotNull] CPP14Parser.SimpleTypeSpecifierContext context)
        {
            // 基础返回类型, 但不确定是否指针
            retTypeName = context.GetText();
        }

        public override void EnterStorageClassSpecifier([NotNull] CPP14Parser.StorageClassSpecifierContext context)
        {
            if (context.GetText() == "static") isStatic = true;
        }

        public override void EnterMemberdeclaration([NotNull] CPP14Parser.MemberdeclarationContext context)
        {
            ClearState();
                       
            if (context.functionDefinition() != null)
            {
                // 写在头文件的函数定义, 而非仅声明

                // 触发 EnterSimpleTypeSpecifier, EnterStorageClassSpecifier, 获取 retTypeName 和 isStatic 的值
                CPP14Parser.DeclSpecifierSeqContext declSpecifierSeq = context.functionDefinition().declSpecifierSeq();
                if(declSpecifierSeq!=null)
                {
                    walker.Walk(this, declSpecifierSeq);
                }
                else
                {
                    // 构造函数和析构函数没有 declSpecifierSeq 结构
                    retTypeName = "";
                    isStatic = false;
                }

                var declarator = context.functionDefinition().declarator();
                var isPointer = declarator.pointerDeclarator().pointerOperator().Length > 0;
                var retType = new TypeDecl { baseTypeName = retTypeName, isPointer = isPointer };
                // 构建函数签名
                FunctionDecl func = new FunctionDecl { retType = retType };
                var paramsVisitor = new ParameterListVisitor();
                func.parameters = paramsVisitor.Visit(declarator) ?? new List<ParameterDecl>();
                func.isMemberFunction = !isStatic;
                func.name = GetIdExpression(declarator.pointerDeclarator());
                members.Add(func);
            }
            else
            {
                // 触发 EnterSimpleTypeSpecifier, EnterStorageClassSpecifier, 获取 retTypeName 和 isStatic 的值
                if (context.declSpecifierSeq() != null)
                {
                    walker.Walk(this, context.declSpecifierSeq());
                }
                else
                {
                    // 构造函数和析构函数没有 declSpecifierSeq 结构
                    retTypeName = "";
                    isStatic = false;
                }

                var isFunctionChecker = new IsFunctionChecker();

                foreach (var declarator in context.memberDeclaratorList().memberDeclarator())
                {
                    MemberDecl newMember;
                    // 祈祷这一串没有 NRE, 下同
                    var isPointer = declarator.declarator().pointerDeclarator().pointerOperator().Length > 0;
                    var retType = new TypeDecl { baseTypeName = retTypeName, isPointer = isPointer };

                    // 是函数吗?
                    if (isFunctionChecker.Visit(declarator) ?? false)
                    {
                        FunctionDecl function = new FunctionDecl { retType = retType };
                        newMember = function;
                        var paramsVisitor = new ParameterListVisitor();
                        function.parameters = paramsVisitor.Visit(declarator) ?? new List<ParameterDecl>();
                        function.isMemberFunction = !isStatic;
                    }
                    else
                    {
                        newMember = new VariableDecl { retType = retType };
                    }
                    newMember.name = GetIdExpression(declarator.declarator().pointerDeclarator());
                    members.Add(newMember);
                }
            }
        }

        private void ClearState()
        {
            members.Clear();
            retTypeName = "";
            isStatic = false;
        }


        public static string GetIdExpression(CPP14Parser.PointerDeclaratorContext context)
        {
            var noPointer = context.noPointerDeclarator();
            while (noPointer.noPointerDeclarator() != null)
            {
                noPointer = noPointer.noPointerDeclarator();
            }
            return noPointer.GetText();
        }

    }
}

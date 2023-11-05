namespace CppClassExporter
{
    namespace CppElements
    {
        public struct TypeDecl
        {
            public string baseTypeName = "";
            public bool isPointer = false;
            public bool IsVoid => baseTypeName == "void";

            public TypeDecl() { }

            public override string ToString()
            {
                return isPointer ? baseTypeName + "*" : baseTypeName;
            }
        }

        public abstract class MemberDecl
        {
            public string name = "";
            public TypeDecl retType = new TypeDecl();

            public abstract void AddToClass(ClassDecl classDef);
        }

        public class ParameterDecl
        {
            public string name = "";
            public TypeDecl type = new TypeDecl();

            public override string ToString() => $"{type} {name}";
        }

        public class FunctionDecl : MemberDecl
        {
            public bool isMemberFunction = true;
            public List<ParameterDecl> parameters = new List<ParameterDecl>();

            public override void AddToClass(ClassDecl def)
            {
                if (string.IsNullOrEmpty(retType.baseTypeName))
                {
                    if (!name.StartsWith("~")) // 加多一层 if 而不是合并到外层, 否则会走 else if 的逻辑变成 dynamicMethods 了
                    { 
                        def.ctors.Add(this);
                    }
                }
                else if (isMemberFunction)
                {
                    def.dynamicMethods.Add(this);
                }
                else
                {
                    def.staticMethods.Add(this);
                }
            }

            public override string ToString()
            {
                if (string.IsNullOrEmpty(retType.baseTypeName))
                {
                    // 没有基础类型, 说明是构造函数或者析构函数
                    return $"{name}({string.Join(", ", parameters)})";
                }
                else if (isMemberFunction)
                {
                    // 成员函数
                    return $"{retType} {name}({string.Join(", ", parameters)})";
                }
                else
                {
                    // 静态函数
                    return $"static {retType} {name}({string.Join(", ", parameters)})";
                }
            }
        }

        public class VariableDecl : MemberDecl
        {
            public override void AddToClass(ClassDecl classDef)
            {
                classDef.variables.Add(this);
            }

            public override string ToString() => $"{retType} {name}";
        }

        public class ClassDecl
        {
            public string name = "";
            public string fileName = "";
            public List<VariableDecl> variables = new List<VariableDecl>();
            public List<FunctionDecl> ctors = new List<FunctionDecl>();
            public FunctionDecl? dtor = null;
            public List<FunctionDecl> dynamicMethods = new List<FunctionDecl>();
            public List<FunctionDecl> staticMethods = new List<FunctionDecl>();

            public ClassDecl Aggregate(ClassDecl nextResult)
            {
                if (string.IsNullOrEmpty(name)) name = nextResult.name;
                variables.AddRange(nextResult.variables);
                ctors.AddRange(nextResult.ctors);
                dynamicMethods.AddRange(nextResult.dynamicMethods);
                staticMethods.AddRange(nextResult.staticMethods);
                return this;
            }
        }
    }
}
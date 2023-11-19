using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CppClassExporter.CppElements;

namespace CppClassExporter.Exporter
{
    public partial class Exporter
    {
        public ClassDecl decl;

        public string sourceFilePath;
        public string dllFileName;
        public string? cExportDecorator;
        public string indentStr = "\t";

        public Dictionary<string, string> cppToCTypeMap = new Dictionary<string, string>();

        public Exporter(ClassDecl decl, string cFuncFileName, string dllFileName, string sourceFilePath, string? cExportDecorator = null)
        {
            this.decl = decl;
            if (string.IsNullOrEmpty(decl.fileName)) decl.fileName = $"{decl.name}.h";
            cppToCTypeMap[decl.name] = "void";
            this.dllFileName = dllFileName;
            this.sourceFilePath = sourceFilePath;
            this.cExportDecorator = cExportDecorator;
            if(!string.IsNullOrEmpty(cExportDecorator))
            {
                this.cExportDecorator = $"{cExportDecorator.Trim()} ";
            }
        }

        #region ExportAsC

        public void WriteCDeclFile(string path)
        {
            List<string> lines = new List<string>();
            int prefixLine, postfixLine;
            CollectGenerateLines(path, lines, out prefixLine, out postfixLine);

            using (var sw = new StreamWriter(path))
            {
                for (int i = 0; i < prefixLine; i++)
                {
                    sw.WriteLine(lines[i]);
                }
                sw.WriteLine(TextSnippet.GeneratedPrefix);
                sw.WriteLine($"#include \"{Path.GetRelativePath(Path.GetDirectoryName(path)!, sourceFilePath)}\"");
                sw.WriteLine();
                sw.WriteLine(ExportAsCDecl());
                sw.WriteLine(TextSnippet.GeneratedPostfix);
                for (int i = postfixLine + 1; i < lines.Count; i++)
                {
                    sw.WriteLine(lines[i]);
                }
            }
        }

        public void WriteCDefFile(string path, string declFilePath)
        {
            List<string> lines = new List<string>();
            int prefixLine, postfixLine;
            CollectGenerateLines(path, lines, out prefixLine, out postfixLine);

            using (var sw = new StreamWriter(path))
            {
                for (int i = 0; i < prefixLine; i++)
                {
                    sw.WriteLine(lines[i]);
                }
                sw.WriteLine(TextSnippet.GeneratedPrefix);
                sw.WriteLine($"#include \"{Path.GetRelativePath(Path.GetDirectoryName(path)!, declFilePath)}\"");
                sw.WriteLine();
                sw.WriteLine(ExportAsCDef());
                sw.WriteLine(TextSnippet.GeneratedPostfix);
                for (int i = postfixLine + 1; i < lines.Count; i++)
                {
                    sw.WriteLine(lines[i]);
                }
            }
        }

        private static void CollectGenerateLines(string path, List<string> lines, out int prefixLine, out int postfixLine)
        {
            prefixLine = 0;
            postfixLine = 0;
            using (var sr = new StreamReader(path))
            {
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains(TextSnippet.GeneratedPrefixKeyword))
                    {
                        prefixLine = lines.Count;
                    }
                    if (line.Contains(TextSnippet.GeneratedPostfixKeyword))
                    {
                        postfixLine = lines.Count;
                    }
                    lines.Add(line);
                }
            }
            if (postfixLine < prefixLine)
            {
                Console.WriteLine($"Error occurs when processing {path}: " +
                $"\"{TextSnippet.GeneratedPostfixKeyword}\" at line:{postfixLine} should be located AFTER " +
                $"\"{TextSnippet.GeneratedPrefixKeyword}\" at line:{prefixLine}. " +
                $"Code within these range will be removed, which may not be expected.");
                var t = postfixLine;
                postfixLine = prefixLine;
                prefixLine = t;
            }
        }

        /// <summary>
        /// 根据类声明输出 C 语言函数声明, 并标记 dll 导出
        /// </summary>
        public string ExportAsCDecl()
        {
            var strBuilder = new StringBuilder();
            // extern "C" {
            using (new ExternCScope(strBuilder))
            {
                C_GenCtor(strBuilder, decl.ctors, 1);
                C_GenDtor(strBuilder, 1);
                foreach (var func in decl.dynamicMethods)
                {
                    C_GenFunction(strBuilder, func, 1);
                }
                foreach (var func in decl.staticMethods)
                {
                    C_GenFunction(strBuilder, func, 1);
                }
            }
            // }
            return strBuilder.ToString();
        }

        /// <summary>
        /// 根据类声明输出 C 语言函数定义
        /// </summary>
        public string ExportAsCDef()
        {
            var strBuilder = new StringBuilder();
            C_GenCtorDef(strBuilder, decl.ctors, 0);
            C_GenDtorDef(strBuilder, 0);
            foreach (var func in decl.dynamicMethods)
            {
                C_GenFunctionDef(strBuilder, func, 0);
            }
            foreach (var func in decl.staticMethods)
            {
                C_GenFunctionDef(strBuilder, func, 0);
            }
            return strBuilder.ToString();
        }
        
        /// <summary>
        /// 将类型转换为可安全导出的类型. 转换规则基于 <see cref="cppToCTypeMap"/> 字典
        /// </summary>
        public TypeDecl ConvertCppToCType(TypeDecl cppType)
        {
            if (cppToCTypeMap.ContainsKey(cppType.baseTypeName))
            {
                cppType.baseTypeName = cppToCTypeMap[cppType.baseTypeName];
            }
            return cppType;
        }

        public List<ParameterDecl> ConvertCToCSharpTypes(IList<ParameterDecl> parameters)
        {
            var result = new List<ParameterDecl>(parameters.Count);
            for (int i = 0; i < parameters.Count; i++)
            {
                result.Add(new ParameterDecl { name = parameters[i].name, type = ConvertCppToCType(parameters[i].type) });
            }
            return result;
        }

        private void C_GenCtor(StringBuilder str, IList<FunctionDecl> ctors, int indent)
        {
            for (int i = 0; i < ctors.Count; i++)
            {
                FunctionDecl ctor = ctors[i];
                List<ParameterDecl> safeParams = ConvertCToCSharpTypes(ctor.parameters);
                // void* $(cExportDecorator) name_ctor_i(int a, char b);
                str.Indent(indent, indentStr)
                .Append(cExportDecorator)
                .Append("void* ")
                .Append(string.Format(TextSnippet.C_CtorFuncNamePattern, decl.name, i));
                using (new ParameterListScope(str))
                {
                    str.Append(string.Join(", ", safeParams));
                }
                str.AppendLine(";");

                str.AppendLine();
            }
        }

        private void C_GenCtorDef(StringBuilder str, IList<FunctionDecl> ctors, int indent)
        {
            for (int i = 0; i < ctors.Count; i++)
            {
                FunctionDecl ctor = ctors[i];
                List<ParameterDecl> safeParams = ConvertCToCSharpTypes(ctor.parameters);
                // 实现 不需要加 cExportDecorator
                str.Indent(indent, indentStr)
                .Append("void* ")
                .Append(string.Format(TextSnippet.C_CtorFuncNamePattern, decl.name, i));
                using (new ParameterListScope(str))
                {
                    str.Append(string.Join(", ", safeParams));
                }
                str.AppendLine();
                // {
                using (new CodeBodyScope(str, indent, indentStr))
                {
                    // return new MyClass(a,b);
                    string args = string.Join(", ", safeParams.Select((p, i) =>
                    {
                        // 填写形参时要把类型转回去
                        var originType = ctor.parameters[i].type;
                        if (p.type.baseTypeName == originType.baseTypeName) return p.name;
                        else return $"({originType}){p.name}";
                    }
                    ).ToArray());
                    str.Indent(indent + 1, indentStr).AppendLine($"return (void*)(new {decl.name}({args}));");
                }
                // }

                str.AppendLine();
            }
        }

        private void C_GenDtor(StringBuilder str, int indent)
        {
            // 声明
            // void $(cExportDecorator) className_dtor()
            str.Indent(indent, indentStr)
            .Append(cExportDecorator)
            .Append("void ")
            .Append(string.Format(TextSnippet.C_DtorFuncNamePattern, decl.name))
            .AppendLine("(void* this_raw);");
            
            str.AppendLine();
        }

        private void C_GenDtorDef(StringBuilder str, int indent)
        {
            // 实现
            str.Indent(indent, indentStr)
            .Append("void ")
            .Append(string.Format(TextSnippet.C_DtorFuncNamePattern, decl.name))
            .AppendLine("(void* this_raw)");
            using (new CodeBodyScope(str, indent, indentStr))
            {
                str.Indent(indent + 1, indentStr)
                .AppendLine($"delete ({decl.name}*)this_raw;");
            }

            str.AppendLine();
        }

        private void C_GenFunction(StringBuilder str, FunctionDecl func, int indent)
        {
            List<ParameterDecl> safeParams = ConvertCToCSharpTypes(func.parameters);
            Dictionary<string, TypeDecl> paramMap = func.parameters.ToDictionary(p => p.name, p => p.type);
            if (func.isMemberFunction)
            {
                // 成员函数需要加个 this 指针, 参数名为 "this_raw"
                safeParams.Insert(0, new ParameterDecl
                {
                    name = "this_raw",
                    type = new TypeDecl { baseTypeName = "void", isPointer = true }
                });
            }

            //声明
            // retType name_ctor_i(int a, char b)
            str.Indent(indent, indentStr)
            .Append(cExportDecorator)
            .Append(func.retType).Append(' ')
            .Append($"{decl.name}_{func.name}");
            
            using (new ParameterListScope(str))
            {
                str.Append(string.Join(", ", safeParams));
            }
            str.AppendLine(";");

            str.AppendLine();
        }

        private void C_GenFunctionDef(StringBuilder str, FunctionDecl func, int indent)
        {
            List<ParameterDecl> safeParams = ConvertCToCSharpTypes(func.parameters);
            Dictionary<string, TypeDecl> paramMap = func.parameters.ToDictionary(p => p.name, p => p.type);
            if (func.isMemberFunction)
            {
                // 成员函数需要加个 this 指针, 参数名为 "this_raw"
                safeParams.Insert(0, new ParameterDecl
                {
                    name = "this_raw",
                    type = new TypeDecl { baseTypeName = "void", isPointer = true }
                });
            }

            // 实现
            str.Indent(indent, indentStr)
            .Append(func.retType).Append(' ')
            .Append($"{decl.name}_{func.name}");
            using (new ParameterListScope(str))
            {
                str.Append(string.Join(", ", safeParams));
            }
            str.AppendLine();
            // {
            //
            // }
            using (new CodeBodyScope(str, indent, indentStr))
            {
                str.Indent(indent + 1, indentStr);
                // return new MyClass(a,b);
                string args = string.Join(", ", func.parameters.Select(p =>
                {
                    if (paramMap.TryGetValue(p.name, out TypeDecl originType))
                    {
                        if (p.type.baseTypeName != originType.baseTypeName) return $"({originType}){p.name}";
                    }
                    return p.name;
                }).ToArray());
                if (!func.retType.IsVoid) str.Append("return ");
                if (func.isMemberFunction) str.AppendLine($"(({decl.name}*)this_raw)->{func.name}({args});");
                else str.AppendLine($"{decl.name}::{func.name}({args});");
            }

            str.AppendLine();
        }

        #endregion

        #region ExportAsCSharp

        /// <summary>
        /// 根据类声明输出 C# 类, 通过 [DllImport] 导入
        /// </summary>
        /// <returns></returns>
        public string ExportAsCSharp()
        {
            var strBuilder = new StringBuilder();
            return strBuilder.ToString();
        }

        #endregion

        /// <summary>
        /// 载入类型转换映射表
        /// </summary>
        public void LoadTypeMap(IEnumerable<KeyValuePair<string, string>> valuePairs)
        {
            foreach (var kv in valuePairs)
            {
                cppToCTypeMap.Add(kv.Key, kv.Value);
            }
        }
    }
}

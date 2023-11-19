using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CppClassExporter.CppElements;
using CppClassExporter.CppVisitors;

namespace CppClassExporter.Exporter
{

    public class ExportAction
    {
        [JsonInclude] public string sourceHeaderPath;
        [JsonInclude] public string? exportHeaderPath;
        [JsonInclude] public string? exportImplPath;
        [JsonInclude] public string? dllFile;
        [JsonInclude] public string? exportCSharpPath;
        [JsonInclude] public bool genearteNonStaticClass;

        [JsonIgnore] protected ExportTask? _owner;
        [JsonIgnore] protected Dictionary<string, string> _typeMap = new Dictionary<string, string>();

        public ExportAction()
        {
            sourceHeaderPath = "Undefined";
            genearteNonStaticClass = false;
        }

        public ExportAction(string sourceHeaderPath, string? exportHeaderPath = null, string? exportImplPath = null, string? dllFile = null, string? exportCSharpPath = null, bool genearteNonStaticClass = false)
        {
            this.sourceHeaderPath = sourceHeaderPath;
            this.exportImplPath = exportImplPath;
            this.exportCSharpPath = exportHeaderPath;
            this.dllFile = dllFile;
            this.exportCSharpPath = exportCSharpPath;
            this.genearteNonStaticClass = genearteNonStaticClass;
            InitFields();
        }

        private static string GetFinalPath(string path, string defaultBasePath)
        {
            if (Path.IsPathRooted(path)) return path;
            return Path.GetFullPath(path, defaultBasePath);
        }

        private void InitFields(string curDir)
        {
            var fileName = Path.GetFileNameWithoutExtension(sourceHeaderPath);
            sourceHeaderPath = GetFinalPath(sourceHeaderPath, curDir);

            exportHeaderPath ??= $"{fileName}.export.h";
            exportHeaderPath = GetFinalPath(exportHeaderPath, curDir);

            exportImplPath ??= $"{fileName}.export.cpp";
            exportImplPath = GetFinalPath(exportImplPath, curDir);

            dllFile ??= $"{fileName}.dll";

            exportCSharpPath ??= $"../CSharp/{fileName}.cs";
            exportCSharpPath = GetFinalPath(exportCSharpPath, Path.GetDirectoryName(sourceHeaderPath) ?? curDir);
        }

        private void InitFields()
        {
            InitFields(Directory.GetCurrentDirectory());
        }

        public bool Run(IEnumerable<ClassDecl> decls)
        {
            bool allSuccess = true;
            foreach (var decl in decls)
            {
                var exporter = new Exporter(decl, Path.GetFileName(exportHeaderPath!), dllFile!, sourceHeaderPath, _owner?.cExportDecorator);

                using (var file = File.Open(exportHeaderPath!, FileMode.OpenOrCreate))
                {
                    if (file == null)
                    {
                        Console.WriteLine($"OpenOrCreate File failed at \"{exportHeaderPath}\"");
                        allSuccess = false;
                        continue;
                    }
                }

                exporter.LoadTypeMap(_typeMap);

                try
                {
                    exporter.WriteCDeclFile(exportHeaderPath!);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error exporting CFile of class {decl.name} in {sourceHeaderPath}:");
                    Console.WriteLine(e);
                    allSuccess = false;
                    continue;
                }

                using (var file = File.Open(exportImplPath!, FileMode.OpenOrCreate))
                {
                    if (file == null)
                    {
                        Console.WriteLine($"OpenOrCreate File failed at \"{exportHeaderPath}\"");
                        allSuccess = false;
                        continue;
                    }
                }

                try
                {
                    exporter.WriteCDefFile(exportImplPath!, exportHeaderPath!);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error exporting CFile of class {decl.name} in {sourceHeaderPath}:");
                    Console.WriteLine(e);
                    allSuccess = false;
                    continue;
                }
            }
            return allSuccess;
        }

        public static ExportTask LoadTasks(string taskJsonPath)
        {
            var task = JsonSerializer.Deserialize<ExportTask>(File.ReadAllText(taskJsonPath)) ?? new ExportTask();

            var curDir = Path.GetDirectoryName(taskJsonPath)!;
            foreach (var action in task.actions)
            {
                action._owner = task;
                action._typeMap = task.typeMap.ToDictionary(kv => kv.Key, kv => kv.Value);
                action.InitFields(curDir);
            }
            return task;
        }

    }

    public class ExportTask
    {
        [JsonInclude] public ExportAction[] actions = new ExportAction[0];
        [JsonInclude] public KeyValuePair<string, string>[] typeMap = new KeyValuePair<string, string>[0];
        [JsonInclude] public KeyValuePair<string, string>[] strReplaceMap = new KeyValuePair<string, string>[0];
        [JsonInclude] public string? cExportDecorator;
        public void RunActions()
        {
            var fileToClassesMap = new Dictionary<string, ClassDecl[]>();
            var filePaths = new HashSet<string>();

            foreach (var action in actions)
            {
                filePaths.Add(action.sourceHeaderPath);
            }

            // 解析类定义
            bool allSuccess = true;
            TranslateUnitVisitor visitor = new TranslateUnitVisitor();
            foreach (var path in filePaths)
            {
                string sourceText = File.ReadAllText(path);

                // 替换字符串
                foreach (var replacePair in strReplaceMap)
                {
                    sourceText = sourceText.Replace(replacePair.Key, replacePair.Value);
                }

                ICharStream stream = new AntlrInputStream(sourceText);
                ITokenSource lexer = new CPP14Lexer(stream);
                ITokenStream tokens = new CommonTokenStream(lexer);
                CPP14Parser parser = new CPP14Parser(tokens);
                // parser.BuildParseTree = true;
                IParseTree tree;
                try
                {
                    tree = parser.translationUnit();
                }
                catch (Exception e)
                {
                    allSuccess = false;
                    Console.WriteLine($"Error parsing file {path}:");
                    Console.WriteLine(e);
                    continue;
                }
                var decls = visitor.Visit(tree);
                if (decls == null || decls.Count == 0)
                {
                    allSuccess = false;
                    Console.WriteLine($"Can not find a valid Class Declaration in file {path}. Check your grammar first.");
                    continue;
                }
                fileToClassesMap[path] = decls.ToArray();
            }

            // 运行代码生成任务
            // TODO: 假设不同任务使用的源文件不重复
            foreach (var action in actions)
            {
                if (fileToClassesMap.TryGetValue(action.sourceHeaderPath, out var decls))
                {
                    allSuccess &= action.Run(decls);
                }
            }

            Console.WriteLine();
            if (!allSuccess)
            {
                Console.WriteLine("Finished with error(s), see log above.");
            }
            else
            {
                Console.WriteLine("Finshed successfully.");
            }
        }
    }
}

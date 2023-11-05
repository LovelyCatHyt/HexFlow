using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using CppClassExporter.Exporter;

namespace CppClassExporter
{
    public class Program
    {
        static string ToAbsulutePath(string path)
        {
            if (Path.IsPathRooted(path)) return path;
            return Path.GetFullPath(path, Directory.GetCurrentDirectory());
        }

        static void Main(string[] args)
        {
            //TranslateUnitVisitor visitor = new TranslateUnitVisitor();
            //string test = File.ReadAllText("test.hpp");

            //ICharStream stream = new AntlrInputStream(test);
            //ITokenSource lexer = new CPP14Lexer(stream);
            //ITokenStream tokens = new CommonTokenStream(lexer);
            //CPP14Parser parser = new CPP14Parser(tokens);
            //parser.BuildParseTree = true;
            //IParseTree tree = parser.translationUnit();

            //var results = visitor.Visit(tree);

            //foreach (var cls in results)
            //{
            //    var exporter = new Exporter.Exporter(cls);
            //    Console.WriteLine(exporter.ExportAsC());
            //}

            if (args.Length == 0)
            {
                args = new[] { "Tasks.json"};
            }

            foreach (var arg in args)
            {
                string taskPath = ToAbsulutePath(arg);

                var tasks = ExportAction.LoadTasks(taskPath);
                Console.WriteLine($"{tasks.actions.Length} actions loaded.");
                tasks.RunActions();
            }
        }
    }
}
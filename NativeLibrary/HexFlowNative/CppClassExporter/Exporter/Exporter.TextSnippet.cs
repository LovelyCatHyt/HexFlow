namespace CppClassExporter.Exporter
{
    public partial class Exporter
    {
        public static class TextSnippet
        {
            public const string GeneratedPrefixKeyword = "[GeneratedCodeBegin]";

            public const string GeneratedPostfixKeyword = "[GeneratedCodeEnd]";

            public const string GeneratedPrefix = 
            $"// {GeneratedPrefixKeyword}\r\n" +
            $"// 请勿手动修改以下内容, 代码生成器会将其覆盖.";
            
            public const string GeneratedPostfix = 
            $"// 下一行注释之后可以任意添加用户内容, 代码生成器不会修改. \r\n" +
            $"// {GeneratedPostfixKeyword}";

            #region CFunction
            /// <summary>
            /// example: className_ctor_ctorIndex
            /// </summary>
            public const string C_CtorFuncNamePattern = "{0}_ctor_{1}";
            /// <summary>
            /// example: className_dtor
            /// </summary>
            public const string C_DtorFuncNamePattern = "{0}_dtor";
            #endregion
        }
    }
}

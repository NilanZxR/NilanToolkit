namespace NilanToolkit.ConfigTool.Editor {
    public struct CSharpClassFileGenerateInfo {
        public string @namespace;
        public string classNameFormat;

        public static CSharpClassFileGenerateInfo Default => new CSharpClassFileGenerateInfo("GeneratedData", "{0}");

        public CSharpClassFileGenerateInfo(string @namespace, string classNameFormat) {
            this.@namespace = @namespace;
            this.classNameFormat = classNameFormat;
        }
        
    }
}

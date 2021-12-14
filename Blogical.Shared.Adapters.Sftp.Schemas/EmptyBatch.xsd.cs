namespace Blogical.Shared.Adapters.Sftp.Schemas {
    using Microsoft.XLANGs.BaseTypes;
    
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [SchemaType(SchemaTypeEnum.Document)]
    [Schema(@"http://Blogical.Shared.Adapters.Sftp.Schemas.EmptyBatch",@"EmptyBatch")]
    [System.SerializableAttribute()]
    [SchemaRoots(new string[] {@"EmptyBatch"})]
    public sealed class EmptyBatch : Microsoft.XLANGs.BaseTypes.SchemaBase {
        
        [System.NonSerializedAttribute()]
        private static object _rawSchema;
        
        [System.NonSerializedAttribute()]
        private const string _strSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns=""http://Blogical.Shared.Adapters.Sftp.Schemas.EmptyBatch"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" targetNamespace=""http://Blogical.Shared.Adapters.Sftp.Schemas.EmptyBatch"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:element name=""EmptyBatch"">
    <xs:complexType>
      <xs:attribute name=""message"" type=""xs:string"" />
      <xs:attribute name=""datetime"" type=""xs:string"" />
      <xs:attribute name=""source"" type=""xs:string"" />
    </xs:complexType>
  </xs:element>
</xs:schema>";
        
        public EmptyBatch() {
        }
        
        public override string XmlContent {
            get {
                return _strSchema;
            }
        }
        
        public override string[] RootNodes {
            get {
                string[] _RootElements = new string [1];
                _RootElements[0] = "EmptyBatch";
                return _RootElements;
            }
        }
        
        protected override object RawSchema {
            get {
                return _rawSchema;
            }
            set {
                _rawSchema = value;
            }
        }
    }
}

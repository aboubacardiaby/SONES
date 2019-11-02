namespace SONES.Biztalk.Schemas {
    using Microsoft.XLANGs.BaseTypes;
    
    
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.BizTalk.Schema.Compiler", "3.0.1.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [SchemaType(SchemaTypeEnum.Document)]
    [Schema(@"http://SONES.Biztalk.Schemas.Customer",@"Customer")]
    [Microsoft.XLANGs.BaseTypes.PropertyAttribute(typeof(global::SONES.Biztalk.Schemas.PropertySchema.CustomerId), XPath = @"/*[local-name()='Customer' and namespace-uri()='http://SONES.Biztalk.Schemas.Customer']/*[local-name()='CustomerId' and namespace-uri()='']", XsdType = @"string")]
    [Microsoft.XLANGs.BaseTypes.DistinguishedFieldAttribute(typeof(System.String), "CustomerId", XPath = @"/*[local-name()='Customer' and namespace-uri()='http://SONES.Biztalk.Schemas.Customer']/*[local-name()='CustomerId' and namespace-uri()='']", XsdType = @"string")]
    [System.SerializableAttribute()]
    [SchemaRoots(new string[] {@"Customer"})]
    [Microsoft.XLANGs.BaseTypes.SchemaReference(@"SONES.Biztalk.Schemas.PropertySchema.PropertySchema", typeof(global::SONES.Biztalk.Schemas.PropertySchema.PropertySchema))]
    public sealed class Customer : Microsoft.XLANGs.BaseTypes.SchemaBase {
        
        [System.NonSerializedAttribute()]
        private static object _rawSchema;
        
        [System.NonSerializedAttribute()]
        private const string _strSchema = @"<?xml version=""1.0"" encoding=""utf-16""?>
<xs:schema xmlns=""http://SONES.Biztalk.Schemas.Customer"" xmlns:b=""http://schemas.microsoft.com/BizTalk/2003"" xmlns:ns0=""https://SONES.Biztalk.Schemas.PropertySchema"" targetNamespace=""http://SONES.Biztalk.Schemas.Customer"" xmlns:xs=""http://www.w3.org/2001/XMLSchema"">
  <xs:annotation>
    <xs:appinfo>
      <b:imports>
        <b:namespace prefix=""ns0"" uri=""https://SONES.Biztalk.Schemas.PropertySchema"" location=""SONES.Biztalk.Schemas.PropertySchema.PropertySchema"" />
      </b:imports>
    </xs:appinfo>
  </xs:annotation>
  <xs:element name=""Customer"">
    <xs:annotation>
      <xs:appinfo>
        <b:properties>
          <b:property name=""ns0:CustomerId"" xpath=""/*[local-name()='Customer' and namespace-uri()='http://SONES.Biztalk.Schemas.Customer']/*[local-name()='CustomerId' and namespace-uri()='']"" />
          <b:property distinguished=""true"" xpath=""/*[local-name()='Customer' and namespace-uri()='http://SONES.Biztalk.Schemas.Customer']/*[local-name()='CustomerId' and namespace-uri()='']"" />
        </b:properties>
      </xs:appinfo>
    </xs:annotation>
    <xs:complexType>
      <xs:sequence>
        <xs:element name=""CustomerId"" type=""xs:string"" />
        <xs:element name=""Name"" type=""xs:string"" />
        <xs:element name=""OrderId"" type=""xs:string"" />
        <xs:element minOccurs=""0"" maxOccurs=""unbounded"" name=""Address"">
          <xs:complexType>
            <xs:sequence>
              <xs:element name=""Address1"" type=""xs:string"" />
              <xs:element name=""AddressType"" type=""xs:string"" />
            </xs:sequence>
          </xs:complexType>
        </xs:element>
      </xs:sequence>
    </xs:complexType>
  </xs:element>
</xs:schema>";
        
        public Customer() {
        }
        
        public override string XmlContent {
            get {
                return _strSchema;
            }
        }
        
        public override string[] RootNodes {
            get {
                string[] _RootElements = new string [1];
                _RootElements[0] = "Customer";
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

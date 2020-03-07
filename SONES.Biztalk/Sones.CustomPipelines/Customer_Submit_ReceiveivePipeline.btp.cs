namespace Sones.CustomPipelines
{
    using System;
    using System.Collections.Generic;
    using Microsoft.BizTalk.PipelineOM;
    using Microsoft.BizTalk.Component;
    using Microsoft.BizTalk.Component.Interop;
    
    
    public sealed class Customer_Submit_ReceiveivePipeline : Microsoft.BizTalk.PipelineOM.ReceivePipeline
    {
        
        private const string _strPipeline = "<?xml version=\"1.0\" encoding=\"utf-16\"?><Document xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instanc"+
"e\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" MajorVersion=\"1\" MinorVersion=\"0\">  <Description /> "+
" <CategoryId>f66b9f5e-43ff-4f5f-ba46-885348ae1b4e</CategoryId>  <FriendlyName>Receive</FriendlyName>"+
"  <Stages>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"1\" Name=\"Decode\" minOccurs=\""+
"0\" maxOccurs=\"-1\" execMethod=\"All\" stageId=\"9d0e4103-4cce-4536-83fa-4a5040674ad6\" />      <Component"+
"s />    </Stage>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"2\" Name=\"Disassemble\" "+
"minOccurs=\"0\" maxOccurs=\"-1\" execMethod=\"FirstMatch\" stageId=\"9d0e4105-4cce-4536-83fa-4a5040674ad6\" "+
"/>      <Components />    </Stage>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"3\" N"+
"ame=\"Validate\" minOccurs=\"0\" maxOccurs=\"-1\" execMethod=\"All\" stageId=\"9d0e410d-4cce-4536-83fa-4a5040"+
"674ad6\" />      <Components>        <Component>          <Name>Sones.PipelineComponents.SetStreamSiz"+
"eContext,Sones.PipelineComponents, Version=1.0.0.0, Culture=neutral, PublicKeyToken=47d17985a651c45c"+
"</Name>          <ComponentName>SetStreamSizeContext</ComponentName>          <Description>Sets the "+
"Stream Length size to a given context Property</Description>          <Version>1.0.0.0</Version>    "+
"      <Properties>            <Property Name=\"ContextPropertyNamespace\">              <Value xsi:typ"+
"e=\"xsd:string\">https://SONES.Biztalk.Schemas.PropertySchema</Value>            </Property>          "+
"  <Property Name=\"ContextPropertyName\">              <Value xsi:type=\"xsd:string\">StreamSize</Value>"+
"            </Property>          </Properties>          <CachedDisplayName>SetStreamSizeContext</Cac"+
"hedDisplayName>          <CachedIsManaged>true</CachedIsManaged>        </Component>      </Componen"+
"ts>    </Stage>    <Stage>      <PolicyFileStage _locAttrData=\"Name\" _locID=\"4\" Name=\"ResolveParty\" "+
"minOccurs=\"0\" maxOccurs=\"-1\" execMethod=\"All\" stageId=\"9d0e410e-4cce-4536-83fa-4a5040674ad6\" />     "+
" <Components />    </Stage>  </Stages></Document>";
        
        private const string _versionDependentGuid = "24172a6d-5b3c-4907-a040-9a477ed3f927";
        
        public Customer_Submit_ReceiveivePipeline()
        {
            Microsoft.BizTalk.PipelineOM.Stage stage = this.AddStage(new System.Guid("9d0e410d-4cce-4536-83fa-4a5040674ad6"), Microsoft.BizTalk.PipelineOM.ExecutionMode.all);
            IBaseComponent comp0 = Microsoft.BizTalk.PipelineOM.PipelineManager.CreateComponent("Sones.PipelineComponents.SetStreamSizeContext,Sones.PipelineComponents, Version=1.0.0.0, Culture=neutral, PublicKeyToken=47d17985a651c45c");;
            if (comp0 is IPersistPropertyBag)
            {
                string comp0XmlProperties = "<?xml version=\"1.0\" encoding=\"utf-16\"?><PropertyBag xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-inst"+
"ance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">  <Properties>    <Property Name=\"ContextProperty"+
"Namespace\">      <Value xsi:type=\"xsd:string\">https://SONES.Biztalk.Schemas.PropertySchema</Value>  "+
"  </Property>    <Property Name=\"ContextPropertyName\">      <Value xsi:type=\"xsd:string\">StreamSize<"+
"/Value>    </Property>  </Properties></PropertyBag>";
                PropertyBag pb = PropertyBag.DeserializeFromXml(comp0XmlProperties);;
                ((IPersistPropertyBag)(comp0)).Load(pb, 0);
            }
            this.AddComponent(stage, comp0);
        }
        
        public override string XmlContent
        {
            get
            {
                return _strPipeline;
            }
        }
        
        public override System.Guid VersionDependentGuid
        {
            get
            {
                return new System.Guid(_versionDependentGuid);
            }
        }
    }
}

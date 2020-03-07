using Microsoft.BizTalk.Component.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Message.Interop;
using System.Collections;
using Sones.Library;

namespace Sones.PipelineComponents
{
    [ComponentCategory(CategoryTypes.CATID_PipelineComponent)]
    [ComponentCategory(CategoryTypes.CATID_Any)]
    [System.Runtime.InteropServices.Guid(ClassId)]
    public class SetStreamSizeContext: PipelineComponentWithConfigurablePropertiesBase, IBaseComponent, IComponent, IComponentUI
    {
        private const string ClassId = "81C8490B-7E83-47A8-A796-7E198F3D85FB";

       

        public string Description
        {
            get
            {
                return "Sets the Stream Length size to a given context Property";
            }
        }

        public IntPtr Icon
        {
            get
            {
                return new IntPtr();
            }
        }

        public string Name
        {
            get
            {
              return  this.GetType().Name;
            }
        }

        public string Version
        {
            get
            {
                return "1.0.0.0";
            }
        }

        [PipelineConfigurable]
        public string ContextPropertyNamespace { get; set;}

        [PipelineConfigurable]
        public string ContextPropertyName { get; set; }
        //[PipelineConfigurable]
        //public string ContextPropertyValue { get; set; }

        public IBaseMessage Execute(IPipelineContext pContext, IBaseMessage pInMsg)
        {
            if(pInMsg == null || pInMsg.BodyPart== null  || pInMsg.Context == null)
            {
                return pInMsg;
            }
            ContextPropertyNamespace.ValidateAsPipelineProperty("ContextPropertyNamespace");
            ContextPropertyName.ValidateAsPipelineProperty("ContextPropertyName");
           // ContextPropertyValue.ValidateAsPipelineProperty("ContextPropertyValue");
            string streamValue = Convert.ToString(pInMsg.BodyPart.Data.Length);
            string streamsize = streamValue.PadLeft(20, '0');
            pInMsg.Context.Promote(ContextPropertyName, ContextPropertyNamespace, streamsize);
          
          
            return pInMsg;
        }



        public IEnumerator Validate(object projectSystem)
        {
            return null;
        }

        public override void GetClassID(out Guid classID)
        {
            classID = new Guid(ClassId);
        }

        public override void InitNew()
        {
            
        }

        
    }
}

using System;

namespace Sones.PipelineComponents
{
    internal class PipelineConfigurableAttribute: Attribute
    {
      
    }

    public class PipelineConfigurableStringAttribute : Attribute
    {
        public Type Type { get; private set; }

        public PipelineConfigurableStringAttribute()
        {
            this.Type = typeof(string);
        }
    }
}
   
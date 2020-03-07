using System;

namespace Sones.PipelineComponents
{
    internal class PipelineConfigurableBoolAttribute : Attribute
    {
        public Type Type { get; private set; }

        public PipelineConfigurableBoolAttribute()
        {
            this.Type = typeof(bool);
        }
    }
}
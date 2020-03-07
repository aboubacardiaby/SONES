using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sones.Library
{
    public static class MessageUtility
    {
        public static void ValidateAsPipelineProperty(this string source, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                propertyName = "<propertyName is not specified>";
            }

            if (string.IsNullOrWhiteSpace(source))
            {
                throw new InvalidOperationException(string.Concat("The pipeline configuration property \"", propertyName, "\" is null, empty, or, whitespace. A value is required to be configured. Please check the pipeline configuration."));
            }
        }
    }
}

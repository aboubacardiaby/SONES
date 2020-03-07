using Microsoft.BizTalk.Component.Interop;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Sones.PipelineComponents
{
    public abstract  class PipelineComponentWithConfigurablePropertiesBase: IPersistPropertyBag
    {

        private static readonly ConcurrentDictionary<Type, XmlSerializer> Serializers = new ConcurrentDictionary<Type, XmlSerializer>();

        public abstract void GetClassID(out Guid classID);

        public abstract void InitNew();

        public void Load(IPropertyBag propertyBag, int errorLog)
        {
            var propertiesToLoad = this.GetPropertiesWithAttributeOfType<PipelineConfigurableStringAttribute>();
            this.LoadProperties<string>(propertyBag, propertiesToLoad);

            propertiesToLoad = this.GetPropertiesWithAttributeOfType<PipelineConfigurableBoolAttribute>();
            this.LoadProperties<bool>(propertyBag, propertiesToLoad);

            // the lines above this can all be removed when pipeline properties using the above to attributes converted to use PipelineConfigurable
            this.LoadPropertiesByInferringType(propertyBag);
        }

        private void LoadPropertiesByInferringType(IPropertyBag propertyBag)
        {
            foreach (var propertyInfo in this.GetPropertiesWithAttributeOfType<PipelineConfigurableAttribute>())
            {
                var propertyFromPropertyBag = ReadPropertyBag(propertyBag, propertyInfo.Name);

                if (propertyFromPropertyBag == null)
                {
                    continue;
                }

                if (propertyInfo.PropertyType == typeof(string) || propertyInfo.PropertyType.IsPrimitive)
                {
                    propertyInfo.SetValue(this, propertyFromPropertyBag, null);
                }
                else if (propertyInfo.PropertyType.IsSerializable)
                {
                    this.SetValueAsXmlDeserialised(propertyInfo, propertyFromPropertyBag.ToString());
                }
                else
                {
                    throw new SerializationException(propertyInfo.PropertyType + " must be string, primitive or serializiable to use the PipelineConfigurable Attribute");
                }
            }
        }

        private void SetValueAsXmlDeserialised(PropertyInfo propertyInfo, string property)
        {
            if (string.IsNullOrWhiteSpace(property))
            {
                propertyInfo.SetValue(this, null, null);
                return;
            }

            try
            {
                var serializer = Serializers.GetOrAdd(propertyInfo.PropertyType, new XmlSerializer(propertyInfo.PropertyType));

                using (var reader = new StringReader(property))
                {
                    var value = serializer.Deserialize(reader);
                    propertyInfo.SetValue(this, value, null);
                }
            }
            catch (Exception e)
            {
                var errorMessage = "Error when deserialising value: [" + property + "] within " + propertyInfo.Name + " to " + propertyInfo.PropertyType.Name;
               
               // throw new PipelineConfigurationException(errorMessage, e);
            }
        }

        private void LoadProperties<TUnderlyingType>(IPropertyBag propertyBag, IEnumerable<PropertyInfo> propertiesToLoad)
        {
            foreach (var propertyInfo in propertiesToLoad)
            {
                var propertyFromPropertyBag = ReadPropertyBag(propertyBag, propertyInfo.Name);

                if (propertyFromPropertyBag == null)
                {
                    continue;
                }

                propertyInfo.SetValue(this, (TUnderlyingType)propertyFromPropertyBag, null);
            }
        }

        public void Save(IPropertyBag propertyBag, bool clearDirty, bool saveAllProperties)
        {

            var stringPropertiesToSave = this.GetPropertiesWithAttributeOfType<PipelineConfigurableStringAttribute>();

            foreach (var propertyInfo in stringPropertiesToSave)
            {
                var val = propertyInfo.GetValue(this, null);
                WritePropertyBag(propertyBag, propertyInfo.Name, val);
            }

            var boolPropertiesToSave = this.GetPropertiesWithAttributeOfType<PipelineConfigurableBoolAttribute>();

            foreach (var propertyInfo in boolPropertiesToSave)
            {
                var val = propertyInfo.GetValue(this, null);
                WritePropertyBag(propertyBag, propertyInfo.Name, val);
            }

            //todo: the above can be deleted when typed attributes removed

            var propertiesToSave = this.GetPropertiesWithAttributeOfType<PipelineConfigurableAttribute>();

            foreach (var propertyInfo in propertiesToSave)
            {
                var val = propertyInfo.GetValue(this, null);
                if (propertyInfo.PropertyType == typeof(string) || propertyInfo.PropertyType.IsPrimitive)
                {
                    WritePropertyBag(propertyBag, propertyInfo.Name, val);
                }
                else if (propertyInfo.PropertyType.IsSerializable)
                {
                    var itemValue = SerialiseValue(propertyInfo, val);
                    WritePropertyBag(propertyBag, propertyInfo.Name, itemValue);
                }
                else
                {
                    throw new SerializationException(propertyInfo.PropertyType + " must be primitive or serializiable to use the PipelineConfigurable Attribute");
                }
            }
        }

        private static string SerialiseValue(PropertyInfo propertyInfo, object val)
        {
            var serializer = Serializers.GetOrAdd(propertyInfo.PropertyType, new XmlSerializer(propertyInfo.PropertyType));

            var sb = new StringBuilder();
            using (var stringWriter = new StringWriter(sb))
            {
                serializer.Serialize(stringWriter, val);
            }

            var itemValue = sb.ToString();
            return itemValue;
        }

        private IEnumerable<PropertyInfo> GetPropertiesWithAttributeOfType<T>()
        {
            return this.GetType()
                .GetProperties()
                .Where(x => x.GetCustomAttributes(true).Any(y => y.GetType() == typeof(T)));
        }

        private static object ReadPropertyBag(IPropertyBag propertyBag, string propertyName)
        {
            object itemValue;

            try
            {
                propertyBag.Read(propertyName, out itemValue, 0);
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

            return itemValue;
        }

        private static void WritePropertyBag(IPropertyBag propertyBag, string propertyName, object itemValue)
        {
            try
            {
                propertyBag.Write(propertyName, ref itemValue);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
 
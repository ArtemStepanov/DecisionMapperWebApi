using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using DecisionWebApi.Extensions;
using Newtonsoft.Json.Linq;

namespace DecisionWebApi.Helpers
{
    public class JObjectDeconstructor<T> : JObject where T : new()
    {
        public JObject EntityJObject { get; }
        public Type EntityType => typeof(T);

        public JObjectDeconstructor(JObject entityJObject)
        {
            EntityJObject = entityJObject;
        }

        public IEnumerable<PropertyInfo> GetRequiredProperties()
        {
            return EntityType.GetProperties().Where(prop => prop.HasCustomAttributeType<RequiredAttribute>());
        }

        public bool AllRequiredPropertiesForJObjectOfModelNotEmptyOrNull()
        {
            return GetRequiredProperties().ToList()
                .TrueForAll(property =>
                {
                    var value = EntityJObject.ContainsKey(property.Name)
                        ? EntityJObject.GetValue(property.Name).ToObject(property.PropertyType) : null;
                    return value != null && !string.IsNullOrWhiteSpace(value.ToString());
                });
        }

        public IDictionary<string, object> GetJObjectKeyToValueMap()
        {
            var dict = new Dictionary<string, object>();
            foreach (var element in EntityJObject)
            {
                var entityProperty = EntityType.GetProperties().FirstOrDefault(prop => prop.Name.Equals(element.Key));
                if (entityProperty == null)
                {
                    continue;
                }

                var isRequiredProperty = entityProperty.HasCustomAttributeType<RequiredAttribute>();
                var objectElementValue = element.Value.ToObject(entityProperty.PropertyType);
                if (isRequiredProperty && string.IsNullOrWhiteSpace(objectElementValue?.ToString()))
                {
                    continue;
                }

                dict.Add(element.Key, objectElementValue);
            }

            return dict;
        }

        public IDictionary<string, PropertyInfo> GetJObjectKeyToModelPropertyInfoMap()
        {
            var dict = new Dictionary<string, PropertyInfo>();
            
            foreach (var element in EntityJObject)
            {
                var entityProperty = EntityType.GetProperties().FirstOrDefault(prop => prop.Name.Equals(element.Key));
                if (entityProperty == null)
                {
                    continue;
                }

                dict.Add(element.Key, entityProperty);
            }

            return dict;
        }

        public T GetObjectInstance()
        {
            var instance = new T();
            foreach (var keyValueMap in GetJObjectKeyToValueMap())
            {
                var entityProperty = EntityType.GetProperty(keyValueMap.Key);
                var val = Convert.ChangeType(keyValueMap.Value, entityProperty.PropertyType);
                EntityType.GetProperty(keyValueMap.Key).SetValue(instance, val);
            }

            return instance;
        }

        public void Patch(T newEntity)
        {
            foreach (var keyValueMap in GetJObjectKeyToModelPropertyInfoMap())
            {
                var key = keyValueMap.Key;
                var propertyInfo = keyValueMap.Value;
                if (!EntityJObject.ContainsKey(key))
                {
                    continue;
                }

                var propertyType = propertyInfo.PropertyType;
                var value = EntityJObject.GetValue(key).ToObject(propertyType);
                if (GetRequiredProperties().Any(x => x.Name == key) && string.IsNullOrWhiteSpace(value?.ToString()))
                {
                    throw new ArgumentNullException();
                }

                propertyInfo.SetValue(newEntity, value);
            }
        }
    }
}
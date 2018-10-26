using System.Linq;
using System.Reflection;

namespace DecisionWebApi.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static bool HasCustomAttributeType<T>(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttributes(typeof(T)).Any();
        }
    }
}
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace Komponenty.Utilities
{
    public static class PropertyInfoExtensions
    {
        extension(PropertyInfo propertyInfo) 
        {
            /// <summary>
            /// Indicates whether this property is razor's component parameter by checking the [Parameter] attribute and whether the declaring type is shadowing this property. 
            /// </summary>
            public bool IsParameter => propertyInfo.IsPropertyParameter();
        }

        private static bool IsPropertyParameter(this PropertyInfo prop)
        {
            if (prop.GetCustomAttribute<ParameterAttribute>() is null)
            {
                return false;
            }

            Type? reflectedType = prop.ReflectedType;
            Type? declaringType = prop.DeclaringType;

            if (declaringType == reflectedType)
            {
                return true;
            }

            PropertyInfo? shadowed = reflectedType?.GetProperty(prop.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

            return shadowed is null ? true : IsPropertyParameter(shadowed);
        }
    }
}

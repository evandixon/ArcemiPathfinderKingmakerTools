using Arcemi.Pathfinder.Kingmaker.GameData.Blueprints;
using Newtonsoft.Json.Linq;
using System;

namespace Arcemi.Pathfinder.Kingmaker.Infrastructure.Extensions
{
    public static class JTokenExtensions
    {
        public static T ValueWithCustomConverters<T>(this JToken jToken)
        {
            var targetType = typeof(T);

            // In the future this could be made generic by checking for a custom attribute,
            // but for now there's few enough special cases that manually checking types is fine
            if (targetType.IsAssignableTo(typeof(BlueprintReference)))
            {
                var id = jToken.Value<string>();
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(BlueprintReference<>))
                {
                    var value = targetType.GetConstructor(new Type[] { typeof(string) }).Invoke(new object[] { id });
                    return (T)value;
                }
                else
                {
                    var value = (object)new BlueprintReference(id);
                    return (T)value;
                }
            }

            return jToken.Value<T>();
        }
    }
}

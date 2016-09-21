using System;
using System.Linq;
using System.Reflection;

namespace TwentyTwenty.DomainDriven
{
    public static class ReflectionExtensions
    {
        public static T CloseAndBuildAs<T>(this Type openType, params Type[] parameterTypes)
        {
            var closedType = openType.MakeGenericType(parameterTypes);
            return (T)Activator.CreateInstance(closedType);
        }

        public static T CloseAndBuildAs<T>(this Type openType, object ctorArgument, params Type[] parameterTypes)
        {
            var closedType = openType.MakeGenericType(parameterTypes);
            return (T)Activator.CreateInstance(closedType, ctorArgument);
        }

        /// <summary>
        /// Does a hard cast of the object to T.  *Will* throw InvalidCastException
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static T As<T>(this object target)
        {
            return (T)target;
        }

        public static object InvokeGeneric<T>(this T obj, string methodName, Type[] typeParams, params object[] parameters)
        {
            var objType = typeof(T).GetTypeInfo();
            var method = objType.GetDeclaredMethod(methodName);
            var genericMethod = method.MakeGenericMethod(typeParams);
            return genericMethod.Invoke(obj, parameters);
        }

        public static bool IsAssignableFromGeneric(this Type genericType, Type givenType)
        {
            var interfaceTypes = givenType.GetInterfaces()
                .Select(t => t.GetTypeInfo());

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenType.GetTypeInfo().IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            var baseType = givenType.GetTypeInfo().BaseType;
            if (baseType == null)
            {
                return false;
            }

            return IsAssignableFromGeneric(genericType, baseType);
        }
    }
}
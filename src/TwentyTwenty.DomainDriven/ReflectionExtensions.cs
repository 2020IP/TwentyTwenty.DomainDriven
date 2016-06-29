using System;
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
    }
}
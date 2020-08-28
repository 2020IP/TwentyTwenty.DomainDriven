using System;
using System.Linq;
using System.Reflection;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MassTransit.Util;
using TwentyTwenty.DomainDriven.CQRS;

namespace TwentyTwenty.DomainDriven.MassTransit
{
    public static class MassTransitExtensions
    {
        public static Type GetMessageType(this Type handlerType)
        {
            var closedType = handlerType.FindInterfaceThatCloses(typeof(IConsumer<>));
            return closedType == null ? null : closedType.GenericTypeArguments.First();        
        }

        public static void MapEndpointConventions(this IRabbitMqBusFactoryConfigurator cfg, string rabbitMqUri, params Type[] markerTypes)
            => MapEndpointConventions(cfg, rabbitMqUri, markerTypes.Select(t => t.GetTypeInfo().Assembly).ToArray());

        public static void MapEndpointConventions(this IRabbitMqBusFactoryConfigurator cfg, string rabbitMqUri, params Assembly[] assemblies)
        {
            var types = AssemblyTypeCache.FindTypes(assemblies, t =>
            {
                var info = t.GetTypeInfo();
                return !info.IsAbstract && !info.IsInterface && typeof(ICommand).IsAssignableFrom(t);
            }).Result.AllTypes();

            var mapMethod = typeof(EndpointConvention)
                .GetMethod("Map", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(Uri) }, null);

            foreach (var type in types)
            {
                var generic = mapMethod.MakeGenericMethod(type);
                generic.Invoke(null, new object[] { new Uri($"{rabbitMqUri}/{type.Name}") });
            }
        }

        private static Type FindInterfaceThatCloses(this Type type, Type openType)
        {
            if (type == typeof(object)) return null;

            var typeInfo = type.GetTypeInfo();

            if (typeInfo.IsInterface && typeInfo.IsGenericType && type.GetGenericTypeDefinition() == openType)
                return type;


            foreach (var interfaceType in type.GetInterfaces())
            {
                var interfaceTypeInfo = interfaceType.GetTypeInfo();
                if (interfaceTypeInfo.IsGenericType && interfaceType.GetGenericTypeDefinition() == openType)
                {
                    return interfaceType;
                }
            }

            if (typeInfo.IsInterface || typeInfo.IsAbstract) return null;

            return typeInfo.BaseType == typeof(object)
                ? null
                : typeInfo.BaseType.FindInterfaceThatCloses(openType);
        }
    }
}
using System;
using System.Linq;
using System.Reflection;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
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

        public static void AddConsumers(this IServiceCollectionConfigurator opt, params Type[] markerTypes)
            => AddConsumers(opt, markerTypes.Select(t => t.GetTypeInfo().Assembly).ToArray());

        public static void AddConsumers(this IRegistrationConfigurator opt, params Assembly[] assemblies)
        {
            var types = AssemblyTypeCache.FindTypes(assemblies, t =>
            {
                var info = t.GetTypeInfo();
                return !info.IsAbstract && !info.IsInterface && typeof(IConsumer).IsAssignableFrom(t);
            }).Result.AllTypes();

            foreach (var type in types)
            {
                opt.AddConsumer(type);
            }
        }

        public static void MapEndpointConventions(this IRabbitMqBusFactoryConfigurator cfg, string rabbitMqUri, params Type[] markerTypes)
            => MapEndpointConventions(cfg, rabbitMqUri, markerTypes.Select(t => t.GetTypeInfo().Assembly).ToArray());

        public static void MapEndpointConventions(this IRabbitMqBusFactoryConfigurator cfg, string rabbitMqUri, params Assembly[] assemblies)
        {
            var types = AssemblyTypeCache.FindTypes(assemblies, t =>
            {
                var info = t.GetTypeInfo();
                return !info.IsAbstract && !info.IsInterface && typeof(IConsumer).IsAssignableFrom(t);
            }).Result.AllTypes();

            var mapMethod = typeof(EndpointConvention)
                .GetMethod("Map", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(Uri) }, null);

            foreach (var type in types)
            {
                var messageType = type.GetMessageType();
                if (typeof(ICommand).IsAssignableFrom(messageType))
                {
                    var generic = mapMethod.MakeGenericMethod(messageType);
                    generic.Invoke(null, new object[] { new Uri($"{rabbitMqUri}/{messageType.Name}") });
                }
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
using System;
using System.Linq;
using System.Reflection;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.RabbitMqTransport;
using MassTransit.Util;
using Microsoft.Extensions.DependencyInjection;
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
            => AddConsumers(opt, false, null, markerTypes.Select(t => t.GetTypeInfo().Assembly).ToArray());

            public static void AddConsumers(this IServiceCollectionConfigurator opt, params Assembly[] assemblies)
            => AddConsumers(opt, false, null, assemblies);

        public static void AddConsumers(this IServiceCollectionConfigurator opt, bool mapDefaultEndpoints, string rabbitMqUri, params Type[] markerTypes)
            => AddConsumers(opt, mapDefaultEndpoints, rabbitMqUri, markerTypes.Select(t => t.GetTypeInfo().Assembly).ToArray());

        public static void AddConsumers(this IServiceCollectionConfigurator opt, bool mapDefaultEndpoints, string rabbitMqUri, params Assembly[] assemblies)
        {
            var types = AssemblyTypeCache.FindTypes(assemblies, t =>
            {
                var info = t.GetTypeInfo();
                return !info.IsAbstract && !info.IsInterface && typeof(IConsumer).IsAssignableFrom(t);
            }).Result.AllTypes();

            foreach (var type in types)
            {
                if (mapDefaultEndpoints && !string.IsNullOrEmpty(rabbitMqUri))
                {
                    var messageType = type.GetMessageType();
                    var method = typeof(EndpointConvention)
                        .MakeGenericType(messageType)
                        .GetMethod("Map", BindingFlags.Static | BindingFlags.Public);
                    method.Invoke(null, new object[] { null, new Uri($"{rabbitMqUri}/{nameof(messageType)}") });
                }

                opt.InvokeGeneric("AddConsumer", new[] { type });
            }
        }
        
        public static IRabbitMqBusFactoryConfigurator AddEventReceiveEndpoints(this IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host, IServiceProvider services)
        {
            var cache = services.GetRequiredService<IConsumerCacheService>();

            var eventHandlers = cache.GetConfigurators()
                .Select(c => new { HandlerType = c.GetType().GetGenericArguments().First(), Configurator = c, })
                .Where(c => !typeof(ICommand).IsAssignableFrom(c.HandlerType.GetMessageType()));

            foreach (var handler in eventHandlers)
            {
                configurator.ReceiveEndpoint(host, handler.HandlerType.Name, c => handler.Configurator.Configure(c, services));
            }

            return configurator;
        }

        public static IRabbitMqBusFactoryConfigurator AddCommandReceiveEndpoints(this IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host, IServiceProvider services)
        {
            var cache = services.GetRequiredService<IConsumerCacheService>();

            var commandHandlers = cache.GetConfigurators()
                .Select(c => new { MessageType = c.GetType().GetGenericArguments().First().GetMessageType(), Configurator = c, })
                .Where(c => typeof(ICommand).IsAssignableFrom(c.MessageType));

            foreach (var handler in commandHandlers)
            {
                configurator.ReceiveEndpoint(host, handler.MessageType.Name, c => handler.Configurator.Configure(c, services));
            }

            return configurator;
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
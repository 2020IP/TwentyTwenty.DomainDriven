using System;
using System.Threading.Tasks;
using MassTransit;
using GreenPipes;
using TwentyTwenty.DomainDriven;
using TwentyTwenty.DomainDriven.CQRS;
using System.Linq;
using System.Threading;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MassTransit.RabbitMqTransport;

namespace TwentyTwenty.DomainDriven.MassTransit
{
    public class MassTransitMessageBus : IEventPublisher, ICommandSender, ICommandSenderReceiver
    {
        private readonly ILogger<MassTransitMessageBus> _logger;
        private readonly MassTransitMessageBusOptions _options;
        private readonly IBus _bus;
        private readonly Func<Type, Uri> _getSendAddress;

        public MassTransitMessageBus(MassTransitMessageBusOptions options, IBus bus, ILogger<MassTransitMessageBus> logger, Func<Type, Uri> getSendAddress)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (bus == null)
            {
                throw new ArgumentNullException(nameof(bus));
            }
            if (logger == null)
            {
                throw new ArgumentException(nameof(logger));
            }
            if (getSendAddress == null)
            {
                throw new ArgumentException(nameof(getSendAddress));
            }

            _logger = logger;
            _options = options;
            _bus = bus;
            _getSendAddress = getSendAddress;
        }

        public virtual Task Publish(IDomainEvent @event, Type eventType)
            => _bus.Publish(@event, eventType);

        public virtual Task Publish(IDomainEvent @event)
            => _bus.Publish(@event, @event.GetType());

        public virtual async Task Send(ICommand command)
        {
            var commandType = command.GetType();
            var address = _getSendAddress(commandType);
            var endpoint = await _bus.GetSendEndpoint(address).ConfigureAwait(false);
            await endpoint.Send(command, commandType).ConfigureAwait(false);
        }

        public virtual async Task Send(ICommand command, Type commandType)
        {
            var address = _getSendAddress(commandType);
            var endpoint = await _bus.GetSendEndpoint(address).ConfigureAwait(false);

            await endpoint.Send(command, commandType).ConfigureAwait(false);
        }

        public virtual async Task<TResult> Send<TResult>(ICommand command) where TResult : class, IResponse
        {
            var type = command.GetType();
            var address = _getSendAddress(type);
            var invoker = typeof(RequestResponseInvoker<,>).CloseAndBuildAs<IRequestResponseInvoker<TResult>>(type, typeof(TResult));

            return await invoker.Request(command, _bus, address, TimeSpan.FromSeconds(30));
        }

        public virtual async Task<TResult> Send<TResult>(ICommand command, Type commandType) where TResult : class, IResponse
        {
            var address = _getSendAddress(commandType);
            var invoker = typeof(RequestResponseInvoker<,>).CloseAndBuildAs<IRequestResponseInvoker<TResult>>(commandType, typeof(TResult));
            
            return await invoker.Request(command, _bus, address, TimeSpan.FromSeconds(30));
        }

        interface IRequestResponseInvoker<TResponse>
        {
            Task<TResponse> Request(ICommand command, IBus bus, Uri address, TimeSpan timeout, CancellationToken cancellationToken = default(CancellationToken));
        }

        class RequestResponseInvoker<TRequest, TResponse> : IRequestResponseInvoker<TResponse>
            where TRequest : class
            where TResponse : class
        {
            public Task<TResponse> Request(ICommand command, IBus bus, Uri address, TimeSpan timeout, CancellationToken cancellationToken = default(CancellationToken))
            {
                return new MessageRequestClient<TRequest, TResponse>(bus, address, timeout)
                    .Request((TRequest)command, cancellationToken);
            }
        }
    }
}
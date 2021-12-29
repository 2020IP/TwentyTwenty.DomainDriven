using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TwentyTwenty.DomainDriven.CQRS;
using TwentyTwenty.DomainDriven.EventPublishing;

namespace TwentyTwenty.DomainDriven.InMemory
{
    public class InProcessBus : 
        ICommandSender, 
        ICommandSenderReceiver, 
        IEventPublisher, 
        IHandlerRegistrar,
        IHandlerRequestResponseRegistrar
    {
        private readonly Dictionary<Type, List<Action<IMessage>>> _routes = new Dictionary<Type, List<Action<IMessage>>>();
        private readonly Dictionary<Type, List<Func<IMessage, Task<object>>>> _responseRoutes = new Dictionary<Type, List<Func<IMessage, Task<object>>>>();

        public void RegisterHandler<T>(Action<T> handler) where T : class, IMessage
        {
            if (!_routes.TryGetValue(typeof(T), out List<Action<IMessage>> handlers))
            {
                handlers = new List<Action<IMessage>>();
                _routes.Add(typeof(T), handlers);
            }
            handlers.Add(x => handler((T)x));
        }

        public void RegisterHandler<T, TResult>(Func<T, Task<TResult>> handler)
            where T : class, IMessage
            where TResult : class, IResponse
        {
            if (!_responseRoutes.TryGetValue(typeof(T), out List<Func<IMessage, Task<object>>> handlers))
            {
                handlers = new List<Func<IMessage, Task<object>>>();
                _responseRoutes.Add(typeof(T), handlers);
            }
            handlers.Add(x => handler((T)x).ContinueWith(r => (object)r.Result));
        }

        public Task Send(ICommand command, CancellationToken token = default)
            => Send(command, command.GetType(), token);

        public Task Send(ICommand command, Type commandType, CancellationToken token = default)
        {
            if (_routes.TryGetValue(commandType, out List<Action<IMessage>> handlers))
            {
                if (handlers.Count != 1)
                    throw new InvalidOperationException("Cannot send to more than one handler");
                handlers[0](command);

                return Task.FromResult(false);
            }
            else
            {
                throw new InvalidOperationException("No handler registered");
            }
        }

        public Task<TResult> Send<TResult>(ICommand command, CancellationToken token = default)
            where TResult : class, IResponse
            => Send<TResult>(command, command.GetType(), token);

        public Task<TResult> Send<TResult>(ICommand command, Type commandType, CancellationToken token = default)
            where TResult : class, IResponse
        {
            if (_responseRoutes.TryGetValue(commandType, out List<Func<IMessage, Task<object>>> handlers))
            {
                if (handlers.Count != 1)
                    throw new InvalidOperationException("Cannot send to more than one handler");

                return handlers[0](command).ContinueWith(r => (TResult)r.Result);
            }

            throw new InvalidOperationException("No handler registered");
        }

        public Task Publish(IDomainEvent @event, CancellationToken token = default)
            => Publish(@event, @event.GetType(), token);

        public Task Publish(IDomainEvent @event, Type eventType, CancellationToken token = default)
        {
            if (!_routes.TryGetValue(eventType, out List<Action<IMessage>> handlers))
            {
                return Task.FromResult(false);
            }

            // TODO: Make this invocation async.
            foreach (var handler in handlers)
            {
                handler(@event);
            }

            return Task.FromResult(false);
        }
    }
}

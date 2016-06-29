using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.CQRS
{
    public class InProcessBus : ICommandSender, IEventPublisher, IHandlerRegistrar
    {
        private readonly Dictionary<Type, List<Action<IMessage>>> _routes = new Dictionary<Type, List<Action<IMessage>>>();

        public void RegisterHandler<T>(Action<T> handler) where T : class, IMessage
        {
            List<Action<IMessage>> handlers;
            if (!_routes.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Action<IMessage>>();
                _routes.Add(typeof(T), handlers);
            }
            handlers.Add((x => handler((T)x)));
        }

        public Task Send<T>(T command) where T : class, ICommand
        {
            List<Action<IMessage>> handlers;
            if (_routes.TryGetValue(typeof(T), out handlers))
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

        public Task Publish<T>(T @event) where T : class, IDomainEvent
        {
            List<Action<IMessage>> handlers;
            if (!_routes.TryGetValue(@event.GetType(), out handlers))
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

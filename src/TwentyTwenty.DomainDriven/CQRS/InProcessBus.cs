using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.CQRS
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
            List<Action<IMessage>> handlers;
            if (!_routes.TryGetValue(typeof(T), out handlers))
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
            List<Func<IMessage, Task<object>>> handlers;
            if (!_responseRoutes.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<Func<IMessage, Task<object>>>();
                _responseRoutes.Add(typeof(T), handlers);
            }
            handlers.Add(x => handler((T)x).ContinueWith(r => (object)r.Result));
        }

        public Task Send(ICommand command)
            => Send(command, command.GetType());

        public Task Send(ICommand command, Type commandType)
        {
            List<Action<IMessage>> handlers;
            if (_routes.TryGetValue(commandType, out handlers))
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

        public Task<TResult> Send<TResult>(ICommand command)
            where TResult : class, IResponse
            => Send<TResult>(command, command.GetType());

        public Task<TResult> Send<TResult>(ICommand command, Type commandType)
            where TResult : class, IResponse
        {
            List<Func<IMessage, Task<object>>> handlers;
            if (_responseRoutes.TryGetValue(commandType, out handlers))
            {
                if (handlers.Count != 1)
                    throw new InvalidOperationException("Cannot send to more than one handler");
                
                return handlers[0](command).ContinueWith(r => (TResult)r.Result);
            }
            
            throw new InvalidOperationException("No handler registered");
        }

        public Task Publish(IDomainEvent @event)
            => Publish(@event, @event.GetType());

        public Task Publish(IDomainEvent @event, Type eventType)
        {
            List<Action<IMessage>> handlers;
            if (!_routes.TryGetValue(eventType, out handlers))
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

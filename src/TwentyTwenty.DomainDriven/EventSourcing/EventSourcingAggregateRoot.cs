using System;
using System.Reflection;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public abstract class EventSourcingAggregateRoot<TAggregate> : Entity<Guid>, IAggregateRoot<Guid> 
        where TAggregate : EventSourcingAggregateRoot<TAggregate>
    {
        public const string ApplyMethod = "Apply";
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>> HandlerCache = 
            new ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>>(); 
        private readonly List<IDomainEvent> _changes = new List<IDomainEvent>();

        public int Version { get; protected set; }

        public IEnumerable<IDomainEvent> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        public void LoadChangesFromHistory(IEnumerable<IDomainEvent> history)
        {
            foreach (var e in history)
            {
                ApplyChange(e, false);
            }
        }

        protected void ApplyChange(IDomainEvent @event)
        {
            ApplyChange(@event, true);
        }

        private void ApplyChange(IDomainEvent @event, bool isNew)
        {
            var apply = GetApply(@event);
            
            apply((TAggregate)this, @event);
            
            if (isNew)
            {
                _changes.Add(@event);
            }
            
            Version++;
        }

        private Action<TAggregate, IDomainEvent> GetApply(IDomainEvent @event)
        {
            var thisType = typeof(TAggregate);
            var eventType = @event.GetType();

            ConcurrentDictionary<Type, object> cache = null;

            if (!HandlerCache.TryGetValue(thisType, out cache))
            {
                cache = CreateCache(thisType);
                HandlerCache[thisType] = cache;
            }

            if(!cache.ContainsKey(eventType))
            {
                throw new Exception("Apply method not found");
            }

            return cache[eventType].As<Action<TAggregate, IDomainEvent>>();
        }

        private static ConcurrentDictionary<Type, object> CreateCache(Type aggregateType)
        {
            var handlers = aggregateType
                .GetTypeInfo()
                .DeclaredMethods
                .Where(m => m.Name == ApplyMethod);
            
            var cache = new ConcurrentDictionary<Type, object>();
            
            foreach (var handlerMethod in handlers)
            {
                var param = handlerMethod.GetParameters();

                if (param.Length == 1)
                {
                    var eventType = param.First().ParameterType;
                    cache[eventType] = BuildApply(handlerMethod, eventType);
                }
            }
            
            return cache;
        }

        public static Action<TAggregate, IDomainEvent> BuildApply(MethodInfo method, Type eventType)
        {
            var aggregateParameter = Expression.Parameter(typeof(TAggregate), "a");
            var eventParameter = Expression.Parameter(typeof(IDomainEvent), "e");

            var castEvent = Expression.Convert(eventParameter, eventType);
            var body = Expression.Call(aggregateParameter, method, castEvent);
                        
            var lambda = Expression.Lambda<Action<TAggregate, IDomainEvent>>(body, aggregateParameter, eventParameter);

            return lambda.Compile();
        }
    }
}
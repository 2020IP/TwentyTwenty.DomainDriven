using System;
using System.Reflection;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using TwentyTwenty.DomainDriven.EventPublishing;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public abstract class EventSourcingAggregateRoot<TAggregate, TId> : EventPublishingAggregateRoot<TId>, IEventSourcingAggregateRoot<TId>
        where TAggregate : EventSourcingAggregateRoot<TAggregate, TId>
    {
        public const string ApplyMethod = "Apply";
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>> HandlerCache = 
            new ConcurrentDictionary<Type, ConcurrentDictionary<Type, object>>(); 
        public int Version { get; protected set; }

        public void LoadChangesFromHistory(IEnumerable<IDomainEvent> history)
        {
            foreach (var e in history)
            {
                ApplyChange(e, false);
            }
        }

        protected override void AddEvent(IDomainEvent @event)
        {
            ApplyChange(@event, true);
        }

        private void ApplyChange(IDomainEvent @event, bool isNew)
        {
            var apply = GetApply(@event);
            
            apply((TAggregate)this, @event);
            
            if (isNew)
            {
                _uncommittedEvents.Add(@event);
            }
            
            Version++;
        }

        private Action<TAggregate, IDomainEvent> GetApply(IDomainEvent @event)
        {
            var thisType = typeof(TAggregate);
            var eventType = @event.GetType();

            if (!HandlerCache.TryGetValue(thisType, out ConcurrentDictionary<Type, object> cache))
            {
                cache = CreateCache(thisType);
                HandlerCache[thisType] = cache;
            }

            if (!cache.ContainsKey(eventType))
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
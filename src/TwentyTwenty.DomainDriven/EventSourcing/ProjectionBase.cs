using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public abstract class ProjectionBase : IProjection
    {
        public const string ApplyMethod = "Apply";
        private readonly IDictionary<Type, Action<IDomainEvent>> _handlers = new Dictionary<Type, Action<IDomainEvent>>();

        public ProjectionBase()
        {
            var methods = GetType().GetMethods()
                .Where(x => x.Name == ApplyMethod && x.GetParameters().Length == 1)
                .Where(x => typeof(IDomainEvent).IsAssignableFrom(x.GetParameters().Single().ParameterType));

            foreach (var method in methods)
            {
                var eventType = method.GetParameters().Single().ParameterType;
                _handlers.Add(eventType, GetApplyDelegate(method, eventType));
            };
        }

        public void ApplyEvents(IEnumerable<IDomainEvent> events)
        {
            Action<IDomainEvent> action;

            foreach (var @event in events)
            {
                if (_handlers.TryGetValue(@event.GetType(), out action))
                {
                    action(@event);
                }
            }
        }

        private Action<IDomainEvent> GetApplyDelegate(MethodInfo method, Type eventType)
        {
            var eventParameter = Expression.Parameter(typeof(IDomainEvent), "e");
            var instance = Expression.Constant(this);

            var cast = Expression.Convert(eventParameter, eventType);
            var body = Expression.Call(instance, method, cast);

            var lambda = Expression.Lambda<Action<IDomainEvent>>(body, eventParameter);

            return lambda.Compile();
        }
    }

    //public class ProjectionStep<T, TEvent> where TEvent : IDomainEvent
    //{
    //    private readonly Action<T, TEvent> _apply;

    //    public ProjectionStep(MethodInfo method)
    //    {
    //        if (method.GetParameters().Length != 1
    //            || method.GetParameters().Single().ParameterType != typeof(TEvent)
    //            || method.DeclaringType != typeof(T))
    //        {
    //            throw new ArgumentOutOfRangeException($"Method {method.Name} on {method.DeclaringType} cannot be used as an projection apply method");
    //        }

    //        var projectionParameter = Expression.Parameter(typeof(T), "a");
    //        var eventParameter = Expression.Parameter(typeof(TEvent), "e");

    //        var body = Expression.Call(projectionParameter, method, eventParameter);

    //        var lambda = Expression.Lambda<Action<T, TEvent>>(body, projectionParameter, eventParameter);

    //        _apply = lambda.Compile();
    //    }

    //    public ProjectionStep(Action<T, TEvent> apply)
    //    {
    //        _apply = apply;
    //    }

    //    public void Apply(T projection, TEvent @event)
    //    {
    //        _apply(projection, @event);
    //    }
    //}
}
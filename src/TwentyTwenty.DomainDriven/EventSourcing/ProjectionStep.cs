using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public class ProjectionStep<T, TEvent> : IProjectionStep<T, TEvent>
    {
        private readonly Action<T, TEvent> _apply;

        public ProjectionStep(MethodInfo method)
        {
            if (method.GetParameters().Length != 1
                || method.GetParameters().Single().ParameterType != typeof(TEvent)
                || method.DeclaringType != typeof(T))
            {
                throw new ArgumentOutOfRangeException($"Method {method.Name} on {method.DeclaringType} cannot be used as an aggregation method");
            }

            var aggregateParameter = Expression.Parameter(typeof(T), "a");
            var eventParameter = Expression.Parameter(typeof(TEvent), "e");

            var body = Expression.Call(aggregateParameter, method, eventParameter);

            var lambda = Expression.Lambda<Action<T, TEvent>>(body, aggregateParameter, eventParameter);

            _apply = lambda.Compile();
        }

        public ProjectionStep(Action<T, TEvent> apply)
        {
            _apply = apply;
        }

        public void Apply(T aggregate, TEvent @event)
        {
            _apply(aggregate, @event);
        }
    }
}
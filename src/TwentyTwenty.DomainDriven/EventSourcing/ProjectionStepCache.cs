using System;
using System.Linq;
using System.Collections.Generic;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public class ProjectionStepCache<TProjection> : IProjectionStepCache<TProjection> 
        where TProjection : IProjection
    {
        private readonly IDictionary<Type, object> _handlers = new Dictionary<Type, object>();
        private readonly Type[] _eventTypes;
	
        public ProjectionStepCache(string methodName = "Apply")
        {
            var eventTypes = new List<Type>();
            var methods = typeof(TProjection).GetMethods()
                .Where(x => x.Name == methodName && x.GetParameters().Length == 1)
                .Select(x => new { ParamType = x.GetParameters().First().ParameterType, Method = x });
            
            foreach(var method in methods)
            {
                // Could really use pattern matching here...
                if(typeof(IDomainEvent).IsAssignableFrom(method.ParamType))
                {
                    eventTypes.Add(method.ParamType);
                }
                else if(typeof(IEventDescriptor<>).IsAssignableFromGeneric(method.ParamType))
                {
                    var eventType = method.ParamType.GenericTypeArguments.First();
                    eventTypes.Add(eventType);                                        
                }                
                else
                {
                    // No other Apply method type supported.
                    continue;                    
                }

                _handlers[method.ParamType] = typeof(ProjectionStep<,>)
                    .CloseAndBuildAs<object>(method.Method, typeof(TProjection), method.ParamType);
            }

            _eventTypes = eventTypes.Distinct().ToArray();                
        }
        
        public Type ProjectionType => typeof(TProjection);

        public Type[] ConsumedEvents => _eventTypes;

        public IProjectionStep<TProjection, TEvent> StepFor<TEvent>()
        {
            object step;

            _handlers.TryGetValue(typeof(TEvent), out step);

            return step?.As<IProjectionStep<TProjection, TEvent>>();
        }
    }
}
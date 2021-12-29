using System;
using System.Collections.Generic;
using System.Linq;
using TwentyTwenty.DomainDriven.EventSourcing;

namespace TwentyTwenty.DomainDriven.Projections
{
    public abstract class ProjectionBase<T> : IProjection
        where T : class, IProjection
    {
        private static readonly IProjectionStepCache<T> Cache = new ProjectionStepCache<T>();
        
        public Type[] Consumes
            => Cache.ConsumedEvents;
        
        public virtual void ApplyEvents(IEnumerable<ICanApply> eventDescriptors)
        {            
            foreach (var descriptor in eventDescriptors)
            {
                descriptor.Apply(this.As<T>(), Cache);
            }
        }

        void IProjection.ApplyEvents(IEnumerable<IEventDescriptor> eventDescriptors)
        {            
            ApplyEvents(eventDescriptors.OfType<ICanApply>());
        }
    }
}
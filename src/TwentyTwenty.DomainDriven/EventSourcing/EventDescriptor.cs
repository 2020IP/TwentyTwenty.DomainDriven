using System;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public abstract class EventDescriptor<TEvent> : ICanApply, IEventDescriptor<TEvent>
        where TEvent : IDomainEvent
    {
        public Guid Id { get; protected set; }

        public int Version { get; protected set; }

        public DateTime TimeStamp { get; protected set; }

        public TEvent Data { get; protected set; }

        IDomainEvent IEventDescriptor.Data => Data;

        public virtual void Apply<TProjection>(TProjection state, IProjectionStepCache<TProjection> cache)
            where TProjection : class, IProjection
        {            
            cache.StepFor<TEvent>()?.Apply(state, Data);
            cache.StepFor<IEventDescriptor<TEvent>>()?.Apply(state, this);
        }
    }    
}
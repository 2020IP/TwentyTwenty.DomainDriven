using System;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IEventDescriptor
    {
        Guid Id { get; }
        long Version { get; }
        DateTime TimeStamp { get; }
        IDomainEvent Data { get; }
    }

    public interface IEventDescriptor<TEvent> : IEventDescriptor
        where TEvent : IDomainEvent
    {
        new TEvent Data { get; }        
    }

    public interface ICanApply 
    {
        void Apply<TProjection>(TProjection state, IProjectionStepCache<TProjection> cache)
            where TProjection : class, IProjection;
    }
}
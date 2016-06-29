using System;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IEventSourcingRepository
    {
        void Save<T>(T aggregate, int? expectedVersion = null) where T : EventSourcingAggregateRoot<T>, new();

        T GetById<T>(Guid id) where T : EventSourcingAggregateRoot<T>, new();
    }
}

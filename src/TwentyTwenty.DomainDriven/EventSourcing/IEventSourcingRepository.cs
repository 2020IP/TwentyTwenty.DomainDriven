using System;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IEventSourcingRepository
    {
        void Save<T>(T aggregate, int? expectedVersion = null) where T : EventSourcingAggregateRoot<T>, new();
        
        Task SaveAsync<T>(T aggregate, int? expectedVersion = null) where T : EventSourcingAggregateRoot<T>, new();

        T GetById<T>(Guid id) where T : EventSourcingAggregateRoot<T>, new();

        Task<T> GetByIdAsync<T>(Guid id) where T : EventSourcingAggregateRoot<T>, new();
    }
}
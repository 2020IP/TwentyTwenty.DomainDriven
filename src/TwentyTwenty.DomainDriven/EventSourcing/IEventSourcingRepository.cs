using System;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IEventSourcingRepository<TId>
    {
        void Save<T>(T aggregate, int? expectedVersion = null) 
            where T : IEventSourcingAggregateRoot<TId>, new();
        
        Task SaveAsync<T>(T aggregate, int? expectedVersion = null) 
            where T : IEventSourcingAggregateRoot<TId>, new();

        T GetById<T>(TId id) 
            where T : IEventSourcingAggregateRoot<TId>, new();

        Task<T> GetByIdAsync<T>(TId id) 
            where T : IEventSourcingAggregateRoot<TId>, new();
    }
}
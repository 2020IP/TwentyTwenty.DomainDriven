using System;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventPublishing
{
    public interface IEventPublishingRepository<TId>
    {
        T Save<T>(T entity) 
            where T : class, IEventPublishingAggregateRoot<TId>, new();

        Task<T> SaveAsync<T>(T entity) 
            where T : class, IEventPublishingAggregateRoot<TId>, new();

        T GetById<T>(TId id) 
            where T : class, IEventPublishingAggregateRoot<TId>, new();

        Task<T> GetByIdAsync<T>(Guid id) 
            where T : class, IEventPublishingAggregateRoot<TId>, new();

        void Delete<T>(T entity) 
            where T : class, IEventPublishingAggregateRoot<TId>, new();

        Task DeleteAsync<T>(T entity) 
            where T : class, IEventPublishingAggregateRoot<TId>, new();
    }
}
using System;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventPublishing
{
    public interface IEventPublishingRepository
    {
        T Save<T>(T entity) where T : EventPublishingAggregateRoot<T>, new();

        Task<T> SaveAsync<T>(T entity) where T : EventPublishingAggregateRoot<T>, new();

        T GetById<T>(Guid id) where T : EventPublishingAggregateRoot<T>, new();

        Task<T> GetByIdAsync<T>(Guid id) where T : EventPublishingAggregateRoot<T>, new();
    }
}
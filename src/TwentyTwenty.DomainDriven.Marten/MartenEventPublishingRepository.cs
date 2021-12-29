using Marten;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TwentyTwenty.DomainDriven.EventPublishing;

namespace TwentyTwenty.DomainDriven.Marten
{
    public class MartenEventPublishingRepository : IEventPublishingRepository<Guid>
    {
        private readonly IDocumentSession _database;
        private readonly IEventPublisher _eventPublisher;

        public MartenEventPublishingRepository(IDocumentSession database, IEventPublisher eventPublisher)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _eventPublisher = eventPublisher;
        }

        public Task<T> GetById<T>(Guid id, CancellationToken token = default) 
            where T : class, IEventPublishingAggregateRoot<Guid>, new()
        {
            return _database.LoadAsync<T>(id, token);
        }

        public async Task Save<T>(T aggregate, CancellationToken token = default)
            where T : class, IEventPublishingAggregateRoot<Guid>, new()
        {
            _database.Store(aggregate);
            await _database.SaveChangesAsync(token);
            await PublishEvents(aggregate, token);
        }

        public async Task Save<T>(IEnumerable<T> aggregates, CancellationToken token = default) 
            where T : class, IEventPublishingAggregateRoot<Guid>, new()
        {
            _database.Store(aggregates);
            await _database.SaveChangesAsync(token);
            await PublishEvents(aggregates, token);
        }

        public async Task Delete<T>(T aggregate, CancellationToken token = default) 
            where T : class, IEventPublishingAggregateRoot<Guid>, new()
        {
            _database.Delete(aggregate);
            await _database.SaveChangesAsync(token);
            await PublishEvents(aggregate, token);
        }

        private async Task PublishEvents<T>(T aggregate, CancellationToken token = default)
            where T : class, IEventPublishingAggregateRoot<Guid>, new()
        {
            if (_eventPublisher != null)
            {
                var uncommittedEvents = aggregate.GetUncommittedEvents();
                foreach (var uncommittedEvent in uncommittedEvents)
                {
                    await _eventPublisher.Publish(uncommittedEvent, token);
                }
            }

            aggregate.ClearUncommittedEvents();
        }

        private async Task PublishEvents<T>(IEnumerable<T> aggregates, CancellationToken token = default)
            where T : class, IEventPublishingAggregateRoot<Guid>, new()
        {
            foreach (var aggregate in aggregates)
            {
                await PublishEvents(aggregate, token);
            }
        }
    }
}
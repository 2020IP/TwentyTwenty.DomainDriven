using Marten;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TwentyTwenty.DomainDriven.EventPublishing;

namespace TwentyTwenty.DomainDriven.Marten
{
    public class MartenEventPublishingRepository : EventPublishingRepository<Guid>
    {
        private readonly IDocumentSession _database;

        public MartenEventPublishingRepository(IDocumentSession database, IEventPublisher eventPublisher) : base(eventPublisher)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public override Task<T> GetById<T>(Guid id, CancellationToken token = default) 
        {
            return _database.LoadAsync<T>(id, token);
        }

        public override async Task Save<T>(T aggregate, CancellationToken token = default)
        {
            _database.Store(aggregate);
            await _database.SaveChangesAsync(token);
            await PublishEvents(aggregate, token);
        }

        public override async Task Save<T>(IEnumerable<T> aggregates, CancellationToken token = default)
        {
            _database.Store(aggregates);
            await _database.SaveChangesAsync(token);
            await PublishEvents(aggregates, token);
        }

        public override async Task Delete<T>(T aggregate, CancellationToken token = default)
        {
            _database.Delete(aggregate);
            await _database.SaveChangesAsync(token);
            await PublishEvents(aggregate, token);
        }
    }
}
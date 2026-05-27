using Marten;
using System;
using System.Collections.Generic;
using System.Linq;
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
            // Persist in a deterministic order (ascending Id) so that concurrent
            // transactions updating an overlapping set of documents always acquire
            // their row locks in the same order. Without a consistent order, two
            // batched upserts touching the same rows in different orders can form a
            // lock cycle and trigger a PostgreSQL deadlock (SQLState 40P01).
            var ordered = aggregates.OrderBy(a => a.Id).ToList();
            _database.Store(ordered);
            await _database.SaveChangesAsync(token);
            await PublishEvents(ordered, token);
        }

        public override async Task Delete<T>(T aggregate, CancellationToken token = default)
        {
            _database.Delete(aggregate);
            await _database.SaveChangesAsync(token);
            await PublishEvents(aggregate, token);
        }

        public override void Dispose()
            => _database.Dispose();

        public override ValueTask DisposeAsync()
            => _database.DisposeAsync();
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using TwentyTwenty.DomainDriven.EventPublishing;
using TwentyTwenty.DomainDriven.EventSourcing;

namespace TwentyTwenty.DomainDriven.Marten
{
    public class MartenEventSourcingRepository : EventSourcingRepository<Guid>
    {
        private readonly IDocumentSession _database;

        public MartenEventSourcingRepository(IDocumentSession database, IEventPublisher eventPublisher)
            : base(new MartenEventStore(database), eventPublisher)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public override Task<T> GetById<T>(Guid id, CancellationToken token = default)
            => _database.Events.AggregateStreamAsync<T>(id, token: token);
    }
}
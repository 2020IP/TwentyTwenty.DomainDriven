using System;
using System.Threading.Tasks;
using Marten;
using TwentyTwenty.DomainDriven.EventSourcing;

namespace TwentyTwenty.DomainDriven.Marten
{
    public class MartenRepository : EventSourcingRepository<Guid>
    {
        private readonly IDocumentSession _database;

        public MartenRepository(IDocumentSession database, IEventStore<Guid> eventStore, IEventPublisher eventPublisher)
            : base(eventStore, eventPublisher)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public override Task<T> GetById<T>(Guid id)
            => _database.Events.AggregateStreamAsync<T>(id);
    }
}
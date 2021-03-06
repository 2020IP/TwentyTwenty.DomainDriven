using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using TwentyTwenty.DomainDriven.EventSourcing;

namespace TwentyTwenty.DomainDriven.Marten
{
    public class MartenEventStore : IEventStore<Guid>, IDisposable
    {
        protected readonly IDocumentSession _session;

        public MartenEventStore(IDocumentSession session)
        {
            _session = session;
        }

        public void Dispose()
        {
            _session.Dispose();
        }

        public List<IEventDescriptor> GetEventsForAggregate(Guid aggregateId)
        {
            return _session.Events.FetchStream(aggregateId)
                ?.Select(e => (IEventDescriptor)new MartenEvent(e.Id, e.Version, e.Timestamp.UtcDateTime, e.Data as IDomainEvent))
                .ToList();
        }

        public async Task<List<IEventDescriptor>> GetEventsForAggregateAsync(Guid aggregateId)
        {
            var stream = await _session.Events.FetchStreamAsync(aggregateId).ConfigureAwait(false);
            return stream?.Select(e => (IEventDescriptor)new MartenEvent(e.Id, e.Version, e.Timestamp.UtcDateTime, e.Data as IDomainEvent)).ToList();
        }

        public void SaveEvents(Guid aggregateId, IEnumerable<IDomainEvent> events, int? expectedVersion = default(int?))
        {
            _session.Events.Append(aggregateId, events.ToArray());
            _session.SaveChanges();
        }

        public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<IDomainEvent> events, int? expectedVersion = default(int?))
        {
            _session.Events.Append(aggregateId, events.ToArray());
            await _session.SaveChangesAsync().ConfigureAwait(false);
        }

        public void AppendEvents(Guid aggregateId, params IDomainEvent[] events)
        {
            _session.Events.Append(aggregateId, events.ToArray());
        }

        public Task CommitEventsAsync()
           =>  _session.SaveChangesAsync();

        public void CommitEvents()
            => _session.SaveChanges();
    }
}
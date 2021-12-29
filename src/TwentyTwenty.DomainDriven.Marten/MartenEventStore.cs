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

        public List<IEventDescriptor> GetEventsForStream(Guid streamId)
        {
            return _session.Events.FetchStream(streamId)
                ?.Select(e => (IEventDescriptor)new MartenEvent(e.Id, e.Version, e.Timestamp.UtcDateTime, e.Data as IDomainEvent))
                .ToList();
        }

        public async Task<List<IEventDescriptor>> GetEventsForStreamAsync(Guid streamId)
        {
            var stream = await _session.Events.FetchStreamAsync(streamId);
            return stream?.Select(e => (IEventDescriptor)new MartenEvent(e.Id, e.Version, e.Timestamp.UtcDateTime, e.Data as IDomainEvent)).ToList();
        }

        public void AppendEvents(Guid streamId, IEnumerable<IDomainEvent> events, int? expectedVersion = default)
        {
            if (expectedVersion.HasValue)
            {
                _session.Events.Append(streamId, expectedVersion, events);
            }
            else
            {
                _session.Events.Append(streamId, events);
            }
        }

        public void ArchiveStream(Guid streamId)
        {
            _session.Events.ArchiveStream(streamId);
        }

        public Task CommitEventsAsync()
           =>  _session.SaveChangesAsync();

        public void CommitEvents()
            => _session.SaveChanges();

        public void Dispose()
        {
            _session.Dispose();
        }
    }
}
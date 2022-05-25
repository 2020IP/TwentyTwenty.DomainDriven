using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Marten;
using TwentyTwenty.DomainDriven.EventSourcing;

namespace TwentyTwenty.DomainDriven.Marten
{
    public class MartenEventStore : IEventStore<Guid>, IDisposable
    {
        private readonly IDocumentSession _session;

        public MartenEventStore(IDocumentSession session)
        {
            _session = session;
        }
        
        public async Task<StreamEvents> GetEventsForStream(Guid streamId, CancellationToken token = default)
        {
            var batch = _session.CreateBatchQuery();
            var stream = batch.Events.FetchStream(streamId);
            var state = batch.Events.FetchStreamState(streamId);
            await batch.Execute(token);

            return new StreamEvents
            {
                Events = stream.Result?.Select(e => (IEventDescriptor)new MartenEvent(e.Id, e.Version, e.Timestamp.UtcDateTime, e.Data as IDomainEvent)).ToList(),
                CurrentVersion = state.Result?.Version ?? 0,
                IsArchived = state.Result?.IsArchived ?? false,
            };
        }

        public void AppendEvents(Guid streamId, IEnumerable<IDomainEvent> events, long? expectedVersion = default)
        {
            if (expectedVersion.HasValue)
            {
                _session.Events.Append(streamId, expectedVersion.Value, events);
            }
            else
            {
                _session.Events.Append(streamId, events);
            }
        }

        public Task ArchiveStream(Guid streamId, CancellationToken token = default)
        {
            _session.Events.ArchiveStream(streamId);
            return _session.SaveChangesAsync(token);
        }

        public Task CommitEvents(CancellationToken token = default)
           =>  _session.SaveChangesAsync(token);

        public void Dispose()
        {
            _session.Dispose();
        }
    }
}
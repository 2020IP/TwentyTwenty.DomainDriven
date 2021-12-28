using System;
using System.Threading.Tasks;
using Marten;
using TwentyTwenty.DomainDriven.EventSourcing;

namespace TwentyTwenty.DomainDriven.Marten
{
    public class MartenRepository : MartenEventStore, IEventSourcingRepository<Guid>
    {
        public MartenRepository(IDocumentSession session)
            : base(session) { }

        public T GetById<T>(Guid id)
            where T : class, IEventSourcingAggregateRoot<Guid>, new()
            => _session.Events.AggregateStream<T>(id);

        public Task<T> GetByIdAsync<T>(Guid id)
            where T : class, IEventSourcingAggregateRoot<Guid>, new()
            => _session.Events.AggregateStreamAsync<T>(id);

        public void Save<T>(T aggregate, int? expectedVersion = default)
            where T : class, IEventSourcingAggregateRoot<Guid>, new()
        {
            AppendEvents(aggregate.Id, aggregate.GetUnpublishedEvents(), expectedVersion);
            CommitEvents();
            aggregate.MarkEventsAsPublished();
        }

        public async Task SaveAsync<T>(T aggregate, int? expectedVersion = default)
            where T : class, IEventSourcingAggregateRoot<Guid>, new()
        {
            AppendEvents(aggregate.Id, aggregate.GetUnpublishedEvents(), expectedVersion);
            await CommitEventsAsync();
            aggregate.MarkEventsAsPublished();
        }

        public void Save<T>(params T[] aggregates)
            where T : class, IEventSourcingAggregateRoot<Guid>, new()
        {
            foreach (var aggregate in aggregates)
            {
                AppendEvents(aggregate.Id, aggregate.GetUnpublishedEvents());
            }

            CommitEvents();

            foreach (var aggregate in aggregates)
            {
                aggregate.MarkEventsAsPublished();
            }
        }

        public async Task SaveAsync<T>(params T[] aggregates)
            where T : class, IEventSourcingAggregateRoot<Guid>, new()
        {
            foreach (var aggregate in aggregates)
            {
                AppendEvents(aggregate.Id, aggregate.GetUnpublishedEvents());
            }

            await CommitEventsAsync();

            foreach (var aggregate in aggregates)
            {
                aggregate.MarkEventsAsPublished();
            }
        }

        public void SaveAndArchive<T>(T aggregate, int? expectedVersion = default)
            where T : class, IEventSourcingAggregateRoot<Guid>, new()
        {
            AppendEvents(aggregate.Id, aggregate.GetUnpublishedEvents(), expectedVersion);
            ArchiveAggregate(aggregate.Id);
            CommitEvents();

            aggregate.MarkEventsAsPublished();
        }

        public async Task SaveAndArchiveAsync<T>(T aggregate, int? expectedVersion = default)
            where T : class, IEventSourcingAggregateRoot<Guid>, new()
        {
            AppendEvents(aggregate.Id, aggregate.GetUnpublishedEvents(), expectedVersion);
            ArchiveAggregate(aggregate.Id);

            await CommitEventsAsync();

            aggregate.MarkEventsAsPublished();
        }
    }
}
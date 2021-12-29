using System;
using System.Threading;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.EventPublishing
{
	public interface IEventPublisher
	{
		Task Publish(IDomainEvent @event, Type eventType, CancellationToken token = default);
		Task Publish(IDomainEvent @event, CancellationToken token = default);
    }
}

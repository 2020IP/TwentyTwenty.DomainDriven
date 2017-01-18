using System;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven
{
	public interface IEventPublisher
	{
		Task Publish(IDomainEvent @event, Type eventType);

		Task Publish(IDomainEvent @event);
    }
}

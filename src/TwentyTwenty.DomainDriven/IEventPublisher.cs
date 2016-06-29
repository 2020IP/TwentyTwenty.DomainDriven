using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven
{
	public interface IEventPublisher
	{
	    Task Publish<T>(T @event) where T : class, IDomainEvent;
    }
}

using System;
using System.Threading.Tasks;
using MassTransit;

namespace TwentyTwenty.DomainDriven.MassTransit
{
    public class MassTransitEventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task Publish(IDomainEvent @event, Type eventType)
        {
            return _publishEndpoint.Publish(@event, eventType);
        }

        public Task Publish(IDomainEvent @event)
        {
            return _publishEndpoint.Publish(@event);
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using TwentyTwenty.DomainDriven.EventPublishing;

namespace TwentyTwenty.DomainDriven.MassTransit
{
    public class MassTransitEventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task Publish(IDomainEvent @event, Type eventType, CancellationToken token = default)
        {
            return _publishEndpoint.Publish(@event, eventType, token);
        }

        public Task Publish(IDomainEvent @event, CancellationToken token = default)
        {
            return _publishEndpoint.Publish(@event, token);
        }
    }
}
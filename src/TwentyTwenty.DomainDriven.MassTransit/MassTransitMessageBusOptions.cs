using GreenPipes;
using MassTransit;
using MassTransit.Builders;
using MassTransit.Pipeline;

namespace TwentyTwenty.DomainDriven.MassTransit
{
    public class MassTransitMessageBusOptions
    {
        public bool UseInMemoryBus { get; set; }
        public string RabbitMQUri { get; set; }
    }
}
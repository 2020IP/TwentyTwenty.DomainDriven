using System.Collections.Generic;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IProjection
    {
        void ApplyEvents(IEnumerable<IDomainEvent> events);
    }
}
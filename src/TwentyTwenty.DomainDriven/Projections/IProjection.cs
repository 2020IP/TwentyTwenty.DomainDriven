using System.Collections.Generic;
using TwentyTwenty.DomainDriven.EventSourcing;

namespace TwentyTwenty.DomainDriven.Projections
{
    public interface IProjection
    {
        void ApplyEvents(IEnumerable<IEventDescriptor> events);
    }    
}
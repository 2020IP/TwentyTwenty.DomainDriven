using System.Collections.Generic;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public class StreamEvents
    {
        public List<IEventDescriptor> Events { get; set; }
        public long CurrentVersion { get; set; }
        public bool IsArchived { get; set; }
    }
}
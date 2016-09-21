using System;

namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IProjectionStepCache<T> where T : IProjection
    {
        
        Type ProjectionType { get; }

        Type[] ConsumedEvents { get; }

        IProjectionStep<T, TEventDescriptor> StepFor<TEventDescriptor>();
    }
}
namespace TwentyTwenty.DomainDriven.EventSourcing
{
    public interface IProjectionStep<TProjection, TEvent>
    {
        void Apply(TProjection aggregate, TEvent @event);
    }
}
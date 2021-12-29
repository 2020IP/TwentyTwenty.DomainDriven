namespace TwentyTwenty.DomainDriven.Projections
{
    public interface IProjectionStep<TProjection, TEvent>
    {
        void Apply(TProjection aggregate, TEvent @event);
    }
}
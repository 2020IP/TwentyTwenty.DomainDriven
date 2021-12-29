namespace TwentyTwenty.DomainDriven.Projections
{
    public interface ICanApply 
    {
        void Apply<TProjection>(TProjection state, IProjectionStepCache<TProjection> cache)
            where TProjection : class, IProjection;
    }
}
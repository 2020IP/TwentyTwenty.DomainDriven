namespace TwentyTwenty.DomainDriven
{
    public interface IEventListener<in T> : IHandle<T> where T : IDomainEvent
    {

    }
}
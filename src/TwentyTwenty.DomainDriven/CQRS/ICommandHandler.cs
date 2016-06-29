namespace TwentyTwenty.DomainDriven.CQRS
{
    public interface ICommandHandler<in T> : IHandle<T> where T : ICommand
    {

    }
}
namespace TwentyTwenty.DomainDriven.CQRS
{
    public interface ICommandHandler<in T> : IHandle<T>
        where T : ICommand
    {
    }

    public interface ICommandHandler<in T, TResult> : IHandle<T, TResult>
        where T : ICommand
        where TResult : class, IResponse
    {
    }
}
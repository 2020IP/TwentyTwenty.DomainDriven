using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven
{
    public interface IHandle<in T> 
        where T : IMessage
    {
        Task Handle(T message);
    }

    public interface IHandle<in T, TResult>
        where T : IMessage
        where TResult : class, IResponse
    {
        Task<TResult> Handle(T message);
    }
}
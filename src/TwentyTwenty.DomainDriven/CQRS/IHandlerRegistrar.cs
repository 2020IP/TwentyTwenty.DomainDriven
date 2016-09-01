using System;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.CQRS
{
    public interface IHandlerRegistrar
    {
        void RegisterHandler<T>(Action<T> handler) where T : class, IMessage;
    }

    public interface IHandlerRequestResponseRegistrar
    {
        void RegisterHandler<T, TResult>(Func<T, Task<TResult>> handler)
            where T : class, IMessage
            where TResult : class, IResponse;
    }
}

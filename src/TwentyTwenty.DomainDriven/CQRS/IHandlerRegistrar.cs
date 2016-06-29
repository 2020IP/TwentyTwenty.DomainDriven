using System;

namespace TwentyTwenty.DomainDriven.CQRS
{
    public interface IHandlerRegistrar
    {
        void RegisterHandler<T>(Action<T> handler) where T : class, IMessage;
    }
}

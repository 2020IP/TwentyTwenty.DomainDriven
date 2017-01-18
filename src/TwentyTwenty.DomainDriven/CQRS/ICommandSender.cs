using System;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.CQRS
{
    public interface ICommandSender
    {
        Task Send(ICommand command);

        Task Send(ICommand command, Type commandType);
    }

    public interface ICommandSenderReceiver
    {
        Task<TResult> Send<T, TResult>(T command)
            where T : class, ICommand
            where TResult : class, IResponse;
    }
}
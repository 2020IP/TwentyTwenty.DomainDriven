using System;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.CQRS
{
    public interface ICommandSender
    {
        Task Send<T>(T command) where T : class, ICommand;

        Task Send(ICommand command, Type commandType);
    }

    public interface ICommandSenderReceiver
    {
        Task<TResult> Send<T, TResult>(T command)
            where T : class, ICommand
            where TResult : class, IResponse;
    }
}
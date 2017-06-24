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
        Task<TResult> Send<TResult>(ICommand command) 
            where TResult : class, IResponse;
        Task<TResult> Send<TResult>(ICommand command, Type commandType) 
            where TResult : class, IResponse;
    }
}
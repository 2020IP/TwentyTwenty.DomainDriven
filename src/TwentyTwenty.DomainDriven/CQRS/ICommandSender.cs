using System;
using System.Threading;
using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.CQRS
{
    public interface ICommandSender
    {
        Task Send(ICommand command, CancellationToken token = default);
        Task Send(ICommand command, Type commandType, CancellationToken token = default);
    }

    public interface ICommandSenderReceiver
    {
        Task<TResult> Send<TResult>(ICommand command, CancellationToken token = default) 
            where TResult : class, IResponse;
        Task<TResult> Send<TResult>(ICommand command, Type commandType, CancellationToken token = default) 
            where TResult : class, IResponse;
    }
}
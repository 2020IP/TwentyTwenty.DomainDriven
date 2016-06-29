using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven.CQRS
{
    public interface ICommandSender
    {
        Task Send<T>(T command) where T : class, ICommand;
    }
}
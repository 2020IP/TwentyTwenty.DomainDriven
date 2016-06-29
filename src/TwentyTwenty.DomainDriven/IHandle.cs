using System.Threading.Tasks;

namespace TwentyTwenty.DomainDriven
{
    public interface IHandle<in T> where T : IMessage
    {
        Task Handle(T message);
    }
}
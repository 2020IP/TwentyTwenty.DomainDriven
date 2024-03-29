using MassTransit;
using TwentyTwenty.DomainDriven.CQRS;

namespace TwentyTwenty.DomainDriven.MassTransit
{
    public class DomainDrivenEndpointNameFormatter : IEndpointNameFormatter
    {
        public DomainDrivenEndpointNameFormatter()
        {
        }

        public string Separator => string.Empty;

        public string TemporaryEndpoint(string tag)
        {
            return DefaultEndpointNameFormatter.Instance.TemporaryEndpoint(tag);
        }

        public string Consumer<T>()
            where T : class, IConsumer
        {
            if (typeof(ICommand).IsAssignableFrom(typeof(T).GetMessageType()))
            {
                return typeof(T).GetMessageType().Name;
            }
            else
            {
                return typeof(T).Name;
            }
        }

        public string Saga<T>()
            where T : class, ISaga
        {
            return DefaultEndpointNameFormatter.Instance.Saga<T>();
        }

        public string Message<T>() 
            where T : class
        {
            return DefaultEndpointNameFormatter.Instance.Message<T>();
        }

        public string ExecuteActivity<T, TArguments>()
            where T : class, IExecuteActivity<TArguments>
            where TArguments : class
        {
            return DefaultEndpointNameFormatter.Instance.ExecuteActivity<T, TArguments>();
        }

        public string CompensateActivity<T, TLog>()
            where T : class, ICompensateActivity<TLog>
            where TLog : class
        {
            return DefaultEndpointNameFormatter.Instance.CompensateActivity<T, TLog>();
        }

        public virtual string SanitizeName(string name)
        {
            return name;
        }
    }
}
using System.Threading.Tasks;

namespace AGBrand.Packages.Contracts
{
    public interface IQueueHelper<T>
    {
        Task SendAsync(string queueName, object message);
    }
}

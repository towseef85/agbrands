using System.Threading.Tasks;

namespace AGBrand.Packages.Contracts
{
    public interface IEventHubHelper<T>
    {
        Task SendAsync(string eventHubName, object message);
    }
}

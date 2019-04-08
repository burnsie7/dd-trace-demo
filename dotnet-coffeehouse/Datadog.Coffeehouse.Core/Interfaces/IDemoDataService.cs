using System.Threading.Tasks;

namespace Datadog.Coffeehouse.Core.Interfaces
{
    public interface IDemoDataService
    {
        Task CreateDemoDataBatchAsync(bool forceCreate = false, bool withUsers = false, double rowMultiplier = 1, int maxAttempts = 1);
        Task RunDemoDataBatchAsync();
    }
}

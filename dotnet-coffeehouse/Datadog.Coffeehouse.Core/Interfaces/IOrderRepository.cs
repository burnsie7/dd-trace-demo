using Datadog.Coffeehouse.Core.Models;

namespace Datadog.Coffeehouse.Core.Interfaces
{
    public interface IOrderRepository : IApiRepository<Order> { }
    public interface IOrderItemRepository : IApiRepository<OrderItem> { }
}

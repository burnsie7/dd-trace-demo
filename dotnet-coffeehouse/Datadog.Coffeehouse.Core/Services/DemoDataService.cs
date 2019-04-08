using System;
using System.Threading.Tasks;
using Datadog.Coffeehouse.Core.Enums;
using Datadog.Coffeehouse.Core.Extensions;
using Datadog.Coffeehouse.Core.Interfaces;
using Datadog.Coffeehouse.Core.Models;
using Microsoft.Extensions.Logging;
using ServiceStack;

namespace Datadog.Coffeehouse.Core.Services
{
    public class DemoDataService : IDemoDataService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ILogger<DemoDataService> _log;

        public DemoDataService(IUserRepository userRepository,
                               IOrderRepository orderRepository,
                               IProductRepository productRepository,
                               IOrderItemRepository orderItemRepository,
                               ILogger<DemoDataService> log)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _orderItemRepository = orderItemRepository;
            _log = log;
        }

        public async Task RunDemoDataBatchAsync()
        {
            _log.LogInformation("Starting DemoDataBatch Run");

            await CreateDemoDataBatchAsync(forceCreate: true, rowMultiplier: 0.5);

            // Run some selects
            var users = _userRepository.Select(5);
            var products = _productRepository.Select(20);
            var orders = _orderRepository.Select(20);

            // Update, delete, select some stuff
            users.Each(u =>
                       {
                           u.Name = u.Name + " - " + DateTime.UtcNow.ToSqlString();
                           _userRepository.Update(u);
                           _userRepository.Single(u.Id);
                       });

            products.Each((i, p) =>
                          {
                              if (i % 5 == 0)
                              {
                                  _productRepository.Delete(p.Id);
                              }
                              else if (i % 3 == 0)
                              {
                                  p.Name = p.Name + " - " + DateTime.UtcNow.ToSqlString();
                                  p.UnitCost = p.UnitCost + 0.001;
                                  _productRepository.Update(p);
                              }
                              else
                              {
                                  _productRepository.Single(p.Id);
                              }
                          });

            orders.Each((i, o) =>
                        {
                            if (i % 5 == 0)
                            {
                                _orderRepository.Delete(o.Id);
                            }
                            else if (i % 3 == 0)
                            {
                                o.Status = OrderStatus.Complete;
                                _orderRepository.Update(o);
                            }
                            else
                            {
                                _orderRepository.Single(o.Id);
                            }
                        });

            _log.LogInformation("Finished DemoDataBatch Run");
        }

        public async Task CreateDemoDataBatchAsync(bool forceCreate = false, bool withUsers = false, double rowMultiplier = 1, int maxAttempts = 1)
        {
            rowMultiplier = rowMultiplier.GreaterThanZero(1);

            var attempts = 0;

            do
            {
                attempts++;

                try
                {
                    var companyId = Guid.NewGuid().ToStringId();

                    _log.LogInformation($"Seeding data for company [{companyId}]");

                    var usersSeeded = SeedRepo(_userRepository,
                                               i => new User
                                                    {
                                                        Id = Guid.NewGuid().ToStringId(),
                                                        CompanyId = companyId,
                                                        Name = $"Api User {i}",
                                                        Email = $"user{i}@datadogdotnetdemo.com",
                                                        CreatedOn = DateTime.UtcNow.ToUnixTimestamp()
                                                    },
                                               5 * rowMultiplier,
                                               forceCreate && withUsers);

                    var users = _userRepository.Select(50);

                    var ordersSeeded = SeedRepo(_orderRepository,
                                                i => new Order
                                                     {
                                                         Id = Guid.NewGuid().ToStringId(),
                                                         UserId = users[RandomProvider.Instance.GetRandomIntBeween(0, users.Count - 1)].Id,
                                                         Status = (OrderStatus)(i % 3),
                                                         CreatedOn = DateTime.UtcNow.ToUnixTimestamp()
                                                     },
                                                10 * rowMultiplier,
                                                forceCreate);

                    var productsSeeded = SeedRepo(_productRepository,
                                                  i =>
                                                  {
                                                      var prodType = (ProductType)(i % 3);

                                                      return new Product
                                                             {
                                                                 Id = Guid.NewGuid().ToStringId(),
                                                                 ProductType = prodType,
                                                                 Name = $"{prodType.ToString()} Product {i}",
                                                                 UnitCost = Math.Round((i / 8.5), 4)
                                                             };
                                                  },
                                                  4 * rowMultiplier,
                                                  forceCreate);

                    var orders = _orderRepository.Select(25);
                    var products = _productRepository.Select(25);

                    var orderItemsSeeded = SeedRepo(_orderItemRepository,
                                                    i =>
                                                    {
                                                        var product = products[RandomProvider.Instance.GetRandomIntBeween(0, products.Count - 1)];

                                                        return new OrderItem
                                                               {
                                                                   Id = Guid.NewGuid().ToStringId(),
                                                                   OrderId = orders[RandomProvider.Instance.GetRandomIntBeween(0, orders.Count - 1)].Id,
                                                                   ProductId = product.Id,
                                                                   Quantity = i % 7,
                                                                   UnitCost = Math.Round(product.UnitCost + Math.Round((i / 102.3), 4), 4)
                                                               };
                                                    },
                                                    15 * rowMultiplier,
                                                    forceCreate);

                    _log.LogInformation($"Finished creating data for company [{companyId}] - users:[{usersSeeded}] | orders:[{ordersSeeded}] | products:[{productsSeeded}] | items:[{orderItemsSeeded}]");

                    return;
                }
                catch(Exception x)
                {
                    if (attempts > maxAttempts)
                    {
                        break;
                    }

                    var waitFor = 3 * 1000 * attempts;

                    _log.LogError(x, $"Error trying to seed data for company on attempt [{attempts}], waiting [{waitFor}]ms before trying again...");

                    await Task.Delay(waitFor);
                }
            } while (attempts <= maxAttempts);

            _log.LogError("Gave up trying to seed data for company, tried too many times");
        }

        private static int SeedRepo<T>(IApiRepository<T> repo, Func<int, T> builder,
                                       double rows = 5, bool forceCreate = false)
        {
            var rowsToSeed = (int)rows.GreaterThanZero(5);

            repo.InitSchema();

            var shouldAdd = forceCreate || repo.Select(1).IsNullOrEmpty();

            if (!shouldAdd)
            {
                return 0;
            }

            rowsToSeed.Times(i => repo.Add(builder(i)));

            return rowsToSeed;
        }
    }
}

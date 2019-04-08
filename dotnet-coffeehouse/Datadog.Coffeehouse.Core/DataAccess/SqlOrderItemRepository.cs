using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Datadog.Coffeehouse.Core.Extensions;
using Datadog.Coffeehouse.Core.Interfaces;
using Datadog.Coffeehouse.Core.Models;
using ServiceStack.Redis;

namespace Datadog.Coffeehouse.Core.DataAccess
{
    public class CachedSqlOrderItemRepository : CachedRedisApiRepository<OrderItem>, IOrderItemRepository
    {
        public CachedSqlOrderItemRepository(IRedisClientsManager redisClientManager,
                                            IOrderItemRepository innerRepository)
            : base(redisClientManager, innerRepository) { }
    }

    public class SqlOrderItemRepository : BaseApiRepository, IOrderItemRepository
    {
        public SqlOrderItemRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        public OrderItem Single(string id)
            => DbExec(db => db.QuerySingleOrDefault<OrderItem>("SELECT oi.* FROM OrderItems oi WHERE oi.Id = @Id;",
                                                               new
                                                               {
                                                                   Id = id
                                                               }));

        public List<OrderItem> Select(int limit = 0)
            => DbExec(db => db.Query<OrderItem>("SELECT oi.* FROM OrderItems oi;", buffered: false)
                                   .Take(limit.GreaterThanZero(defaultValue: 1000))
                                   .AsList());

        public string Add(OrderItem orderItem)
        {
            if (!orderItem.Id.IsNotNullOrEmpty())
            {
                orderItem.Id = Guid.NewGuid().ToStringId();
            }

            DbExec(db => db.Execute(@"
INSERT    INTO OrderItems
          (Id, OrderId, ProductId, Quantity, UnitCost)
VALUES    (@Id, @OrderId, @ProductId, @Quantity, @UnitCost);
",
                                    new
                                    {
                                        orderItem.Id,
                                        orderItem.OrderId,
                                        orderItem.ProductId,
                                        orderItem.Quantity,
                                        orderItem.UnitCost
                                    }));

            return orderItem.Id;
        }

        public void Update(OrderItem orderItem)
            => DbExec(db => db.Execute(@"
UPDATE    OrderItems
SET       Quantity = @Quantity,
          UnitCost = @UnitCost
WHERE     Id = @Id
          AND OrderId = @OrderId;
",
                                       new
                                       {
                                           orderItem.Id,
                                           orderItem.OrderId,
                                           orderItem.Quantity,
                                           orderItem.UnitCost
                                       }));

        public void Delete(string id)
            => DbExec(db => db.Execute(@"
DELETE    FROM OrderItems
WHERE     Id = @Id;
",
                                       new
                                       {
                                           Id = id
                                       }));

        public void InitSchema()
            => DbExec(db => db.Execute(@"
IF OBJECT_ID('OrderItems') IS NULL
BEGIN;
    CREATE TABLE OrderItems
    (
        Id          VARCHAR(32) NOT NULL PRIMARY KEY,
        OrderId     VARCHAR(32) NOT NULL,
        ProductId   VARCHAR(32) NOT NULL,
        Quantity    INT NOT NULL,
        UnitCost    NUMERIC(18,4) NOT NULL
    )
    ON [DEFAULT];
END;

IF NOT EXISTS(SELECT NULL FROM sys.indexes WHERE NAME = 'UXN_OrderItems_OrderId_Id')
BEGIN;
    CREATE UNIQUE NONCLUSTERED INDEX UXN_OrderItems_OrderId_Id ON OrderItems (OrderId, Id) ON [DEFAULT];
END;
"));

    }
}

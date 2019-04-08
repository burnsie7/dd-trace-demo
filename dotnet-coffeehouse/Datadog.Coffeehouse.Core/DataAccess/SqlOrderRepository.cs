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
    public class CachedSqlOrderRepository : CachedRedisApiRepository<Order>, IOrderRepository
    {
        public CachedSqlOrderRepository(IRedisClientsManager redisClientManager,
                                        IOrderRepository innerRepository)
            : base(redisClientManager, innerRepository) { }
    }

    public class SqlOrderRepository : BaseApiRepository, IOrderRepository
    {
        public SqlOrderRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        public Order Single(string id)
            => DbExec(db => db.QuerySingleOrDefault<Order>("SELECT o.* FROM Orders o WHERE o.Id = @Id;",
                                                           new
                                                           {
                                                               Id = id
                                                           }));

        public List<Order> Select(int limit = 0)
            => DbExec(db => db.Query<Order>("SELECT o.* FROM Orders o;", buffered: false)
                              .Take(limit.GreaterThanZero(defaultValue: 1000))
                              .AsList());

        public string Add(Order order)
        {
            if (!order.Id.IsNotNullOrEmpty())
            {
                order.Id = Guid.NewGuid().ToStringId();
            }

            DbExec(db => db.Execute(@"
INSERT    INTO Orders
          (Id, UserId, Status, CreatedOn)
VALUES    (@Id, @UserId, @Status, @CreatedOn);
",
                                    new
                                    {
                                        order.Id,
                                        order.UserId,
                                        order.Status,
                                        CreatedOn = DateTime.UtcNow.ToUnixTimestamp()
                                    }));

            return order.Id;
        }

        public void Update(Order order)
            => DbExec(db => db.Execute(@"
UPDATE    Orders
SET       Status = @Status
WHERE     Id = @Id;
",
                                       new
                                       {
                                           order.Id,
                                           order.Status
                                       }));

        public void Delete(string id)
            => DbExec(db => db.Execute(@"
DELETE    FROM Orders
WHERE     Id = @Id;
",
                                       new
                                       {
                                           Id = id
                                       }));

        public void InitSchema()
            => DbExec(db => db.Execute(@"
IF OBJECT_ID('Orders') IS NULL
BEGIN;
    CREATE TABLE Orders
    (
        Id          VARCHAR(32) NOT NULL PRIMARY KEY,
        UserId      VARCHAR(32) NOT NULL,
        Status      VARCHAR(50) NOT NULL,
        CreatedOn   BIGINT NOT NULL
    )
    ON [DEFAULT];
END;

IF NOT EXISTS(SELECT NULL FROM sys.indexes WHERE NAME = 'UXN_Orders_UserId_Id')
BEGIN;
    CREATE UNIQUE NONCLUSTERED INDEX UXN_Orders_UserId_Id ON Orders (UserId, Id) ON [DEFAULT];
END;
"));

    }
}

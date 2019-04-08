using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using Datadog.Coffeehouse.Core.Enums;
using Datadog.Coffeehouse.Core.Extensions;
using Datadog.Coffeehouse.Core.Interfaces;
using Datadog.Coffeehouse.Core.Models;
using OpenTracing;
using ServiceStack.Redis;

namespace Datadog.Coffeehouse.Core.DataAccess
{
    public class CachedMySqlProductRepository : CachedRedisApiRepository<Product>, IProductRepository
    {
        public CachedMySqlProductRepository(IRedisClientsManager redisClientManager,
                                            IProductRepository innerRepository)
            : base(redisClientManager, innerRepository) { }
    }

    public class OpenTracedMySqlProductRepository : OpenTracedApiRepository<Product>, IProductRepository
    {
        public OpenTracedMySqlProductRepository(ITracer tracer, IProductRepository innerRepository)
            : base(tracer, innerRepository) { }
    }

    public class MySqlProductRepository : BaseRepository, IProductRepository
    {
        public MySqlProductRepository(IDbFactory dbFactory) : base(dbFactory) { }

        protected override AppDatabase AppDbType => AppDatabase.ProductMySql;

        public Product Single(string id)
            => DbExec(db => db.QuerySingleOrDefault<Product>("SELECT p.* FROM Products p WHERE p.Id = @Id;",
                                                             new
                                                             {
                                                                 Id = id
                                                             }));

        public List<Product> Select(int limit = 0)
            => DbExec(db => db.Query<Product>("SELECT p.* FROM Products p;", buffered: false)
                              .Take(limit.GreaterThanZero(defaultValue: 1000))
                              .AsList());

        public string Add(Product product)
        {
            if (!product.Id.IsNotNullOrEmpty())
            {
                product.Id = Guid.NewGuid().ToStringId();
            }

            DbExec(db => db.Execute(@"
INSERT    INTO Products
          (Id, ProductType, Name, UnitCost)
VALUES    (@Id, @ProductType, @Name, @UnitCost);
",
                                    new
                                    {
                                        product.Id,
                                        product.ProductType,
                                        product.Name,
                                        product.UnitCost
                                    }));

            return product.Id;
        }

        public void Update(Product product)
            => DbExec(db => db.Execute(@"
UPDATE    Products
SET       ProductType = @ProductType,
          Name = @Name,
          UnitCost = @UnitCost
WHERE     Id = @Id;
",
                                       new
                                       {
                                           product.Id,
                                           product.ProductType,
                                           product.Name,
                                           product.UnitCost
                                       }));

        public void Delete(string id)
            => DbExec(db => db.Execute(@"
DELETE    FROM Products
WHERE     Id = @Id;
",
                                       new
                                       {
                                           Id = id
                                       }));
    
        public void InitSchema()
            => DbExec(db => db.Execute(@"
CREATE TABLE IF NOT EXISTS Products
(
    Id          VARCHAR(32) NOT NULL,
    ProductType VARCHAR(50) NOT NULL,
    Name        VARCHAR(100) NOT NULL,
    UnitCost    NUMERIC(18,5) NOT NULL,
    PRIMARY KEY (Id),
    UNIQUE (ProductType, Id)
);"));

    }
}

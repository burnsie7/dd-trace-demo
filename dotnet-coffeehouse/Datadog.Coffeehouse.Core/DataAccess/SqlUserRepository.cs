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
    public class CachedSqlUserRepository : CachedRedisApiRepository<User>, IUserRepository
    {
        public CachedSqlUserRepository(IRedisClientsManager redisClientManager,
                                       IUserRepository innerRepository)
            : base(redisClientManager, innerRepository) { }
    }

    public class ManualTracedSqlUserRepository : DatadogManualTracedApiRepository<User>, IUserRepository
    {
        public ManualTracedSqlUserRepository(IUserRepository innerRepository)
            : base(innerRepository) { }
    }

    public class SqlUserRepository : BaseApiRepository, IUserRepository
    {
        public SqlUserRepository(IDbFactory dbFactory)
            : base(dbFactory) { }

        public User Single(string id)
            => DbExec(db => db.QuerySingleOrDefault<User>("SELECT u.* FROM Users u WHERE u.Id = @Id;",
                                                          new
                                                          {
                                                              Id = id
                                                          }));

        public List<User> Select(int limit = 0)
            => DbExec(db => db.Query<User>("SELECT u.* FROM Users u;", buffered: false)
                              .Take(limit.GreaterThanZero(defaultValue: 1000))
                              .AsList());

        public string Add(User user)
        {
            if (!user.Id.IsNotNullOrEmpty())
            {
                user.Id = Guid.NewGuid().ToStringId();
            }

            DbExec(db => db.Execute(@"
INSERT    INTO Users
          (Id, CompanyId, Name, Email, CreatedOn)
VALUES    (@Id, @CompanyId, @Name, @Email, @CreatedOn);
",
                                    new
                                    {
                                        user.Id,
                                        user.CompanyId,
                                        user.Name,
                                        user.Email,
                                        CreatedOn = DateTime.UtcNow.ToUnixTimestamp()
                                    }));

            return user.Id;
        }

        public void Update(User user)
            => DbExec(db => db.Execute(@"
UPDATE    Users
SET       Name = @Name,
          Email = @Email
WHERE     Id = @Id;
",
                                       new
                                       {
                                           user.Id,
                                           user.Name,
                                           user.Email
                                       }));

        public void Delete(string id)
            => DbExec(db => db.Execute(@"
DELETE    FROM Users
WHERE     Id = @Id;
",
                                       new
                                       {
                                           Id = id
                                       }));

        public void InitSchema()
            => DbExec(db => db.Execute(@"
IF OBJECT_ID('Users') IS NULL
BEGIN;
    CREATE TABLE Users
    (
        Id          VARCHAR(32) NOT NULL PRIMARY KEY,
        CompanyId   VARCHAR(32) NOT NULL,
        Name        VARCHAR(100) NOT NULL,
        Email       VARCHAR(100) NOT NULL,
        CreatedOn   BIGINT NOT NULL
    )
    ON [DEFAULT];
END;

IF NOT EXISTS(SELECT NULL FROM sys.indexes WHERE NAME = 'UXN_Users_CompanyId_Id')
BEGIN;
    CREATE UNIQUE NONCLUSTERED INDEX UXN_Users_CompanyId_Id ON Users (CompanyId, Id) ON [DEFAULT];
END;
"));

    }
}

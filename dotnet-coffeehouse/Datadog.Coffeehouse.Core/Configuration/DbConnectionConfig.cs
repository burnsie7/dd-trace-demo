namespace Datadog.Coffeehouse.Core.Configuration
{
    public class DbConnectionConfig
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public bool TrustedConnection { get; set; }
        public int ConnectTimeout { get; set; }
        public int ConnectionLifetime { get; set; }
    }
}

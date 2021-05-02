using System.Text.RegularExpressions;
using Ianitor.Osp.Backend.Persistence.MongoDb;

namespace Ianitor.Osp.Backend.Persistence
{
    public class OspSystemConfiguration
    {
        private string _systemDatabaseName;

        public OspSystemConfiguration(string databaseHost)
            : this()
        {
            DatabaseHost = databaseHost;
        }

        public OspSystemConfiguration()
        {
            DatabaseHost = "localhost:27017";
            SystemDatabaseName = "OspSystem";
            DatabaseUser = "osp-system-ds-user-{0}";
            AdminUser = "osp-system-admin";
            AuthenticationDatabaseName = "admin";
        }
        
        public string DatabaseHost { get; set; }
        
        public string SystemDatabaseName
        {
            get => _systemDatabaseName;
            set
            {
                if (value == null || !Regex.IsMatch(value, Constants.RegexWithoutWhitespaces))
                {
                    throw new ConfigurationErrorException(
                        "Impossible to apply MongoDB name setting: value is absent, empty or contains whitespaces."
                    );
                }

                _systemDatabaseName = value;
            }
        }

        public string DatabaseUser { get; set; }
        public string DatabaseUserPassword { get; set; }
        public string AdminUser { get; set; }
        public string AdminUserPassword { get; set; }
        
        public string AuthenticationDatabaseName { get; set; }
    }
}
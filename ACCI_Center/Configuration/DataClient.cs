using System.Data.Common;
using ACCI_Center.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace ACCI_Center.Configuraion
{
    public class DataClient : IDataClient
    {
        private DatabaseSettings databaseSettings;
        public DataClient(IOptions<DatabaseSettings> options)
        {
            this.databaseSettings = options.Value;
        }
        public DbConnection GetDbConnection()
        {
            try
            {
                string databaseConnectionString = $"""
                Server = {databaseSettings.DataSourceUrl};
                Database = {databaseSettings.DatabaseName};
                User ID = sa;
                Password = SqlServer@123;
                TrustServerCertificate = True;
                """;

                var sqlConnection = new SqlConnection(databaseConnectionString);

                return sqlConnection;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while creating SQL connection: " + ex.Message);
            }
        }
    }
}

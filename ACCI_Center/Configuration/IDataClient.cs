using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace ACCI_Center.Configuraion
{
    public interface IDataClient
    {
        public DbConnection GetDbConnection();
    }
}

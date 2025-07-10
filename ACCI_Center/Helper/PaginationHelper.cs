using ACCI_Center.Dto;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;

namespace ACCI_Center.Helper
{
    public static class PaginationHelper
    {
        public static async Task<PagedResult<T>> ExecutePagedAsync<T>(
            DbConnection dbConnection,
            string baseSql,
            string orderByClause,
            Func<DbDataReader, T> mapFunc,
            int pageNumber,
            int pageSize,
            DbParameter[] dbParameters)
        {
            try
            {
                if (pageNumber <= 0) pageNumber = 1;
                if (pageSize <= 0) pageSize = 10;

                // 1. Tính totalItems bằng cách bọc baseSql vào COUNT(*)
                var countSql = $"SELECT COUNT(1) FROM ( {baseSql} ) AS CountTable";

                int totalItems;
                using (var cmdCount = dbConnection.CreateCommand())
                {
                    cmdCount.CommandText = countSql;
                    if (dbParameters != null && dbParameters.Length > 0)
                        cmdCount.Parameters.AddRange(dbParameters);

                    if(dbConnection.State != ConnectionState.Open)
                         dbConnection.Open();

                    var result = await cmdCount.ExecuteScalarAsync();
                    totalItems = (result == null || result is DBNull)
                        ? 0
                        : Convert.ToInt32(result);
                }

                // 2. Tạo truy vấn dữ liệu với OFFSET/FETCH
                int offset = (pageNumber - 1) * pageSize;
                var dataSql = $@"
                            {baseSql}
                            {orderByClause}
                            OFFSET @Offset ROWS 
                            FETCH NEXT @PageSize ROWS ONLY";

                var parametersWithPaging = new List<DbParameter>();
                if (dbParameters != null && dbParameters.Length > 0)
                {
                    for (var i = 0; i < dbParameters.Length; i++)
                    {
                        var param = dbConnection.CreateCommand().CreateParameter();
                        param.ParameterName = dbParameters[i].ParameterName;
                        param.DbType = dbParameters[i].DbType;
                        param.Value = dbParameters[i].Value ?? DBNull.Value; // Handle null values
                        parametersWithPaging.Add(param);
                    }
                }

                var offsetParam = dbConnection.CreateCommand().CreateParameter();
                offsetParam.ParameterName = "@Offset";
                offsetParam.DbType = DbType.Int32;
                offsetParam.Value = offset;
                parametersWithPaging.Add(offsetParam);

                var pageSizeParam = dbConnection.CreateCommand().CreateParameter();
                pageSizeParam.ParameterName = "@PageSize";
                pageSizeParam.DbType = DbType.Int32;
                pageSizeParam.Value = pageSize;
                parametersWithPaging.Add(pageSizeParam);

                var items = new List<T>();
                using (var cmdData = dbConnection.CreateCommand())
                {
                    cmdData.CommandText = dataSql;
                    cmdData.Parameters.AddRange(parametersWithPaging.ToArray());

                    if(dbConnection.State != ConnectionState.Open)
                        dbConnection.Open();

                    using (var reader = await cmdData.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            items.Add(mapFunc(reader));
                        }
                    }
                }

                return new PagedResult<T>(items, totalItems, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                throw new Exception("Error executing paged query: " + ex.Message, ex);
            }
            finally
            {
                if (dbConnection.State == ConnectionState.Open)
                    dbConnection.Close();
            }
        }

    }
}

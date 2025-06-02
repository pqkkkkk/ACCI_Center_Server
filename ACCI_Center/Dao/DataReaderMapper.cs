using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace ACCI_Center.Dao
{
    public static class DataReaderMapper
    {
        public static T MapToObject<T>(DbDataReader reader) where T : new()
        {
            var obj = new T();
            var props = typeof(T).GetProperties();

            foreach (var prop in props)
            {
                if (!reader.HasColumn(prop.Name) || reader[prop.Name] == DBNull.Value)
                    continue;

                prop.SetValue(obj, Convert.ChangeType(reader[prop.Name], prop.PropertyType));
            }

            return obj;
        }

        public static bool HasColumn(this DbDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}

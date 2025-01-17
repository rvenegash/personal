using System;
using System.Data;

namespace apialmuerzos
{
    public static class Rutinas
    {
        public static string stringNul(IDataReader reader, string column)
        {
            var ord = reader.GetOrdinal(column);
            return reader.IsDBNull(ord) ? "" : reader.GetString(ord);
        }
        public static decimal decimalNul(IDataReader reader, string column)
        {
            var ord = reader.GetOrdinal(column);
            return reader.IsDBNull(ord) ? 0 : reader.GetDecimal(ord);
        }
        public static long longNul(IDataReader reader, string column)
        {
            var ord = reader.GetOrdinal(column);
            return reader.IsDBNull(ord) ? 0 : reader.GetInt64(ord);
        }
        public static int intNul(IDataReader reader, string column)
        {
            var ord = reader.GetOrdinal(column);
            return reader.IsDBNull(ord) ? 0 : reader.GetInt32(ord);
        }
        public static DateTime datetimeNul(IDataReader reader, string column)
        {
            var ord = reader.GetOrdinal(column);
            return reader.IsDBNull(ord) ? new DateTime(2000, 1, 1) : reader.GetDateTime(ord);
        }
    }
}

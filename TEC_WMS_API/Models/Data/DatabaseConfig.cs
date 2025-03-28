using System.Data;
using System.Data.SqlClient;
using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Data
{
    public class DatabaseConfig
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DatabaseConfig(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public int InsertExceptionLogSP(ExceptionLog logEntry)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertExceptionLog", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters
                    command.Parameters.AddWithValue("@LogLevel", (object)logEntry.LogLevel ?? DBNull.Value);
                    command.Parameters.AddWithValue("@MethodName", (object)logEntry.MethodName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ModuleID", (object)logEntry.ModuleID ?? DBNull.Value);
                    command.Parameters.AddWithValue("@ExceptionMessage", (object)logEntry.ExceptionMessage ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Parameters", (object)logEntry.Parameters ?? DBNull.Value);
                    command.Parameters.AddWithValue("@EventTimestamp", (object)logEntry.EventTimestamp ?? DBNull.Value);
                    command.Parameters.AddWithValue("@StackTrace", (object)logEntry.StackTrace ?? DBNull.Value);

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }
    }
}

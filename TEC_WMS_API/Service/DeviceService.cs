using System.Data.SqlClient;
using TEC_WMS_API.Data;
using TEC_WMS_API.Interface;
using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Service
{
    public class DeviceService : IDevice
    {
        private readonly DatabaseConfig _databaseConfig;

        public DeviceService(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }
        public async Task<int> CreateDeviceAsync(UpdateDeviceRequest device)
        {
            using (var conn = _databaseConfig.GetConnection())
            {
                await conn.OpenAsync();

                string query = @"
            INSERT INTO ODVS 
            (UserName, DeviceSerialNo, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn)
            OUTPUT INSERTED.DeviceId
            VALUES 
            (@UserName, @DeviceSerialNo, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn);";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserName", string.IsNullOrEmpty(device.UserName) ? (object)DBNull.Value : device.UserName);
                    cmd.Parameters.AddWithValue("@DeviceSerialNo", string.IsNullOrEmpty(device.DeviceSerialNo) ? (object)DBNull.Value : device.DeviceSerialNo);
                    cmd.Parameters.AddWithValue("@CreatedBy", string.IsNullOrEmpty(device.CreatedBy) ? (object)DBNull.Value : device.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedOn", device.CreatedOn);

                    cmd.Parameters.AddWithValue("@UpdatedBy", string.IsNullOrEmpty(device.UpdatedBy) ? (object)DBNull.Value : device.UpdatedBy);
                    cmd.Parameters.AddWithValue("@UpdatedOn", device.UpdatedOn.HasValue ? device.UpdatedOn.Value : (object)DBNull.Value);

                    var result = await cmd.ExecuteScalarAsync();

                    return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }
            }
        }

        public async Task<DeviceRequest?> GetDeviceByIdAsync(int id)
        {
            DeviceRequest? deviceRequest = null;
            using (var conn = _databaseConfig.GetConnection())
            {
                string Squery = "SELECT * FROM ODVS WHERE DeviceId = @id";
                var cmd = new SqlCommand(Squery, conn);
                cmd.Parameters.AddWithValue("@id", id);
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        deviceRequest = new DeviceRequest
                        {
                            DeviceId = reader.IsDBNull(reader.GetOrdinal("DeviceId")) ? null : reader.GetInt32(reader.GetOrdinal("DeviceId")),
                            UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? null : reader.GetString(reader.GetOrdinal("UserName")),
                            DeviceSerialNo = reader.IsDBNull(reader.GetOrdinal("DeviceSerialNo")) ? null : reader.GetString(reader.GetOrdinal("DeviceSerialNo")),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                            CreatedOn = reader.IsDBNull(reader.GetOrdinal("CreatedOn")) ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("CreatedOn"))
                        };
                    }
                }
            }
            return deviceRequest;
        }

        public async Task<bool> UpdateDeviceAsync(UpdateDeviceRequest device)
        {
            using (var conn = _databaseConfig.GetConnection())
            {
                string Squery = @"
            UPDATE ODVS 
            SET 
                UserName = @UserName,
                DeviceSerialNo = @DeviceSerialNo,
                CreatedBy = @CreatedBy,
                CreatedOn = @CreatedOn,
                UpdatedBy = @UpdatedBy,
                UpdatedOn = @UpdatedOn
            WHERE DeviceId = @DeviceId;";

                var cmd = new SqlCommand(Squery, conn);
                device.UpdatedOn = DateTime.Now; // Set current date for UpdatedOn

                cmd.Parameters.AddWithValue("@UserName", device.UserName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DeviceSerialNo", device.DeviceSerialNo ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", DBNull.Value);
                cmd.Parameters.AddWithValue("@CreatedOn", DBNull.Value);
                cmd.Parameters.AddWithValue("@UpdatedBy", device.UpdatedBy ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UpdatedOn", device.UpdatedOn);
                cmd.Parameters.AddWithValue("@DeviceId", device.DeviceId ?? (object)DBNull.Value);

                await conn.OpenAsync();
                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                conn.Close();

                return rowsAffected > 0;
            }
        }

        public async Task<List<DeviceRequest>> GetAllDeviceAsync()
        {
            var deviceRequests = new List<DeviceRequest>();
            using (var conn = _databaseConfig.GetConnection())
            {
                string Squery = "SELECT * FROM ODVS";
                var cmd = new SqlCommand(Squery, conn);

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    for (; await reader.ReadAsync();) // 🔹 `for` loop with no condition — acts like `while`
                    {
                        deviceRequests.Add(new DeviceRequest
                        {
                            DeviceId = reader.IsDBNull(reader.GetOrdinal("DeviceId"))
                                ? null : reader.GetInt32(reader.GetOrdinal("DeviceId")),
                            UserName = reader.IsDBNull(reader.GetOrdinal("UserName"))
                                ? null : reader.GetString(reader.GetOrdinal("UserName")),
                            DeviceSerialNo = reader.IsDBNull(reader.GetOrdinal("DeviceSerialNo"))
                                ? null : reader.GetString(reader.GetOrdinal("DeviceSerialNo")),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy"))
                                ? null : reader.GetString(reader.GetOrdinal("CreatedBy")),
                            CreatedOn = reader.IsDBNull(reader.GetOrdinal("CreatedOn"))
                                ? DateTime.Now : reader.GetDateTime(reader.GetOrdinal("CreatedOn"))
                        });
                    }
                }
            }

            return deviceRequests;
        }



    }
}

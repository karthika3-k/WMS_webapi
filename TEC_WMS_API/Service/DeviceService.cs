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
        public async Task<int> CreateDeviceAsync(DeviceRequest device)
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






    }
}

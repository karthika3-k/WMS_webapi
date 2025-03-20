using System.Data.SqlClient;
using TEC_WMS_API.Data;
using TEC_WMS_API.Interface;
using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Service
{
    public class WareHouseService : IWareHouse
    {
        private readonly DatabaseConfig _databaseConfig;
        public WareHouseService(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }
        string Squery = string.Empty;
        public async Task<IEnumerable<WareHouseRequest>> GetAllWareHouseAsync()
        {
            var wareHouse = new List<WareHouseRequest>();
            using (var conn = _databaseConfig.GetConnection())
            {
                Squery = "SELECT * FROM OWHS";
                var cmd = new SqlCommand(Squery, conn);
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    for (int i = 0; await reader.ReadAsync(); i++)
                    {
                        var warehouserequest = new WareHouseRequest
                        { 
                            WhsCode = reader.GetString(0), 
                            WhsName = reader.GetString(1) 
                        };
                        wareHouse.Add(warehouserequest);

                    };
                    
                }
                return wareHouse;
            }
        }
    }
}

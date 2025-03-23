using System.Data;
using System.Data.SqlClient;
using TEC_WMS_API.Data;
using TEC_WMS_API.Interface;
using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Service
{
    public class BinConfigService : IBinConfig
    {
        private readonly DatabaseConfig _databaseConfig;
        public BinConfigService(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }
        string Squery = string.Empty;
        public async Task<int> CreateBinConfigsAsync(IEnumerable<BinConfigRequest> binConfigs)
        {
            using (var conn = _databaseConfig.GetConnection())
            {
                try
                {
                    await conn.OpenAsync();
                   
                    int rowsInserted = 0;
                   
                    foreach (var binConfig in binConfigs)
                    {
                        string query = "INSERT INTO OBNC (BinCode, BinName, Prefix, WhsCode,IsActive, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn) " +
                                       "VALUES (@BinCode, @BinName, @Prefix, @WhsCode, @IsActive,@CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn);";

                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@BinCode", binConfig.BinCode);
                            cmd.Parameters.AddWithValue("@BinName", binConfig.BinName);
                            cmd.Parameters.AddWithValue("@Prefix", binConfig.Prefix);
                            cmd.Parameters.AddWithValue("@WhsCode", binConfig.WhsCode);
                            cmd.Parameters.AddWithValue("@IsActive", binConfig.IsActive);
                            cmd.Parameters.AddWithValue("@CreatedBy", binConfig.CreatedBy);
                            cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);  
                            cmd.Parameters.AddWithValue("@UpdatedBy", DBNull.Value);  
                            cmd.Parameters.AddWithValue("@UpdatedOn", DBNull.Value); 
                            
                            rowsInserted += await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    return rowsInserted;
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while creating the BinConfigs.", ex);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public async Task<bool> DeleteBinConfigAsync(int id)
        {
            using (var conn = _databaseConfig.GetConnection())
            {
                Squery = "DELETE FROM OBNC WHERE ID = @id;";
                var cmd = new SqlCommand(Squery, conn);
                cmd.Parameters.AddWithValue("@id", id);
                await conn.OpenAsync();
                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        public async Task<IEnumerable<BinConfigRequest>> GetAllBinConfigAsync()
        {
            var binconfig = new List<BinConfigRequest>();
            using (var conn = _databaseConfig.GetConnection())
            {
                Squery = "SELECT * FROM OBNC";
                var cmd = new SqlCommand(Squery, conn);
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    for (int i = 0; await reader.ReadAsync(); i++)
                    {
                        var bincongrequest = new BinConfigRequest
                        {
                            BinConfigId = reader.GetInt32(0),
                            BinCode = reader.GetString(1),
                            BinName = reader.GetString(2),
                            Prefix = reader.GetString(3),
                            WhsCode = reader.GetString(4),
                            IsActive = reader.GetBoolean(5)
                        };
                        binconfig.Add(bincongrequest);
                    }
                }
                return binconfig;
            }
        }

        public async Task<BinConfigRequest?> GetBinConfigByIdAsync(int id)
        {
            BinConfigRequest? binConfigRequest = null;
            using (var conn = _databaseConfig.GetConnection())
            {
                Squery = "SELECT * FROM OBNC WHERE ID = @id";
                var cmd = new SqlCommand(Squery, conn);
                cmd.Parameters.AddWithValue("@id", id);
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        binConfigRequest = new BinConfigRequest
                        {
                            BinConfigId = reader.GetInt32(0),
                            BinCode = reader.GetString(1),
                            BinName = reader.GetString(2),
                            Prefix = reader.GetString(3),
                            WhsCode = reader.GetString(4),
                        };
                    }
                }

            }
            return binConfigRequest;
        }
        
        public async Task<bool> UpdateBinConfigAsync(int id,BinConfigRequest binConfig)
        {
            binConfig.UpdatedOn = DateTime.Now;

            using (var conn = _databaseConfig.GetConnection())
            {
                try
                {
                    await conn.OpenAsync();

                    Squery = "UPDATE OBNC SET BinCode = @BinCode, BinName = @BinName, Prefix = @Prefix,WhsCode=@WhsCode, IsActive = @IsActive," +
                    "UpdatedBy = @UpdatedBy, UpdatedOn = @UpdatedOn WHERE ID = @ID;";

                    using (var cmd = new SqlCommand(Squery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", id);
                        cmd.Parameters.AddWithValue("@BinCode", binConfig.BinCode);
                        cmd.Parameters.AddWithValue("@BinName", binConfig.BinName);
                        cmd.Parameters.AddWithValue("@Prefix", binConfig.Prefix);                       
                        cmd.Parameters.AddWithValue("@WhsCode", binConfig.WhsCode);
                        cmd.Parameters.AddWithValue("@IsActive", binConfig.IsActive);
                        cmd.Parameters.AddWithValue("@UpdatedBy", binConfig.UpdatedBy);
                        cmd.Parameters.AddWithValue("@UpdatedOn", binConfig.UpdatedOn);

                        var result = await cmd.ExecuteNonQueryAsync();
                        return result>0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while Updating the BinConfig.", ex);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

    }
}

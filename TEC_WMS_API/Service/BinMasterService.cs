using System.Data.SqlClient;
using TEC_WMS_API.Data;
using TEC_WMS_API.Interface;
using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Service
{
    public class BinMasterService : IBinMaster
    {
        string Squery = string.Empty;
        private readonly DatabaseConfig _databaseConfig;
        public BinMasterService(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }
        public async Task<int> CreateBinMasterAsync(BinMasterRequest binMaster)
        {
            binMaster.CreatedOn = DateTime.Now;

            using (var conn = _databaseConfig.GetConnection())
            {
                try
                {
                    await conn.OpenAsync();

                    Squery = @"INSERT INTO OBNM
                            (Floor, Rack, Bin, BinLocCode, WhsCode, Height, Width, Length, 
                             Brand, Category, Quantity, Level, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn)
                            VALUES 
                            (@Floor, @Rack, @Bin, @BinLocCode, @WhsCode, @Height, @Width, @Length, 
                             @Brand, @Category, @Quantity, @Level, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn);
                            SELECT SCOPE_IDENTITY();";

                    using (var cmd = new SqlCommand(Squery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Floor", binMaster.Floor);
                        cmd.Parameters.AddWithValue("@Rack", binMaster.Rack);
                        cmd.Parameters.AddWithValue("@Bin", binMaster.Bin);
                        cmd.Parameters.AddWithValue("@BinLocCode", binMaster.BinLocCode);
                        cmd.Parameters.AddWithValue("@WhsCode", binMaster.WhsCode);
                        cmd.Parameters.AddWithValue("@Height", binMaster.Height);
                        cmd.Parameters.AddWithValue("@Width", binMaster.Width);
                        cmd.Parameters.AddWithValue("@Length", binMaster.Length);
                        cmd.Parameters.AddWithValue("@Brand", binMaster.Brand);
                        cmd.Parameters.AddWithValue("@Category", binMaster.Category);
                        cmd.Parameters.AddWithValue("@Quantity", binMaster.Quantity);
                        cmd.Parameters.AddWithValue("@Level", binMaster.Level);
                        cmd.Parameters.AddWithValue("@CreatedBy", binMaster.CreatedBy);
                        cmd.Parameters.AddWithValue("@CreatedOn", binMaster.CreatedOn ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@UpdatedBy", DBNull.Value);
                        cmd.Parameters.AddWithValue("@UpdatedOn", DBNull.Value);

                        var result = await cmd.ExecuteScalarAsync();
                        return Convert.ToInt32(result);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while creating the BinMaster.", ex);
                }
                finally
                {
                    conn.Close();
                }
            }


        }
        public async Task<IEnumerable<BinMasterRequest>> GetAllMasterConfigAsync()
        {
            var binMasterList = new List<BinMasterRequest>();

            using (var conn = _databaseConfig.GetConnection())
            {
                Squery = "SELECT * FROM OBNM";
                var cmd = new SqlCommand(Squery, conn);
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var binMasterRequest = new BinMasterRequest
                        {
                            BinID = reader["BinID"] as int? ?? 0,
                            Floor = reader["Floor"] as string ?? string.Empty,
                            Rack = reader["Rack"] as string ?? string.Empty,
                            Bin = reader["Bin"] as string ?? string.Empty,
                            BinLocCode = reader["BinLocCode"] as string ?? string.Empty,
                            WhsCode = reader["WhsCode"] as string ?? string.Empty,
                            Height = reader["Height"] as decimal? ?? 0m,
                            Width = reader["Width"] as decimal? ?? 0m,
                            Length = reader["Length"] as decimal? ?? 0m,
                            Brand = reader["Brand"] as string ?? string.Empty,
                            Category = reader["Category"] as string ?? string.Empty,
                            Quantity = reader["Quantity"] as int? ?? 0,
                            Level = reader["Level"] as int? ?? 0,
                            CreatedBy = reader["CreatedBy"] as string ?? string.Empty,
                            CreatedOn = reader["CreatedOn"] as DateTime?,
                            UpdatedBy = reader["UpdatedBy"] as string ?? string.Empty,
                            UpdatedOn = reader["UpdatedOn"] as DateTime?
                        };

                        binMasterList.Add(binMasterRequest);
                    }
                }
            }

            return binMasterList;
        }

        public async Task<BinMasterRequest?> GetBinMasterByIdAsync(int id)
        {
            BinMasterRequest? binMasterRequest = null;
            using (var conn = _databaseConfig.GetConnection())
            {
                Squery = "SELECT * FROM OBNM WHERE BinID = @id";
                var cmd = new SqlCommand(Squery, conn);
                cmd.Parameters.AddWithValue("@id", id);
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        binMasterRequest = new BinMasterRequest
                        {
                            BinID = reader["BinID"] as int? ?? 0,
                            Floor = reader["Floor"] as string ?? string.Empty,
                            Rack = reader["Rack"] as string ?? string.Empty,
                            Bin = reader["Bin"] as string ?? string.Empty,
                            BinLocCode = reader["BinLocCode"] as string ?? string.Empty,
                            WhsCode = reader["WhsCode"] as string ?? string.Empty,
                            Height = reader["Height"] as decimal? ?? 0m,
                            Width = reader["Width"] as decimal? ?? 0m,
                            Length = reader["Length"] as decimal? ?? 0m,
                            Brand = reader["Brand"] as string ?? string.Empty,
                            Category = reader["Category"] as string ?? string.Empty,
                            Quantity = reader["Quantity"] as int? ?? 0,
                            Level = reader["Level"] as int? ?? 0,
                            CreatedBy = reader["CreatedBy"] as string ?? string.Empty,
                            CreatedOn = reader["CreatedOn"] as DateTime?,
                            UpdatedBy = reader["UpdatedBy"] as string ?? string.Empty,
                            UpdatedOn = reader["UpdatedOn"] as DateTime?
                        };
                    }
                }

            }
            return binMasterRequest;
        }

        public async Task<bool> UpdateBinMasterAsync(int binId, BinMasterRequest binMaster)
        {
            binMaster.UpdatedOn = DateTime.Now;

            using (var conn = _databaseConfig.GetConnection())
            {
                try
                {
                    await conn.OpenAsync();

                    string query = @"UPDATE OBNM 
                             SET Floor = @Floor, 
                                 Rack = @Rack, 
                                 Bin = @Bin, 
                                 BinLocCode = @BinLocCode,
                                 WhsCode = @WhsCode,
                                 Height = @Height,
                                 Width = @Width,
                                 Length = @Length,
                                 Brand = @Brand,
                                 Category = @Category,
                                 Quantity = @Quantity,
                                 Level = @Level,
                                 UpdatedBy = @UpdatedBy,
                                 UpdatedOn = @UpdatedOn
                             WHERE BinID = @BinID;";

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BinID", binId);
                        cmd.Parameters.AddWithValue("@Floor", binMaster.Floor);
                        cmd.Parameters.AddWithValue("@Rack", binMaster.Rack);
                        cmd.Parameters.AddWithValue("@Bin", binMaster.Bin);
                        cmd.Parameters.AddWithValue("@BinLocCode", binMaster.BinLocCode);
                        cmd.Parameters.AddWithValue("@WhsCode", binMaster.WhsCode);
                        cmd.Parameters.AddWithValue("@Height", binMaster.Height);
                        cmd.Parameters.AddWithValue("@Width", binMaster.Width);
                        cmd.Parameters.AddWithValue("@Length", binMaster.Length);
                        cmd.Parameters.AddWithValue("@Brand", binMaster.Brand);
                        cmd.Parameters.AddWithValue("@Category", binMaster.Category);
                        cmd.Parameters.AddWithValue("@Quantity", binMaster.Quantity);
                        cmd.Parameters.AddWithValue("@Level", binMaster.Level);
                        cmd.Parameters.AddWithValue("@UpdatedBy", binMaster.UpdatedBy);
                        cmd.Parameters.AddWithValue("@UpdatedOn", binMaster.UpdatedOn);

                        var result = await cmd.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while updating the BinMaster.", ex);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public async Task<bool> DeleteBinMasterAsync(int id)
        {
            using (var conn = _databaseConfig.GetConnection())
            {
                Squery = "DELETE FROM OBNM WHERE BinID = @id;";
                var cmd = new SqlCommand(Squery, conn);
                cmd.Parameters.AddWithValue("@id", id);
                await conn.OpenAsync();
                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

    }
}

using System.Data;
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
        public async Task<int> CreateBinMasterAsync(List<BinMasterRequest> binMasters)
        {
            if (binMasters == null || binMasters.Count == 0)
                throw new ArgumentException("No data provided for insertion.");

            using (var conn = _databaseConfig.GetConnection())
            {
                try
                {
                    await conn.OpenAsync();

                    using (var transaction = conn.BeginTransaction()) // Ensure transaction safety
                    {
                        using (var bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.DestinationTableName = "OBNM";

                            // Map columns
                            bulkCopy.ColumnMappings.Add("WhsCode", "WhsCode");
                            bulkCopy.ColumnMappings.Add("BinLocCode", "BinLocCode");
                            bulkCopy.ColumnMappings.Add("SL1Code", "SL1Code");
                            bulkCopy.ColumnMappings.Add("SL2Code", "SL2Code");
                            bulkCopy.ColumnMappings.Add("SL3Code", "SL3Code");
                            bulkCopy.ColumnMappings.Add("SL4Code", "SL4Code");
                            bulkCopy.ColumnMappings.Add("SL5Code", "SL5Code");
                            bulkCopy.ColumnMappings.Add("Height", "Height");
                            bulkCopy.ColumnMappings.Add("Width", "Width");
                            bulkCopy.ColumnMappings.Add("Length", "Length");
                            bulkCopy.ColumnMappings.Add("Filter1", "Filter1");
                            bulkCopy.ColumnMappings.Add("Filter2", "Filter2");
                            bulkCopy.ColumnMappings.Add("Filter3", "Filter3");
                            bulkCopy.ColumnMappings.Add("Quantity", "Quantity");
                            bulkCopy.ColumnMappings.Add("Level", "Level");
                            bulkCopy.ColumnMappings.Add("Active", "Active");
                            bulkCopy.ColumnMappings.Add("UserSign", "UserSign");

                            // Convert List to DataTable
                            var dataTable = ConvertToDataTable(binMasters);

                            await bulkCopy.WriteToServerAsync(dataTable);

                            transaction.Commit(); // Commit transaction
                        }
                    }

                    return binMasters.Count; // Return count of inserted rows
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while creating BinMaster.", ex);
                }
                finally
                {
                    await conn.CloseAsync();
                }
            }
        }
        public async Task<int> CreateBinMasterTempAsync(List<BinMasterRequest> binMasters)
        {
            if (binMasters == null || binMasters.Count == 0)
                throw new ArgumentException("No data provided for insertion.");

            using (var conn = _databaseConfig.GetConnection())
            {
                try
                {
                    await conn.OpenAsync();

                    using (var transaction = conn.BeginTransaction()) // Ensure transaction safety
                    {
                        using (var bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, transaction))
                        {
                            bulkCopy.DestinationTableName = "OBNM_TEMP";

                            // Map columns
                            bulkCopy.ColumnMappings.Add("WhsCode", "WhsCode");
                            bulkCopy.ColumnMappings.Add("BinLocCode", "BinLocCode");
                            bulkCopy.ColumnMappings.Add("SL1Code", "SL1Code");
                            bulkCopy.ColumnMappings.Add("SL2Code", "SL2Code");
                            bulkCopy.ColumnMappings.Add("SL3Code", "SL3Code");
                            bulkCopy.ColumnMappings.Add("SL4Code", "SL4Code");
                            bulkCopy.ColumnMappings.Add("SL5Code", "SL5Code");
                            bulkCopy.ColumnMappings.Add("Height", "Height");
                            bulkCopy.ColumnMappings.Add("Width", "Width");
                            bulkCopy.ColumnMappings.Add("Length", "Length");
                            bulkCopy.ColumnMappings.Add("Filter1", "Filter1");
                            bulkCopy.ColumnMappings.Add("Filter2", "Filter2");
                            bulkCopy.ColumnMappings.Add("Filter3", "Filter3");
                            bulkCopy.ColumnMappings.Add("Quantity", "Quantity");
                            bulkCopy.ColumnMappings.Add("Level", "Level");
                            bulkCopy.ColumnMappings.Add("Active", "Active");
                            bulkCopy.ColumnMappings.Add("UserSign", "UserSign");

                            // Convert List to DataTable
                            var dataTable = ConvertToDataTable(binMasters);

                            await bulkCopy.WriteToServerAsync(dataTable);

                            transaction.Commit(); // Commit transaction
                        }
                    }

                    return binMasters.Count; // Return count of inserted rows
                }
                catch (Exception ex)
                {
                    throw new Exception("An error occurred while creating BinMaster.", ex);
                }
                finally
                {
                    await conn.CloseAsync();
                }
            }
        }


        private DataTable ConvertToDataTable(List<BinMasterRequest> binMasters)
        {
            var table = new DataTable();

            table.Columns.Add("WhsCode", typeof(string));
            table.Columns.Add("BinLocCode", typeof(string));
            table.Columns.Add("SL1Code", typeof(string));
            table.Columns.Add("SL2Code", typeof(string));
            table.Columns.Add("SL3Code", typeof(string));
            table.Columns.Add("SL4Code", typeof(string));
            table.Columns.Add("SL5Code", typeof(string));
            table.Columns.Add("Height", typeof(decimal));
            table.Columns.Add("Width", typeof(decimal));
            table.Columns.Add("Length", typeof(decimal));
            table.Columns.Add("Filter1", typeof(string));
            table.Columns.Add("Filter2", typeof(string));
            table.Columns.Add("Filter3", typeof(string));
            table.Columns.Add("Quantity", typeof(int));
            table.Columns.Add("Level", typeof(int));
            table.Columns.Add("Active", typeof(bool));
            table.Columns.Add("UserSign", typeof(string));

            foreach (var bin in binMasters)
            {
                table.Rows.Add(
                    bin.WhsCode,
                    bin.BinLocCode,
                    bin.SL1Code ?? (object)DBNull.Value,
                    bin.SL2Code ?? (object)DBNull.Value,
                    bin.SL3Code ?? (object)DBNull.Value,
                    bin.SL4Code ?? (object)DBNull.Value,
                    bin.SL5Code ?? (object)DBNull.Value,
                    bin.Height,
                    bin.Width,
                    bin.Length,
                    bin.Filter1 ?? (object)DBNull.Value,
                    bin.Filter2 ?? (object)DBNull.Value,
                    bin.Filter3 ?? (object)DBNull.Value,
                    bin.Quantity,
                    bin.Level,
                    bin.Active ?? (object)DBNull.Value,
                    bin.UserSign ?? (object)DBNull.Value
                );
            }

            return table;
        }

        public async Task<IEnumerable<BinMasterRequest>> GetAllMasterConfigAsync()
        {
            var binMasterList = new List<BinMasterRequest>();

            using (var conn = _databaseConfig.GetConnection())
            {
                string query = "SELECT * FROM OBNM";
                var cmd = new SqlCommand(query, conn);
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var binMasterRequest = new BinMasterRequest
                        {
                            BinID = reader.IsDBNull(reader.GetOrdinal("BinID")) ? 0 : reader.GetInt32(reader.GetOrdinal("BinID")),
                            BinLocCode = reader.IsDBNull(reader.GetOrdinal("BinLocCode")) ? string.Empty : reader.GetString(reader.GetOrdinal("BinLocCode")),
                            WhsCode = reader.IsDBNull(reader.GetOrdinal("WhsCode")) ? string.Empty : reader.GetString(reader.GetOrdinal("WhsCode")),
                            SL1Code = reader.IsDBNull(reader.GetOrdinal("SL1Code")) ? null : reader.GetString(reader.GetOrdinal("SL1Code")),
                            SL2Code = reader.IsDBNull(reader.GetOrdinal("SL2Code")) ? null : reader.GetString(reader.GetOrdinal("SL2Code")),
                            SL3Code = reader.IsDBNull(reader.GetOrdinal("SL3Code")) ? null : reader.GetString(reader.GetOrdinal("SL3Code")),
                            SL4Code = reader.IsDBNull(reader.GetOrdinal("SL4Code")) ? null : reader.GetString(reader.GetOrdinal("SL4Code")),
                            SL5Code = reader.IsDBNull(reader.GetOrdinal("SL5Code")) ? null : reader.GetString(reader.GetOrdinal("SL5Code")),
                            Height = reader.IsDBNull(reader.GetOrdinal("Height")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Height")),
                            Width = reader.IsDBNull(reader.GetOrdinal("Width")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Width")),
                            Length = reader.IsDBNull(reader.GetOrdinal("Length")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Length")),
                            Filter1 = reader.IsDBNull(reader.GetOrdinal("Filter1")) ? null : reader.GetString(reader.GetOrdinal("Filter1")),
                            Filter2 = reader.IsDBNull(reader.GetOrdinal("Filter2")) ? null : reader.GetString(reader.GetOrdinal("Filter2")),
                            Filter3 = reader.IsDBNull(reader.GetOrdinal("Filter3")) ? null : reader.GetString(reader.GetOrdinal("Filter3")),
                            Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Quantity")),
                            Level = reader.IsDBNull(reader.GetOrdinal("Level")) ? 0 : reader.GetInt32(reader.GetOrdinal("Level")),
                            Active = !reader.IsDBNull(reader.GetOrdinal("Active")) && reader.GetBoolean(reader.GetOrdinal("Active")),

                            UserSign = reader.IsDBNull(reader.GetOrdinal("UserSign")) ? string.Empty : reader.GetString(reader.GetOrdinal("UserSign"))
                        };

                        binMasterList.Add(binMasterRequest);
                    }
                }
            }

            return binMasterList;
        }

        public async Task<IEnumerable<BinMasterRequest>> GetBinMasterByWhsAsync(string whsCode)
        {
            var binMasterList = new List<BinMasterRequest>();

            using (var conn = _databaseConfig.GetConnection())
            {
                string query = "SELECT * FROM OBNM where WhsCode = @whsCode";

                var cmd = new SqlCommand(query, conn);
             
                cmd.Parameters.AddWithValue("@whsCode", whsCode);
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var binMasterRequest = new BinMasterRequest
                        {
                            BinID = reader.IsDBNull(reader.GetOrdinal("BinID")) ? 0 : reader.GetInt32(reader.GetOrdinal("BinID")),
                            BinLocCode = reader.IsDBNull(reader.GetOrdinal("BinLocCode")) ? string.Empty : reader.GetString(reader.GetOrdinal("BinLocCode")),
                            WhsCode = reader.IsDBNull(reader.GetOrdinal("WhsCode")) ? string.Empty : reader.GetString(reader.GetOrdinal("WhsCode")),
                            SL1Code = reader.IsDBNull(reader.GetOrdinal("SL1Code")) ? null : reader.GetString(reader.GetOrdinal("SL1Code")),
                            SL2Code = reader.IsDBNull(reader.GetOrdinal("SL2Code")) ? null : reader.GetString(reader.GetOrdinal("SL2Code")),
                            SL3Code = reader.IsDBNull(reader.GetOrdinal("SL3Code")) ? null : reader.GetString(reader.GetOrdinal("SL3Code")),
                            SL4Code = reader.IsDBNull(reader.GetOrdinal("SL4Code")) ? null : reader.GetString(reader.GetOrdinal("SL4Code")),
                            SL5Code = reader.IsDBNull(reader.GetOrdinal("SL5Code")) ? null : reader.GetString(reader.GetOrdinal("SL5Code")),
                            Height = reader.IsDBNull(reader.GetOrdinal("Height")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Height")),
                            Width = reader.IsDBNull(reader.GetOrdinal("Width")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Width")),
                            Length = reader.IsDBNull(reader.GetOrdinal("Length")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Length")),
                            Filter1 = reader.IsDBNull(reader.GetOrdinal("Filter1")) ? null : reader.GetString(reader.GetOrdinal("Filter1")),
                            Filter2 = reader.IsDBNull(reader.GetOrdinal("Filter2")) ? null : reader.GetString(reader.GetOrdinal("Filter2")),
                            Filter3 = reader.IsDBNull(reader.GetOrdinal("Filter3")) ? null : reader.GetString(reader.GetOrdinal("Filter3")),
                            Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Quantity")),
                            Level = reader.IsDBNull(reader.GetOrdinal("Level")) ? 0 : reader.GetInt32(reader.GetOrdinal("Level")),
                            Active = !reader.IsDBNull(reader.GetOrdinal("Active")) && reader.GetBoolean(reader.GetOrdinal("Active")),

                            UserSign = reader.IsDBNull(reader.GetOrdinal("UserSign")) ? string.Empty : reader.GetString(reader.GetOrdinal("UserSign"))
                        };

                        binMasterList.Add(binMasterRequest);
                    }
                }
            }

            return binMasterList;
        }

        public async Task<IEnumerable<BinMasterRequest>> ValBinMasterDataAsync(string userSign)
        {
            var binMasterList = new List<BinMasterRequest>();

            using (var conn = _databaseConfig.GetConnection())
            {
                await conn.OpenAsync();

                // Call the stored procedure first
                using (var spCmd = new SqlCommand("IMPORT_DATA_VALIDATION", conn))
                {
                    spCmd.CommandType = CommandType.StoredProcedure;
                    spCmd.Parameters.AddWithValue("@UserSign", userSign);
                    await spCmd.ExecuteNonQueryAsync(); // Execute the stored procedure
                }

                // Now, execute the SELECT query
                string query = "SELECT * FROM OBNM_TEMP WHERE UserSign = @UserSign";
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserSign", userSign);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var binMasterRequest = new BinMasterRequest
                            {
                                BinID = reader.IsDBNull(reader.GetOrdinal("BinID")) ? 0 : reader.GetInt32(reader.GetOrdinal("BinID")),
                                BinLocCode = reader.IsDBNull(reader.GetOrdinal("BinLocCode")) ? string.Empty : reader.GetString(reader.GetOrdinal("BinLocCode")),
                                WhsCode = reader.IsDBNull(reader.GetOrdinal("WhsCode")) ? string.Empty : reader.GetString(reader.GetOrdinal("WhsCode")),
                                SL1Code = reader.IsDBNull(reader.GetOrdinal("SL1Code")) ? null : reader.GetString(reader.GetOrdinal("SL1Code")),
                                SL2Code = reader.IsDBNull(reader.GetOrdinal("SL2Code")) ? null : reader.GetString(reader.GetOrdinal("SL2Code")),
                                SL3Code = reader.IsDBNull(reader.GetOrdinal("SL3Code")) ? null : reader.GetString(reader.GetOrdinal("SL3Code")),
                                SL4Code = reader.IsDBNull(reader.GetOrdinal("SL4Code")) ? null : reader.GetString(reader.GetOrdinal("SL4Code")),
                                SL5Code = reader.IsDBNull(reader.GetOrdinal("SL5Code")) ? null : reader.GetString(reader.GetOrdinal("SL5Code")),
                                Height = reader.IsDBNull(reader.GetOrdinal("Height")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Height")),
                                Width = reader.IsDBNull(reader.GetOrdinal("Width")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Width")),
                                Length = reader.IsDBNull(reader.GetOrdinal("Length")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Length")),
                                Filter1 = reader.IsDBNull(reader.GetOrdinal("Filter1")) ? null : reader.GetString(reader.GetOrdinal("Filter1")),
                                Filter2 = reader.IsDBNull(reader.GetOrdinal("Filter2")) ? null : reader.GetString(reader.GetOrdinal("Filter2")),
                                Filter3 = reader.IsDBNull(reader.GetOrdinal("Filter3")) ? null : reader.GetString(reader.GetOrdinal("Filter3")),
                                Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Quantity")),
                                Level = reader.IsDBNull(reader.GetOrdinal("Level")) ? 0 : reader.GetInt32(reader.GetOrdinal("Level")),
                                Active = !reader.IsDBNull(reader.GetOrdinal("Active")) && reader.GetBoolean(reader.GetOrdinal("Active")),
                                UserSign = reader.IsDBNull(reader.GetOrdinal("UserSign")) ? string.Empty : reader.GetString(reader.GetOrdinal("UserSign"))
                            };

                            binMasterList.Add(binMasterRequest);
                        }
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
                            BinID = reader.IsDBNull(reader.GetOrdinal("BinID")) ? 0 : reader.GetInt32(reader.GetOrdinal("BinID")),
                            BinLocCode = reader.IsDBNull(reader.GetOrdinal("BinLocCode")) ? string.Empty : reader.GetString(reader.GetOrdinal("BinLocCode")),
                            WhsCode = reader.IsDBNull(reader.GetOrdinal("WhsCode")) ? string.Empty : reader.GetString(reader.GetOrdinal("WhsCode")),
                            SL1Code = reader.IsDBNull(reader.GetOrdinal("SL1Code")) ? null : reader.GetString(reader.GetOrdinal("SL1Code")),
                            SL2Code = reader.IsDBNull(reader.GetOrdinal("SL2Code")) ? null : reader.GetString(reader.GetOrdinal("SL2Code")),
                            SL3Code = reader.IsDBNull(reader.GetOrdinal("SL3Code")) ? null : reader.GetString(reader.GetOrdinal("SL3Code")),
                            SL4Code = reader.IsDBNull(reader.GetOrdinal("SL4Code")) ? null : reader.GetString(reader.GetOrdinal("SL4Code")),
                            SL5Code = reader.IsDBNull(reader.GetOrdinal("SL5Code")) ? null : reader.GetString(reader.GetOrdinal("SL5Code")),
                            Height = reader.IsDBNull(reader.GetOrdinal("Height")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Height")),
                            Width = reader.IsDBNull(reader.GetOrdinal("Width")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Width")),
                            Length = reader.IsDBNull(reader.GetOrdinal("Length")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Length")),
                            Filter1 = reader.IsDBNull(reader.GetOrdinal("Filter1")) ? null : reader.GetString(reader.GetOrdinal("Filter1")),
                            Filter2 = reader.IsDBNull(reader.GetOrdinal("Filter2")) ? null : reader.GetString(reader.GetOrdinal("Filter2")),
                            Filter3 = reader.IsDBNull(reader.GetOrdinal("Filter3")) ? null : reader.GetString(reader.GetOrdinal("Filter3")),
                            Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Quantity")),
                            Level = reader.IsDBNull(reader.GetOrdinal("Level")) ? 0 : reader.GetInt32(reader.GetOrdinal("Level")),
                            Active = !reader.IsDBNull(reader.GetOrdinal("Active")) && reader.GetBoolean(reader.GetOrdinal("Active")),

                            UserSign = reader.IsDBNull(reader.GetOrdinal("UserSign")) ? string.Empty : reader.GetString(reader.GetOrdinal("UserSign"))
                        };
                    }
                }

            }
            return binMasterRequest;
        }

        public async Task<bool> UpdateBinMasterAsync(int binId, BinMasterRequest binMaster)
        {
            using (var conn = _databaseConfig.GetConnection())
            {
                await conn.OpenAsync();

                string query = @"UPDATE OBNM 
                         SET BinLocCode = @BinLocCode,
                             WhsCode = @WhsCode,
                             SL1Code = @SL1Code,
                             SL2Code = @SL2Code,
                             SL3Code = @SL3Code,
                             SL4Code = @SL4Code,
                             SL5Code = @SL5Code,
                             Height = @Height,
                             Width = @Width,
                             Length = @Length,
                             Filter1 = @Filter1,
                             Filter2 = @Filter2,
                             Filter3 = @Filter3,
                             Quantity = @Quantity,
                             Level = @Level,
                             Active = @Active,
                             UserSign = @UserSign
                            
                         WHERE BinID = @BinID;";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BinID", binId);
                    cmd.Parameters.AddWithValue("@BinLocCode", binMaster.BinLocCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@WhsCode", binMaster.WhsCode ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SL1Code", binMaster.SL1Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SL2Code", binMaster.SL2Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SL3Code", binMaster.SL3Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SL4Code", binMaster.SL4Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SL5Code", binMaster.SL5Code ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Height", binMaster.Height);
                    cmd.Parameters.AddWithValue("@Width", binMaster.Width);
                    cmd.Parameters.AddWithValue("@Length", binMaster.Length);
                    cmd.Parameters.AddWithValue("@Filter1", binMaster.Filter1 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Filter2", binMaster.Filter2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Filter3", binMaster.Filter3 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Quantity", binMaster.Quantity);
                    cmd.Parameters.AddWithValue("@Level", binMaster.Level);
                    cmd.Parameters.AddWithValue("@Active", binMaster.Active ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@UserSign", binMaster.UserSign ?? (object)DBNull.Value);
                    

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
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
        public async Task<bool> DeleteBinMasterTempAsync(string userSign)
        {
            using (var conn = _databaseConfig.GetConnection())
            {
                Squery = "DELETE FROM OBNM_TEMP WHERE UserSign = @userSign;";
                var cmd = new SqlCommand(Squery, conn);
                cmd.Parameters.AddWithValue("@userSign", userSign);
                await conn.OpenAsync();
                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

    }
}

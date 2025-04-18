﻿using System.Data;
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
        public async Task<string> CreateBinConfigsAsync(IEnumerable<BinConfigRequest> binConfigs)
        {
            using (var conn = _databaseConfig.GetConnection())
            {
                try
                {
                    await conn.OpenAsync();

                    int rowsInserted = 0;
                    string message = string.Empty;  // Initialize message string

                    foreach (var binConfig in binConfigs)
                    {
                        // Check if the BinCode and WhsCode already exist in the database
                        string checkQuery = "SELECT COUNT(*) FROM OBNC WHERE BinCode = @BinCode AND WhsCode = @WhsCode";

                        using (var checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@BinCode", binConfig.BinCode);
                            checkCmd.Parameters.AddWithValue("@WhsCode", binConfig.WhsCode);

                            int count = (int)await checkCmd.ExecuteScalarAsync();

                            // If the combination already exists, return a message and stop processing further.
                            if (count > 0)
                            {
                                message = $"The WhsCode '{binConfig.WhsCode}' already exists.";
                                return message;  // Early return with the message
                            }
                        }

                        // If not exists, insert the new record
                        string query = "INSERT INTO OBNC (BinCode, BinName, Prefix, WhsCode, IsActive, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn) " +
                                       "VALUES (@BinCode, @BinName, @Prefix, @WhsCode, @IsActive, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn);";

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

                    return rowsInserted > 0 ? $"BinConfig Saved successfully." : "No records were inserted.";
                }
                catch (Exception ex)
                {
                    var logEntry = new ExceptionLog
                    {
                        LogLevel = "Error",
                        MethodName = nameof(CreateBinConfigsAsync),
                        ModuleID = null,
                        ExceptionMessage = ex.Message, 
                        Parameters = $"{binConfigs}",
                        StackTrace = string.Empty, 
                        EventTimestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")
                    };
                    _databaseConfig.InsertExceptionLogSP(logEntry);                   
                    throw new Exception("An error occurred while creating the BinConfigs.", ex);
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        public async Task<bool> DeleteBinConfigAsync(string whsCode)
        {
            try
            {
                using (var conn = _databaseConfig.GetConnection())
                {
                    Squery = "DELETE FROM OBNC WHERE WhsCode = @whsCode;";
                    var cmd = new SqlCommand(Squery, conn);
                    cmd.Parameters.AddWithValue("@whsCode", whsCode);
                    await conn.OpenAsync();
                    var rowsAffected = await cmd.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                var logEntry = new ExceptionLog
                {
                    LogLevel = "Error",
                    MethodName = nameof(DeleteBinConfigAsync),
                    ModuleID = null,
                    ExceptionMessage = ex.Message,
                    Parameters = $"{whsCode}",
                    StackTrace = string.Empty,
                    EventTimestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")
                };
                _databaseConfig.InsertExceptionLogSP(logEntry);
                throw;
            }
            
        }

        public async Task<IEnumerable<BinConfigRequest>> GetAllBinConfigAsync()
        {
            try
            {
                var binconfig = new List<BinConfigRequest>();
                using (var conn = _databaseConfig.GetConnection())
                {
                    Squery = "SELECT MIN(ID) AS ID, WhsCode, MIN(CAST(IsActive AS INT)) AS IsActive, MIN(CreatedOn) AS CreatedOn FROM OBNC GROUP BY WhsCode;";
                    var cmd = new SqlCommand(Squery, conn);
                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        for (int i = 0; await reader.ReadAsync(); i++)
                        {
                            var bincongrequest = new BinConfigRequest
                            {
                                BinConfigId = reader.GetInt32(0),
                                WhsCode = reader.GetString(1),
                                IsActive = reader.GetInt32(2) != 0,
                                CreatedOn = reader.GetDateTime(3),
                            };
                            binconfig.Add(bincongrequest);
                        }
                    }
                    return binconfig;
                }
            }
            catch (Exception ex)
            {
                var logEntry = new ExceptionLog
                {
                    LogLevel = "Error",
                    MethodName = nameof(GetAllBinConfigAsync),
                    ModuleID = null,
                    ExceptionMessage = ex.Message,
                    Parameters = $"{""}",
                    StackTrace = string.Empty,
                    EventTimestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")
                };
                _databaseConfig.InsertExceptionLogSP(logEntry);
                throw;
            }
           
           
        }
         public async Task<IEnumerable<BinConfigRequest>> GetAllBinListbywhsConfigAsync(string whsCode)
        {
            try
            {
                var binconfig = new List<BinConfigRequest>();
                using (var conn = _databaseConfig.GetConnection())
                {
                    Squery = "SELECT * FROM OBNC where WhsCode='" + whsCode + "'";
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
                                IsActive = reader.GetBoolean(5),
                                CreatedBy = reader.GetString(6),
                                CreatedOn = reader.GetDateTime(7),
                            };
                            binconfig.Add(bincongrequest);
                        }
                    }
                    return binconfig;
                }
            }
            catch (Exception ex)
            {
                var logEntry = new ExceptionLog
                {
                    LogLevel = "Error",
                    MethodName = nameof(GetAllBinListbywhsConfigAsync),
                    ModuleID = null,
                    ExceptionMessage = ex.Message,
                    Parameters = $"{whsCode}",
                    StackTrace = string.Empty,
                    EventTimestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")
                };
                _databaseConfig.InsertExceptionLogSP(logEntry);
                throw;
            }
            
        }

        public async Task<BinConfigRequest?> GetBinConfigByIdAsync(int id)
        {
            try
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
            catch (Exception ex)
            {

                var logEntry = new ExceptionLog
                {
                    LogLevel = "Error",
                    MethodName = nameof(GetBinConfigByIdAsync),
                    ModuleID = null,
                    ExceptionMessage = ex.Message, // Custom message
                    Parameters = $"{id}",
                    StackTrace = string.Empty, // No stack trace here
                    EventTimestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")
                };
                _databaseConfig.InsertExceptionLogSP(logEntry);
                throw;

            }
        }

        public async Task<bool> UpdateBinConfigAsync(IEnumerable<BinConfigRequest> binConfigs)
        {
            using (var conn = _databaseConfig.GetConnection())
            {
                try
                {
                    await conn.OpenAsync();
                    bool allUpdatesSucceeded = true;

                    foreach (var binConfig in binConfigs)
                    {
                        binConfig.UpdatedOn = DateTime.Now; // Set the updated timestamp for each config.

                        // Check if the BinCode and WhsCode combination exists
                        var checkQuery = "SELECT COUNT(1) FROM OBNC WHERE BinCode = @BinCode AND WhsCode = @WhsCode;";

                        using (var checkCmd = new SqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.Add(new SqlParameter("@BinCode", SqlDbType.VarChar) { Value = binConfig.BinCode });
                            checkCmd.Parameters.Add(new SqlParameter("@WhsCode", SqlDbType.VarChar) { Value = binConfig.WhsCode });

                            var exists = (int)await checkCmd.ExecuteScalarAsync() > 0;

                            if (exists)
                            {
                                // Update query if record exists
                                var updateQuery = "UPDATE OBNC SET BinName = @BinName, Prefix = @Prefix, IsActive = @IsActive, " +
                                                  "UpdatedBy = @UpdatedBy, UpdatedOn = @UpdatedOn " +
                                                  "WHERE BinCode = @BinCode AND WhsCode = @WhsCode;";

                                using (var updateCmd = new SqlCommand(updateQuery, conn))
                                {
                                    updateCmd.Parameters.Add(new SqlParameter("@BinCode", SqlDbType.VarChar) { Value = binConfig.BinCode });
                                    updateCmd.Parameters.Add(new SqlParameter("@BinName", SqlDbType.VarChar) { Value = binConfig.BinName });
                                    updateCmd.Parameters.Add(new SqlParameter("@Prefix", SqlDbType.VarChar) { Value = binConfig.Prefix });
                                    updateCmd.Parameters.Add(new SqlParameter("@WhsCode", SqlDbType.VarChar) { Value = binConfig.WhsCode });
                                    updateCmd.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = binConfig.IsActive });
                                    updateCmd.Parameters.Add(new SqlParameter("@UpdatedBy", SqlDbType.VarChar) { Value = binConfig.UpdatedBy });
                                    updateCmd.Parameters.Add(new SqlParameter("@UpdatedOn", SqlDbType.DateTime) { Value = binConfig.UpdatedOn });

                                    var updateResult = await updateCmd.ExecuteNonQueryAsync();
                                    if (updateResult <= 0)
                                    {
                                        allUpdatesSucceeded = false;
                                    }
                                }
                            }
                            else
                            {
                                // Insert query if record does not exist
                                var insertQuery = "INSERT INTO OBNC (BinCode, BinName, Prefix, WhsCode, IsActive, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn) " +
                                                  "VALUES (@BinCode, @BinName, @Prefix, @WhsCode, @IsActive, @CreatedBy, @CreatedOn, @UpdatedBy, @UpdatedOn);";
                                binConfig.CreatedOn = DateTime.Now;
                                using (var insertCmd = new SqlCommand(insertQuery, conn))
                                {
                                    insertCmd.Parameters.Add(new SqlParameter("@BinCode", SqlDbType.VarChar) { Value = binConfig.BinCode });
                                    insertCmd.Parameters.Add(new SqlParameter("@BinName", SqlDbType.VarChar) { Value = binConfig.BinName });
                                    insertCmd.Parameters.Add(new SqlParameter("@Prefix", SqlDbType.VarChar) { Value = binConfig.Prefix });
                                    insertCmd.Parameters.Add(new SqlParameter("@WhsCode", SqlDbType.VarChar) { Value = binConfig.WhsCode });
                                    insertCmd.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = binConfig.IsActive });
                                    insertCmd.Parameters.Add(new SqlParameter("@CreatedBy", SqlDbType.VarChar) { Value = binConfig.CreatedBy });  // Assuming this value exists in the request
                                    insertCmd.Parameters.Add(new SqlParameter("@CreatedOn", SqlDbType.DateTime) { Value = DateTime.Now });
                                    insertCmd.Parameters.Add(new SqlParameter("@UpdatedBy", DBNull.Value));
                                    insertCmd.Parameters.Add(new SqlParameter("@UpdatedOn",  DBNull.Value ));

                                    var insertResult = await insertCmd.ExecuteNonQueryAsync();
                                    if (insertResult <= 0)
                                    {
                                        allUpdatesSucceeded = false;
                                    }
                                }
                            }
                        }
                    }

                    return allUpdatesSucceeded;
                }
                catch (Exception ex)
                {
                    var logEntry = new ExceptionLog
                    {
                        LogLevel = "Error",
                        MethodName = nameof(UpdateBinConfigAsync),
                        ModuleID = null,
                        ExceptionMessage = ex.Message, // Custom message
                        Parameters = $"{binConfigs}",
                        StackTrace = string.Empty, // No stack trace here
                        EventTimestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")
                    };
                    _databaseConfig.InsertExceptionLogSP(logEntry);
                    throw new Exception("An error occurred while updating or creating the BinConfig.", ex);
                }
                finally
                {
                    await conn.CloseAsync();
                }
            }
        }


    }
}

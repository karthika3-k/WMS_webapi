using Microsoft.Extensions.Logging.Abstractions;
using System.Data;
using System.Data.SqlClient;
using TEC_WMS_API.Data;
using TEC_WMS_API.Interface;
using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Service
{
    public class InwardService : IInward
    {
        private readonly DatabaseConfig _databaseConfig;
        public InwardService(DatabaseConfig databaseConfig)
        {
            _databaseConfig = databaseConfig;
        }
        public async Task<IEnumerable<InwardRequest>> GetAllInwardAsync(string userSign)
        {
            try
            {
                var inwardList = new List<InwardRequest>();

                using (var conn = _databaseConfig.GetConnection())
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand("TEC_NOTALLOCATED_DOC", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserSign", userSign);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var inwardRequest = new InwardRequest
                                {
                                    ID = reader.GetOrdinal("ID") >= 0 && !reader.IsDBNull(reader.GetOrdinal("ID")) ? reader.GetInt32(reader.GetOrdinal("ID")) : 0,
                                    DocEntry = reader.IsDBNull(reader.GetOrdinal("DocEntry")) ? null : reader.GetInt32(reader.GetOrdinal("DocEntry")),
                                    DocNum = reader.IsDBNull(reader.GetOrdinal("DocNum")) ? null : reader.GetInt32(reader.GetOrdinal("DocNum")),
                                    DocDate = reader.IsDBNull(reader.GetOrdinal("DocDate")) ? null : reader.GetDateTime(reader.GetOrdinal("DocDate")),
                                    TransType = reader.IsDBNull(reader.GetOrdinal("TransType")) ? null : reader.GetInt32(reader.GetOrdinal("TransType")),
                                    CardCode = reader.IsDBNull(reader.GetOrdinal("CardCode")) ? null : reader.GetString(reader.GetOrdinal("CardCode")),
                                    CardName = reader.IsDBNull(reader.GetOrdinal("CardName")) ? null : reader.GetString(reader.GetOrdinal("CardName")),
                                    ItemCode = reader.IsDBNull(reader.GetOrdinal("ItemCode")) ? null : reader.GetString(reader.GetOrdinal("ItemCode")),
                                    ItemName = reader.IsDBNull(reader.GetOrdinal("ItemName")) ? null : reader.GetString(reader.GetOrdinal("ItemName")),
                                    Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity")) ? null : reader.GetDecimal(reader.GetOrdinal("Quantity")),
                                    CreatedDateTime = reader.IsDBNull(reader.GetOrdinal("CreatedDateTime")) ? null : reader.GetDateTime(reader.GetOrdinal("CreatedDateTime")),
                                    UserSign = reader.IsDBNull(reader.GetOrdinal("UserSign")) ? null : reader.GetString(reader.GetOrdinal("UserSign")),
                                    IsAllocated = reader.IsDBNull(reader.GetOrdinal("IsAllocated")) ? null : reader.GetString(reader.GetOrdinal("IsAllocated"))
                                };

                                inwardList.Add(inwardRequest);
                            }
                        }
                    }
                }

                return inwardList;
            }
            catch (Exception ex)
            {
                var logEntry = new ExceptionLog
                {
                    LogLevel = "Error",
                    MethodName = nameof(GetAllInwardAsync),
                    ModuleID = null,
                    ExceptionMessage = ex.Message,
                    Parameters = $"UserSign={userSign}",
                    StackTrace = ex.StackTrace ?? "",
                    EventTimestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")
                };
                _databaseConfig.InsertExceptionLogSP(logEntry);
                throw;
            }
        }

        public async Task<bool> UpdateInwardAsync(List<InwardChildRequest> inwardList, string userSign)
        {
            try
            {
                using (var conn = _databaseConfig.GetConnection())
                {
                    await conn.OpenAsync();

                    foreach (var inward in inwardList)
                    {
                        using (var cmd = new SqlCommand("TEC_UPDATE_ALLOCATED_QTY", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@DocEntry", inward.BaseEntry);
                            cmd.Parameters.AddWithValue("@ItemCode", inward.ItemCode);
                            cmd.Parameters.AddWithValue("@UserSign", userSign);

                            int rowsAffected = await cmd.ExecuteNonQueryAsync();

                            // Optional: If the stored procedure is expected to always affect at least one row
                            if (rowsAffected == 0)
                                return false;
                        }
                    }
                }

                return true; // all updates succeeded
            }
            catch (Exception ex)
            {
                var logEntry = new ExceptionLog
                {
                    LogLevel = "Error",
                    MethodName = nameof(UpdateInwardAsync),
                    ModuleID = null,
                    ExceptionMessage = ex.Message,
                    Parameters = $"UserSign={userSign}, InwardCount={inwardList?.Count ?? 0}",
                    StackTrace = ex.StackTrace ?? "",
                    EventTimestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")
                };
                _databaseConfig.InsertExceptionLogSP(logEntry);
                return false;
            }
        }

        public async Task<bool> CreateInwardChildAsync(List<InwardChildRequest> inwards)
        {
            try
            {
                using (var conn = _databaseConfig.GetConnection())
                {
                    await conn.OpenAsync();

                    foreach (var inward in inwards)
                    {
                        using (var cmd = new SqlCommand("TEC_ALLOCATE_QTY", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@BaseEntry", inward.BaseEntry);
                            cmd.Parameters.AddWithValue("@ItemCode", inward.ItemCode);
                            cmd.Parameters.AddWithValue("@ItemName", inward.ItemName);
                            cmd.Parameters.AddWithValue("@Qty", inward.Quantity);

                            var insertedRowsParam = new SqlParameter("@InsertedRows", SqlDbType.Int)
                            {
                                Direction = ParameterDirection.Output
                            };
                            cmd.Parameters.Add(insertedRowsParam);

                            await cmd.ExecuteNonQueryAsync();

                            int insertedRows = (int)insertedRowsParam.Value;
                            if (insertedRows == 0)
                            {
                                return false; // Early exit if no rows were inserted
                            }
                        }
                    }

                    return true; // All inwards processed successfully
                }
            }
            catch (Exception ex)
            {
                var logEntry = new ExceptionLog
                {
                    LogLevel = "Error",
                    MethodName = nameof(CreateInwardChildAsync),
                    ModuleID = null,
                    ExceptionMessage = ex.Message,
                    Parameters = string.Join(",", inwards.Select(i => $"{i.BaseEntry}-{i.ItemCode}")),
                    StackTrace = string.Empty,
                    EventTimestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")
                };
                _databaseConfig.InsertExceptionLogSP(logEntry);
                throw;
            }
        }



    }
}

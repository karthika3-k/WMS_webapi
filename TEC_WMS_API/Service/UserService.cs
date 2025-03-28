using System.Data.SqlClient;
using TEC_WMS_API.Data;
using TEC_WMS_API.Interface;
using TEC_WMS_API.Models.RequestModel;


namespace TEC_WMS_API.Service
{
    public interface IUserService
    {
        Task<int> CreateUserAsync(LoginRequest login);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<LoginRequest>> GetAllUserAsync();
        Task<LoginRequest?> GetUserByIdAsync(int id);
        Task<bool> UpdateUserAsync(int id,LoginRequest login);
    }
    public class UserService : IUserService
    {
        //private readonly IUserRepository _repository;
        private readonly DatabaseConfig _databaseConfig;

        public UserService(DatabaseConfig databaseConfig)
        {
            // _repository = repository;
            _databaseConfig = databaseConfig;
        }
        string Squery = string.Empty;
        public async Task<int> CreateUserAsync(LoginRequest login)
        {
            try
            {
                using (var conn = _databaseConfig.GetConnection())
                {
                    conn.Open();
                    string query = "INSERT INTO OUSR (UserName, Password, Warehouse, Role, DeviceId, CreatedBy, CreatedOn, IsActive, UpdatedBy, UpdatedOn, IsDeleted) " +
                                   "VALUES (@UserName, @Password, @Warehouse, @Role, @DeviceId, @CreatedBy, @CreatedOn, @IsActive, @UpdatedBy, @UpdatedOn, @IsDeleted); " +
                                   "SELECT SCOPE_IDENTITY();";

                    var cmd = new SqlCommand(query, conn);

                    login.CreatedOn = DateTime.Now;

                    cmd.Parameters.AddWithValue("@UserName", login.UserName);
                    cmd.Parameters.AddWithValue("@Password", HashPassword(login.Password));
                    cmd.Parameters.AddWithValue("@Warehouse", login.WareHouse);
                    cmd.Parameters.AddWithValue("@Role", login.Role);
                    cmd.Parameters.AddWithValue("@DeviceId", login.DeviceId);
                    cmd.Parameters.AddWithValue("@CreatedBy", login.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedOn", login.CreatedOn);
                    cmd.Parameters.AddWithValue("@IsActive", login.IsActive);
                    cmd.Parameters.AddWithValue("@UpdatedBy", DBNull.Value);
                    cmd.Parameters.AddWithValue("@UpdatedOn", DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsDeleted", false);

                    var result = await cmd.ExecuteNonQueryAsync();
                    conn.Close();
                    return Convert.ToInt32(result);

                }
            }
            catch (Exception ex)
            {
                var logEntry = new ExceptionLog
                {
                    LogLevel = "Error",
                    MethodName = nameof(CreateUserAsync),
                    ModuleID = null,
                    ExceptionMessage = ex.Message,
                    Parameters = $"{login}",
                    StackTrace = string.Empty,
                    EventTimestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")
                };
                _databaseConfig.InsertExceptionLogSP(logEntry);
                throw;
            }
           
        }
        private string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                using (var conn = _databaseConfig.GetConnection())
                {
                    Squery = "DELETE FROM OUSR WHERE ID = @id;";
                    var cmd = new SqlCommand(Squery, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    await conn.OpenAsync();
                    var rowsAffected = await cmd.ExecuteNonQueryAsync();
                    conn.CloseAsync();
                    return rowsAffected > 0;


                }
            }
            catch (Exception ex)
            {
                var logEntry = new ExceptionLog
                {
                    LogLevel = "Error",
                    MethodName = nameof(DeleteUserAsync),
                    ModuleID = null,
                    ExceptionMessage = ex.Message,
                    Parameters = $"{id}",
                    StackTrace = string.Empty,
                    EventTimestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")
                };
                _databaseConfig.InsertExceptionLogSP(logEntry);
                throw;
            }
            
        }

        public async Task<IEnumerable<LoginRequest>> GetAllUserAsync()
        {
            try
            {
                var users = new List<LoginRequest>();
                using (var conn = _databaseConfig.GetConnection())
                {
                    Squery = "SELECT * FROM OUSR";
                    var cmd = new SqlCommand(Squery, conn);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        for (int i = 0; await reader.ReadAsync(); i++)
                        {
                            var loginRequest = new LoginRequest
                            {
                                UserId = reader.GetInt32(0),
                                UserName = reader.GetString(1),
                                Password = reader.GetString(2),
                                WareHouse = reader.GetString(3),
                                Role = reader.GetString(4),
                                DeviceId = reader.GetString(5),
                                IsActive = reader.GetBoolean(8)
                            };

                            users.Add(loginRequest);
                        }
                    }

                    return users;
                }
            }
            catch (Exception ex)
            {
                var logEntry = new ExceptionLog
                {
                    LogLevel = "Error",
                    MethodName = nameof(GetAllUserAsync),
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

        public async Task<LoginRequest?> GetUserByIdAsync(int id)
        {
            try
            {
                LoginRequest? loginRequest = null;
                using (var conn = _databaseConfig.GetConnection())
                {
                    Squery = "SELECT * FROM OUSR WHERE ID = @id";
                    var cmd = new SqlCommand(Squery, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            loginRequest = new LoginRequest
                            {
                                UserId = reader.GetInt32(0),
                                UserName = reader.GetString(1),
                                Password = reader.GetString(2),
                                WareHouse = reader.GetString(3),
                                Role = reader.GetString(4),
                                DeviceId = reader.GetString(5),
                                CreatedBy = reader.GetString(6)
                            };
                        }
                    }
                }
                return loginRequest;
            }
            catch (Exception ex)
            {
                var logEntry = new ExceptionLog
                {
                    LogLevel = "Error",
                    MethodName = nameof(GetUserByIdAsync),
                    ModuleID = null,
                    ExceptionMessage = ex.Message,
                    Parameters = $"{id}",
                    StackTrace = string.Empty,
                    EventTimestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")
                };
                _databaseConfig.InsertExceptionLogSP(logEntry);
                throw;
            }
            
        }

        public async Task<bool> UpdateUserAsync(int id,LoginRequest login)
        {
            try
            {
                using (var conn = _databaseConfig.GetConnection())
                {
                    Squery = "UPDATE OUSR SET UserName = @UserName, Password = @Password, Warehouse = @Warehouse, Role = @Role, DeviceId = @DeviceId, " +
                        "IsActive = @IsActive, UpdatedBy = @UpdatedBy, UpdatedOn = @UpdatedOn WHERE ID = @ID;";
                    var cmd = new SqlCommand(Squery, conn);
                    conn.Open();
                    login.UpdatedOn = DateTime.Now;
                    cmd.Parameters.AddWithValue("@ID", id);
                    cmd.Parameters.AddWithValue("@UserName", login.UserName);
                    cmd.Parameters.AddWithValue("@Password", HashPassword(login.Password));
                    cmd.Parameters.AddWithValue("@Warehouse", login.WareHouse);
                    cmd.Parameters.AddWithValue("@Role", login.Role);
                    cmd.Parameters.AddWithValue("@DeviceId", login.DeviceId);
                    cmd.Parameters.AddWithValue("@IsActive", login.IsActive);
                    cmd.Parameters.AddWithValue("@UpdatedBy", login.UpdatedBy);
                    cmd.Parameters.AddWithValue("@UpdatedOn", login.UpdatedOn);

                    var rowsAffected = await cmd.ExecuteNonQueryAsync();
                    conn.Close();
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                var logEntry = new ExceptionLog
                {
                    LogLevel = "Error",
                    MethodName = nameof(UpdateUserAsync),
                    ModuleID = null,
                    ExceptionMessage = ex.Message,
                    Parameters = $"{id},{login}",
                    StackTrace = string.Empty,
                    EventTimestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")
                };
                _databaseConfig.InsertExceptionLogSP(logEntry);
                throw;
            }
            
        }

    }

}

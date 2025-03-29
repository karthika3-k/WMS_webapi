using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TEC_WMS_API.Data;
using TEC_WMS_API.Interface;
using TEC_WMS_API.Models.RequestModel;
using TEC_WMS_API.Models.ResponseModel;


namespace TEC_WMS_API.Service
{
    public interface ILoginService
    {
        Task<LoginRequest?> GetByIdAsync(string UserName, string Password);
        Task<bool> UpdateUserPwdAsync(int id, string userName, string password);
    }

    public class LoginService : ILoginService
    {
        //private readonly ILoginRepository _repository;
        private readonly DatabaseConfig _databaseConfig;
        private readonly IConfiguration _configuration;

        public LoginService(DatabaseConfig databaseConfig, IConfiguration configuration)
        {
            // _repository = repository;
            _databaseConfig = databaseConfig;
            _configuration = configuration;
        }
        string sQuery = string.Empty;
        public async Task<LoginRequest?> GetByIdAsync(string UserName, string Password)
        {
            try
            {
                using (var conn = _databaseConfig.GetConnection())
                {
                    sQuery = "EXEC TEC_WMS001_LoginUser @UserName, @Password;";
                    var cmd = new SqlCommand(sQuery, conn);
                    cmd.Parameters.AddWithValue("@UserName", UserName);
                    cmd.Parameters.AddWithValue("@Password", HashPassword(Password));

                    await conn.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            LoginRequest login = new LoginRequest
                            {
                                UserId = reader.IsDBNull(reader.GetOrdinal("ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("ID")),
                                UserName = reader.IsDBNull(reader.GetOrdinal("UserName")) ? string.Empty : reader.GetString(reader.GetOrdinal("UserName")),
                                Password = reader.IsDBNull(reader.GetOrdinal("Password")) ? string.Empty : reader.GetString(reader.GetOrdinal("Password")),
                                WareHouse = reader.IsDBNull(reader.GetOrdinal("Warehouse")) ? string.Empty : reader.GetString(reader.GetOrdinal("Warehouse")),
                                Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? string.Empty : reader.GetString(reader.GetOrdinal("Role")),
                                DeviceId = reader.IsDBNull(reader.GetOrdinal("DeviceId")) ? string.Empty : reader.GetString(reader.GetOrdinal("DeviceId")),
                                IsActive = reader.IsDBNull(reader.GetOrdinal("IsActive")) ? false : reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                IsDeleted = reader.IsDBNull(reader.GetOrdinal("IsDeleted")) ? false : reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
                            };

                            if (VerifyPassword(HashPassword(Password), login.Password))
                            {

                                var claims = new[]
                                {
                            new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                            new Claim("ID", login.UserId.ToString()),
                            new Claim("UserName", login.UserName),
                        };
                                var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                                var signIn = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
                                var token = new JwtSecurityToken(
                                    _configuration["Jwt:Issuer"],
                                    _configuration["Jwt:Audience"],
                                    claims,
                                    expires: DateTime.UtcNow.AddMinutes(60),
                                    signingCredentials: signIn
                                );
                                string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
                                return new LoginResponse { AccessToken = tokenValue, UserName = login.UserName, Role=login.Role, UserId=login.UserId };
                                //return login;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var logEntry = new ExceptionLog
                {
                    LogLevel = "Error",
                    MethodName = nameof(GetByIdAsync),
                    ModuleID = null,
                    ExceptionMessage = ex.Message,
                    Parameters = $"{UserName},{Password}",
                    StackTrace = string.Empty,
                    EventTimestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")
                };
                _databaseConfig.InsertExceptionLogSP(logEntry);
                throw;
            }
           

            return null;
        }

        private bool VerifyPassword(string enteredPassWord, string Password)
        {
            return enteredPassWord == Password;
        }

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public async Task<bool> UpdateUserPwdAsync(int id, string userName, string password)
        {
            try
            {
                using (var conn = _databaseConfig.GetConnection())
                {
                    string Squery = "UPDATE OUSR SET UserName = @UserName, Password = @Password, UpdatedBy = @UpdatedBy, UpdatedOn = @UpdatedOn WHERE ID = @ID;";
                    var cmd = new SqlCommand(Squery, conn);
                    conn.Open();

                    LoginRequest login = new LoginRequest();
                    login.UpdatedOn = DateTime.Now;

                    cmd.Parameters.AddWithValue("@ID", id);               // ID
                    cmd.Parameters.AddWithValue("@UserName", userName);    // UserName
                    cmd.Parameters.AddWithValue("@Password", HashPassword(password));  // Password (hashed)
                    cmd.Parameters.AddWithValue("@UpdatedBy", userName);   // UpdatedBy (who is making the change)
                    cmd.Parameters.AddWithValue("@UpdatedOn", login.UpdatedOn); // UpdatedOn (time of update)

                    var rowsAffected = await cmd.ExecuteNonQueryAsync();

                    conn.Close();

                    return rowsAffected > 0; // Return true if update is successful.
                }
            }
            catch (Exception ex)
            {
                var logEntry = new ExceptionLog
                {
                    LogLevel = "Error",
                    MethodName = nameof(UpdateUserPwdAsync),
                    ModuleID = null,
                    ExceptionMessage = ex.Message,
                    Parameters = $"{id},{userName},{password}",
                    StackTrace = string.Empty,
                    EventTimestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt")
                };
                _databaseConfig.InsertExceptionLogSP(logEntry);
                throw;
            }
            
        }

    }
}

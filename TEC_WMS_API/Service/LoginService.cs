using System.Data.SqlClient;
using TEC_WMS_API.Data;
using TEC_WMS_API.Models.RequestModel;


namespace TEC_WMS_API.Service
{
    public interface ILoginService
    {
        Task<LoginRequest?> GetByIdAsync(string UserName, string Password);
    }

    public class LoginService : ILoginService
    {
        //private readonly ILoginRepository _repository;
        private readonly DatabaseConfig _databaseConfig;

        public LoginService(DatabaseConfig databaseConfig)
        {
            // _repository = repository;
            _databaseConfig = databaseConfig;
        }
        string sQuery = string.Empty;
        public async Task<LoginRequest?> GetByIdAsync(string UserName, string Password)
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
                            return login;
                        }

                    }
                }
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
    }
}

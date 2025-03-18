using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TEC_WMS_API.Data;
using TEC_WMS_API.Models.RequestModel;
using TEC_WMS_API.Models.ResponseModel;


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
                            return new LoginResponse { Token = tokenValue, Login = login };
                            //return login;
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

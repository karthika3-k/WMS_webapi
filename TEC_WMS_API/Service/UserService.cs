using System.Data.SqlClient;
using TEC_WMS_API.Data;
using TEC_WMS_API.Models.RequestModel;


namespace TEC_WMS_API.Service
{
    public interface IUserService
    {
        Task<int> CreateUserAsync(LoginRequest login);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<LoginRequest>> GetAllUserAsync();
        Task<LoginRequest?> GetUserByIdAsync(int id);
        Task<bool> UpdateUserAsync(LoginRequest login);
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
                cmd.Parameters.AddWithValue("@UpdatedBy", "");
                cmd.Parameters.AddWithValue("@UpdatedOn", "");
                cmd.Parameters.AddWithValue("@IsDeleted", false);

                var result = await cmd.ExecuteScalarAsync();
                conn.Close();
                return Convert.ToInt32(result);

            }
        }
        private string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            using (var conn = _databaseConfig.GetConnection())
            {
                Squery = "DELETE FROM OUSR WHERE ID = @id;";
                var cmd = new SqlCommand(Squery, conn);
                cmd.Parameters.AddWithValue("@id", id);
                await conn.OpenAsync();
                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;

            }
        }

        public async Task<IEnumerable<LoginRequest>> GetAllUserAsync()
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
                            CreatedBy = reader.GetString(6)
                        };

                        users.Add(loginRequest);
                    }
                }

                return users;
            }
        }

        public async Task<LoginRequest?> GetUserByIdAsync(int id)
        {
            LoginRequest? loginRequest = null;
            using (var conn = _databaseConfig.GetConnection())
            {
                string Squery = "SELECT * FROM OUSR WHERE ID = @id";
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

        public async Task<bool> UpdateUserAsync(LoginRequest login)
        {
            using (var conn = _databaseConfig.GetConnection())
            {
                Squery = "UPDATE OUSR SET UserName = @UserName, Password = @Password, Warehouse = @Warehouse, Role = @Role, DeviceId = @DeviceId, " +
                    "CreatedBy = @CreatedBy, CreatedOn = @CreatedOn, IsActive = @IsActive, UpdatedBy = @UpdatedBy, UpdatedOn = @UpdatedOn, " +
                    "IsDeleted = @IsDeleted  WHERE ID = @ID;";
                var cmd = new SqlCommand(Squery, conn);
                conn.Open();
                login.UpdatedOn = DateTime.Now;
                cmd.Parameters.AddWithValue("@UserName", login.UserName);
                cmd.Parameters.AddWithValue("@Password", login.Password);
                cmd.Parameters.AddWithValue("@Warehouse", login.WareHouse);
                cmd.Parameters.AddWithValue("@Role", login.Role);
                cmd.Parameters.AddWithValue("@DeviceId", login.DeviceId);
                cmd.Parameters.AddWithValue("@CreatedBy", "");
                cmd.Parameters.AddWithValue("@CreatedOn", "");
                cmd.Parameters.AddWithValue("@IsActive", login.IsActive);
                cmd.Parameters.AddWithValue("@UpdatedBy", login.UpdatedBy);
                cmd.Parameters.AddWithValue("@UpdatedOn", login.UpdatedOn);
                cmd.Parameters.AddWithValue("@IsDeleted", login.IsDeleted);

                await conn.OpenAsync();
                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                conn.Close();
                return rowsAffected > 0;
            }
        }
    }
}

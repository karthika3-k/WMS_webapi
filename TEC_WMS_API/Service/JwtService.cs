using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TEC_WMS_API.Data;
using TEC_WMS_API.Handler;
using TEC_WMS_API.Models.RequestModel;
using TEC_WMS_API.Models.ResponseModel;

namespace TEC_WMS_API.Service
{
    public class JwtService
    {
        private readonly DatabaseConfig _databaseConfig;
        private readonly IConfiguration _configuration;
        public readonly LoginService _loginService;

        public JwtService(DatabaseConfig databaseConfig, IConfiguration configuration, LoginService loginService)
        {
            // _repository = repository;
            _databaseConfig = databaseConfig;
            _configuration = configuration;
            _loginService = loginService;

        }
        public async Task<LoginResponse?> Authenticate(LoginRequest request )
        {
            if (string.IsNullOrWhiteSpace(request.UserName)|| string.IsNullOrWhiteSpace(request.Password))
            {
                return null;
            }
            var userAccount = await _loginService.GetByIdAsync(request.UserName, request.Password);
            if (userAccount == null || PasswordHashHandler.VerifyPassword(request.Password, userAccount.Password))
            {
                return null;
            }
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];
            var key = _configuration["Jwt:Key"];
            var tokenValidityMins = _configuration["Jwt:TokenValidityMins"];
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(Convert.ToDouble(tokenValidityMins));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, request.UserName),
                    //new Claim(ClaimTypes.Role, request.Role),
                    //new Claim(ClaimTypes.NameIdentifier, userAccount.UserId.ToString())
                }),
                Expires = tokenExpiryTimeStamp,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);
            return new LoginResponse
            {
                AccessToken = accessToken,
                UserName = userAccount.UserName,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds
            };
        }
    }
}

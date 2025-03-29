using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Models.ResponseModel
{
    public class LoginResponse : LoginRequest
    {        
        public string? UserName { get; set; }
        public string? AccessToken { get; set; }
        public int ExpiresIn { get; set; }
    }
    
}

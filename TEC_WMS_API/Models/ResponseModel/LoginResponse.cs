using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Models.ResponseModel
{
    public class LoginResponse : LoginRequest
    {
        public string Token { get; set; }
        public LoginRequest Login { get; set; }
    }
    
}

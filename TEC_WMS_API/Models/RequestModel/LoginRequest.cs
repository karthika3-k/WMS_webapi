using System.Windows.Markup;

namespace TEC_WMS_API.Models.RequestModel
{
    public class LoginRequest
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string WareHouse { get; set; }
        public string Role { get; set; }
        public string DeviceId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? IsActive { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }        
        public bool? IsDeleted { get; set; }
    }
}

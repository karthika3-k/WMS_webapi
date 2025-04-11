namespace TEC_WMS_API.Models.RequestModel
{
    public class InwardChildRequest
    {
        public int ID { get; set; }
        public int? BaseEntry { get; set; }
        public string? ItemCode { get; set; }
        public string? ItemName { get; set; }
        public decimal? Quantity { get; set; }

 
    }
}

namespace TEC_WMS_API.Models.RequestModel
{
    public class BinMasterRequest
    {
        public int BinID { get; set; }
        public string Floor { get; set; }
        public string Rack { get; set; }
        public string Bin { get; set; }
        public string BinLocCode { get; set; }
        public string WhsCode { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public string Brand { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public int Level { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}

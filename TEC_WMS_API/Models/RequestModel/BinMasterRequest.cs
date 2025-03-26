namespace TEC_WMS_API.Models.RequestModel
{
    public class BinMasterRequest
    {
        public int BinID { get; set; }
        public string WhsCode { get; set; } 
        public string BinLocCode { get; set; }
        public string? SL1Code { get; set; }
        public string? SL2Code { get; set; }
        public string? SL3Code { get; set; }
        public string? SL4Code { get; set; }
        public string? SL5Code { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public string? Filter1 { get; set; }
        public string? Filter2 { get; set; }
        public string? Filter3 { get; set; }
        public decimal Quantity { get; set; }
        public int Level { get; set; }
        public bool? Active { get; set; }
        public string UserSign { get; set; }
    }
}

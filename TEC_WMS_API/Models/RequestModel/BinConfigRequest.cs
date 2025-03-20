namespace TEC_WMS_API.Models.RequestModel
{
    public class BinConfigRequest
    {
        public int BinConfigId { get; set; }
        public string BinCode { get; set; }
        public string BinName { get; set; }
        public string Prefix { get; set; }
        public string WhsCode { get; set; }
        public int StartNo { get; set; }
        public int EndNo { get; set; }       
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}

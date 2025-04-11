namespace TEC_WMS_API.Models.RequestModel
{
    public class InwardRequest
    {
        public int ID { get; set; } // Identity column, not nullable

        public int? DocEntry { get; set; }

        public int? DocNum { get; set; }

        public DateTime? DocDate { get; set; }

        public int? TransType { get; set; }

        public string? CardCode { get; set; }

        public string? CardName { get; set; }

        public string? ItemCode { get; set; }

        public string? ItemName { get; set; }

        public decimal? Quantity { get; set; }

        public DateTime? CreatedDateTime { get; set; }

        public string? UserSign { get; set; }

        public string  IsAllocated { get; set; }
    }
}

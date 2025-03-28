namespace TEC_WMS_API.Models.RequestModel
{
    public class ExceptionLog
    {
        public long ExceptionLogID { get; set; } // Primary Key
        public string LogLevel { get; set; } // e.g., "Error", "Warning", "Info"
        public string MethodName { get; set; } // Name of the method where the exception occurred
        public long? ModuleID { get; set; } // Identifier for the module, nullable
        public string ExceptionMessage { get; set; } // Exception message details
        public string Parameters { get; set; } // Parameters involved in the method
        public string EventTimestamp { get; set; } // UTC timestamp of the event
        public string StackTrace { get; set; } // Stack trace information
        public DateTime CreateDateUTC { get; set; } // Record creation date in UTC

    }
}

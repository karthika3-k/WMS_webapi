using TEC_WMS_API.Data;
using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Services
{
    public class LogServices
    {
        private readonly object _requsetContext;

        public async Task<ExceptionLog> ExceptionLog(DatabaseConfig db, long moduleId = 1, string methodName = "", string parameters = "", string exMessage = "", Exception ex = null)
        {
            var errorLogEntry = new ExceptionLog
            {
                LogLevel = "Error",
                ModuleID = moduleId,
                MethodName = methodName,
                Parameters = parameters,
                ExceptionMessage = exMessage,
                StackTrace = ex?.StackTrace,
                EventTimestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt"),
                CreateDateUTC = DateTime.Now
                // Initialize other properties as needed
            };

            // Call the stored procedure to insert the log entry
            await Task.Run(() => db.InsertExceptionLogSP(errorLogEntry));

            return errorLogEntry;
        }
    }
}

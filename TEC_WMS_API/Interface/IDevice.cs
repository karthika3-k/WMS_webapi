using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Interface
{
    public interface IDevice
    {
        Task<int> CreateDeviceAsync(DeviceRequest device);
    }
}

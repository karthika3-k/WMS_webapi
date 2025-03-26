using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Interface
{
    public interface IDevice
    {
        Task<int> CreateDeviceAsync(UpdateDeviceRequest device);
        Task<DeviceRequest> GetDeviceByIdAsync(int id);
        Task<bool> UpdateDeviceAsync(UpdateDeviceRequest device);
        Task<List<DeviceRequest>?> GetAllDeviceAsync();
        Task<bool> DeleteDeviceAsync(int id);
        Task<IEnumerable<DevicedropdownRequest>> GetAllDevicedropdownAsync();

    }
}

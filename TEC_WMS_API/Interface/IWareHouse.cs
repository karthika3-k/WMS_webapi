using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Interface
{
    public interface IWareHouse
    {
        Task<IEnumerable<WareHouseRequest>> GetAllWareHouseAsync();
    }
}

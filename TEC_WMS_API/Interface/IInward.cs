using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Interface
{
    public interface IInward
    {
        Task<IEnumerable<InwardRequest>> GetAllInwardAsync(string userSign);
        Task<bool> CreateInwardChildAsync(List<InwardChildRequest> inwards);

        Task<bool> UpdateInwardAsync(List<InwardChildRequest> inwardList, string userSign);
    }
}

using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Interface
{
    public interface IBinConfig
    {
        Task<int> CreateBinConfigsAsync(IEnumerable<BinConfigRequest> binConfigs);
        Task<bool> DeleteBinConfigAsync(int id);
        Task<IEnumerable<BinConfigRequest>> GetAllBinConfigAsync();
        Task<BinConfigRequest?> GetBinConfigByIdAsync(int id);
        Task<bool> UpdateBinConfigAsync(int id,BinConfigRequest binConfig);       
    }
}

using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Interface
{
    public interface IBinConfig
    {
        Task<int> CreateBinConfigsAsync(IEnumerable<BinConfigRequest> binConfigs);
        Task<bool> DeleteBinConfigAsync(string whsCode);
        Task<IEnumerable<BinConfigRequest>> GetAllBinConfigAsync();
        Task<IEnumerable<BinConfigRequest>> GetAllBinListbywhsConfigAsync(string whsCode);
        Task<BinConfigRequest?> GetBinConfigByIdAsync(int id);
        Task<bool> UpdateBinConfigAsync(IEnumerable<BinConfigRequest> binConfigs);
    }
}

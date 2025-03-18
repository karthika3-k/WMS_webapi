using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Interface
{
    public interface IBinConfig
    {
        Task<int> CreateBinConfigAsync(BinConfigRequest binConfig);
        Task<bool> DeleteBinConfigAsync(int id);
        Task<IEnumerable<BinConfigRequest>> GetAllBinConfigAsync();
        Task<BinConfigRequest?> GetBinConfigByIdAsync(int id);
        Task<bool> UpdateBinConfigAsync(int id,BinConfigRequest binConfig);       
    }
}

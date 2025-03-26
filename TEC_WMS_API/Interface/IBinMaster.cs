using TEC_WMS_API.Models.RequestModel;

namespace TEC_WMS_API.Interface
{
    public interface IBinMaster
    {
        Task<int> CreateBinMasterAsync(List<BinMasterRequest> binMasters);
        Task<BinMasterRequest?> GetBinMasterByIdAsync(int id);
        Task<IEnumerable<BinMasterRequest>> GetBinMasterByWhsAsync(string whsCode);
        Task<IEnumerable<BinMasterRequest>> GetAllMasterConfigAsync();
        Task<bool> UpdateBinMasterAsync(int binId, BinMasterRequest binMaster);
        Task<bool> DeleteBinMasterAsync(int id);
    }
}

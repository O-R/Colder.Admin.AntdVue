using Coldairarrow.Entity.OrderAssistant;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.OrderAssistant
{
    public interface ISkuBusiness
    {
        Task<PageResult<Sku>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<Sku> GetTheDataAsync(string id);
        Task<Sku> GetSkuWithPriceAsync(string id);
        Task AddDataAsync(Sku data);
        Task UpdateDataAsync(Sku data);
        Task DeleteDataAsync(List<string> ids);


    }
}
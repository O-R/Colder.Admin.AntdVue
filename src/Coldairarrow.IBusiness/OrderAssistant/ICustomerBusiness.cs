using Coldairarrow.Entity.OrderAssistant;
using Coldairarrow.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.OrderAssistant
{
    public interface ICustomerBusiness
    {
        Task<PageResult<Customer>> GetDataListAsync(PageInput<ConditionDTO> input);
        Task<Customer> GetTheDataAsync(string id);
        Task AddDataAsync(Customer data);
        Task UpdateDataAsync(Customer data);
        Task DeleteDataAsync(List<string> ids);
    }
}
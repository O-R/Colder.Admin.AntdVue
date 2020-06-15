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

        Task<List<CustomerSku>> GetCustomerSkuList(string customerId);
    }

    public class CustomerSkuDTO
    {
        public string CustomerId { get; set; }

        /// <summary>
        /// sku id
        /// </summary>
        public string Id { get; set; }

        public string SkuNo { get; set; }

        public string SkuName { get; set; }

        public decimal Price { get; set; }
    }

    public class CustomerSkuInputDTO
    {
        public string CustomerId { get; set; }
    }
}
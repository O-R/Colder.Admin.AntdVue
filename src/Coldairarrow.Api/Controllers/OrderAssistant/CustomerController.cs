using Coldairarrow.Business.OrderAssistant;
using Coldairarrow.Entity.OrderAssistant;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Api.Controllers.OrderAssistant
{
    [Route("/OrderAssistant/[controller]/[action]")]
    public class CustomerController : BaseApiController
    {
        #region DI

        public CustomerController(ICustomerBusiness customerBus)
        {
            _customerBus = customerBus;
        }

        ICustomerBusiness _customerBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<Customer>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _customerBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<Customer> GetTheData(IdInputDTO input)
        {
            return await _customerBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(Customer data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _customerBus.AddDataAsync(data);
            }
            else
            {
                await _customerBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _customerBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}
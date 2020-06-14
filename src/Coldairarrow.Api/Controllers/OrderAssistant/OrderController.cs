using Coldairarrow.Business.OrderAssistant;
using Coldairarrow.Entity.OrderAssistant;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Api.Controllers.OrderAssistant
{
    [Route("/OrderAssistant/[controller]/[action]")]
    public class OrderController : BaseApiController
    {
        #region DI

        public OrderController(IOrderBusiness orderBus)
        {
            _orderBus = orderBus;
        }

        IOrderBusiness _orderBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<Order>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _orderBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<Order> GetTheData(IdInputDTO input)
        {
            return await _orderBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(Order data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _orderBus.AddDataAsync(data);
            }
            else
            {
                await _orderBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _orderBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}
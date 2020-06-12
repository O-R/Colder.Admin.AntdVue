using Coldairarrow.Business.OrderAssistant;
using Coldairarrow.Entity.OrderAssistant;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Api.Controllers.OrderAssistant
{
    [Route("/OrderAssistant/[controller]/[action]")]
    public class SkuController : BaseApiController
    {
        #region DI

        public SkuController(ISkuBusiness skuBus)
        {
            _skuBus = skuBus;
        }

        ISkuBusiness _skuBus { get; }

        #endregion

        #region 获取

        [HttpPost]
        public async Task<PageResult<Sku>> GetDataList(PageInput<ConditionDTO> input)
        {
            return await _skuBus.GetDataListAsync(input);
        }

        [HttpPost]
        public async Task<Sku> GetTheData(IdInputDTO input)
        {
            return await _skuBus.GetTheDataAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(Sku data)
        {
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                await _skuBus.AddDataAsync(data);
            }
            else
            {
                await _skuBus.UpdateDataAsync(data);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _skuBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}
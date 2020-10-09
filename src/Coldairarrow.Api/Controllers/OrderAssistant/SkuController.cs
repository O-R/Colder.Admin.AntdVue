using Coldairarrow.Business.OrderAssistant;
using Coldairarrow.Entity.OrderAssistant;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
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
            return await _skuBus.GetSkuWithPriceAsync(input.id);
        }

        #endregion

        #region 提交

        [HttpPost]
        public async Task SaveData(Sku data)
        {
            if (data?.SkuCustomers != null)
            {
                if (data.SkuCustomers.GroupBy(sc => sc.CustomerId).Any(gp => gp.Count() > 1))
                {
                    throw new BusException("存在同一客户配置了多条价格信息");
                }
            }
            if (data.Id.IsNullOrEmpty())
            {
                InitEntity(data);

                data.SkuCustomers.ForEach(sc =>
                {
                    InitEntity(sc);
                    sc.SkuId = data.Id;
                });

                await _skuBus.AddDataAsync(data);
            }
            else
            {
                var dbCustomerSkus = (await _skuBus.GetSkuWithPriceAsync(data.Id)).SkuCustomers;

                var addCustomerSkus = new List<CustomerSku>();
                var deleteCustomerSkus = new List<CustomerSku>();
                var updataCustomerSkus = new List<CustomerSku>();
                data.SkuCustomers.ForEach(sc =>
                {
                    if (sc.Id.IsNullOrEmpty())
                    {
                        InitEntity(sc);
                        sc.SkuId = data.Id;
                        addCustomerSkus.Add(sc);
                    }
                    else
                    {
                        updataCustomerSkus.Add(sc);
                    }
                });

                deleteCustomerSkus.AddRange(
                    dbCustomerSkus.Where(ck => !data.SkuCustomers.Any(d => d.Id == ck.Id)));

                await _skuBus.SaveDataChangeAsync(data, addCustomerSkus, updataCustomerSkus, deleteCustomerSkus);
            }
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _skuBus.DeleteDataAsync(ids);
        }

        [HttpPost]
        public async Task Import(List<Sku> list)
        {
            if (list == null || !list.Any())
            {
                throw new BusException("导入数据为空");
            }
            list.ForEach(it =>
            {
                InitEntity(it);
                it.KeyWords = string.Join(',', it.KeywordList.Distinct());
            });
            await _skuBus.AddDataListAsync(list);
        }

        #endregion
    }
}
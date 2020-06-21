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


        [HttpPost]
        public async Task<OrderParseDTO> GetTheItem(IdInputDTO input)
        {
            var item = await _orderBus.GetTheItemAsync(input.id);
            if (item == null)
            {
                throw new BusException("查询不到订单数据");
            }
            return new OrderParseDTO
            {
                Id = item.Id,
                Address = item.Order.Address,
                Area = item.Order.Area,
                AreaNo = item.Order.AreaNo,
                City = item.Order.City,
                CityNo = item.Order.CityNo,
                Count = item.Count,
                CreateTime = item.Order.CreateTime,
                CustomerName = item.Order.CustomerName,
                CustomerNo = item.Order.CustomerNo,
                Discount = item.Order.Discount,
                OrderNo = item.Order.OrderNo,
                Price = item.Price,
                Province = item.Order.Province,
                ProvinceNo = item.Order.ProvinceNo,
                Receiver = item.Order.Receiver,
                ReceiverPhone = item.Order.ReceiverPhone,
                SkuName = item.SkuName,
                SkuNo = item.SkuNo,
                SkuId = item.SkuId,
                OrderId = item.Order.Id,
                CustomerId = item.Order.CustomerId,
                TotalPrice = item.Order.TotalPrice
            };
        }

        [HttpPost]
        public async Task<PageResult<OrderParseDTO>> GetItemList(PageInput<ConditionDTO> input)
        {
            var page = await _orderBus.GetOrderItemListAsync(input);

            var returnPage = new PageResult<OrderParseDTO>()
            {
                ErrorCode = page.ErrorCode,
                Msg = page.Msg,
                Success = page.Success,
                Total = page.Total
            };
            returnPage.Data = page?.Data?.Select(item => new OrderParseDTO
            {
                Id = item.Id,
                Address = item.Order.Address,
                Area = item.Order.Area,
                AreaNo = item.Order.AreaNo,
                City = item.Order.City,
                CityNo = item.Order.CityNo,
                Count = item.Count,
                CreateTime = item.Order.CreateTime,
                CustomerName = item.Order.CustomerName,
                CustomerNo = item.Order.CustomerNo,
                Discount = item.Order.Discount,
                OrderNo = item.Order.OrderNo,
                Price = item.Price,
                Province = item.Order.Province,
                ProvinceNo = item.Order.ProvinceNo,
                Receiver = item.Order.Receiver,
                ReceiverPhone = item.Order.ReceiverPhone,
                SkuName = item.SkuName,
                SkuNo = item.SkuNo,
                SkuId = item.SkuId,
                OrderId = item.Order.Id,
                CustomerId = item.Order.CustomerId,
                TotalPrice = item.Order.TotalPrice
            }).ToList() ?? new List<OrderParseDTO>();

            return returnPage;

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
        public async Task SaveItem(OrderParseDTO data)
        {
            var orderItem = new OrderItem()
            {
                Id = data.Id,
                OrderId = data.OrderId,
                Order = new Order()
                {
                    Province = data.Province,
                    City = data.City,
                    Area = data.Area,
                    Address = data.Address,
                    Receiver = data.Receiver,
                    ReceiverPhone = data.ReceiverPhone,
                },
                SkuId = data.SkuId,
                SkuNo = data.SkuNo,
                SkuName = data.SkuName,
                Price = data.Price,
                Count = data.Count
            };

            await _orderBus.UpdateItemAsync(orderItem);
        }

        [HttpPost]
        public async Task DeleteData(List<string> ids)
        {
            await _orderBus.DeleteDataAsync(ids);
        }

        #endregion
    }
}
using Coldairarrow.Business.OrderAssistant;
using Coldairarrow.Entity.OrderAssistant;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coldairarrow.Api.Controllers.OrderAssistant
{
    [Route("/OrderAssistant/Order/[action]")]
    public class OrderParseController : BaseApiController
    {
        #region DI

        public OrderParseController(IOrderBusiness orderBus, ICustomerBusiness customerBus)
        {
            _orderBus = orderBus;
            _customerBus = customerBus;
        }

        IOrderBusiness _orderBus { get; }
        ICustomerBusiness _customerBus { get; }

        #endregion

        #region 提交

        [HttpPost]
        public async Task<List<OrderParseDTO>> Parse(OrderParseInputDTO data)
        {
            var orders = new List<Order>();
            if (string.IsNullOrEmpty(data.CustomerId))
            {
                throw new BusException("请选择客户");
            }

            if (data.Orders.Count <= 0)
            {
                throw new BusException("订单解析失败，请重试");
            }

            var customer = await _customerBus.GetTheDataAsync(data.CustomerId);

            if (customer == null)
            {
                throw new BusException("客户不存在");
            }


            var customerSkuList = (await _customerBus.GetCustomerSkuList(data.CustomerId));

            var now = DateTime.Now;
            data.Orders.OrderBy(order => order.index).ForEach(item =>
            {
                var id = IdHelper.GetId();
                var orderItems = new List<OrderItem>();

                var skuKeys = item.skuKeyWords.Split(new char[] { '+', '➕' }, StringSplitOptions.RemoveEmptyEntries).Select(sk => sk.Trim()).ToList();

                skuKeys.GroupBy(key => key).ForEach(group =>
                  {
                      var key = group.Key;

                      var result = customerSkuList.Where(cs =>
                            cs.Sku.KeyworkList.Any(kw => kw.Contains(key))
                        ).Select(cs =>
                        {
                            var score = 0;
                            var seed = 10000;
                            var keywords = cs.Sku.KeyworkList.Where(kw => kw.Contains(key)).ToList();

                            // 百分百命中 权重 10000
                            // 命中部分字符串 权重= key.length * 10000 / keyword.length * keyword.count
                            foreach (var kw in cs.Sku.KeyworkList)
                            {
                                if (kw == key)
                                {
                                    score += seed;
                                }
                                else
                                {
                                    score += (key.Length * 10000) / (kw.Length * cs.Sku.KeyworkList.Count());
                                }
                            }
                            return new
                            {
                                Score = score,
                                Sku = cs.Sku,
                                Count = group.Count(),
                                Price = cs.Price

                            };
                        }).OrderByDescending(sku => sku.Score).OrderBy(sku => sku.Sku.Id).ToList();

                      orderItems.AddRange(result.Select(rs => new OrderItem()
                      {
                          Id = IdHelper.GetId(),
                          Count = rs.Count,
                          OrderId = id,
                          Price = rs.Price,
                          SkuId = rs.Sku.Id,
                          SkuName = rs.Sku.SkuName,
                          SkuNo = rs.Sku.SkuNo
                      }));

                  });

                var order = new Order()
                {
                    Id = id,
                    OrderNo = $"WD-{now.ToString("yyMMddHHmmssfff")}{item.index}",
                    Address = item.street + item.address,
                    Area = item.county,
                    AreaNo = item.countyCode,
                    City = item.city,
                    CityNo = item.cityCode,
                    CustomerId = customer.Id,
                    CustomerName = customer.CustomerName,
                    CustomerNo = customer.CustomerNo,
                    Province = item.province,
                    ProvinceNo = item.provinceCode,
                    Receiver = item.name,
                    ReceiverPhone = item.phone,
                    TotalPrice = Math.Round(orderItems.Sum(it => it.Price * it.Count), 2, MidpointRounding.AwayFromZero),
                    OrderItems = orderItems
                };
                orders.Add(order);
            });

            //await _orderBus.AddManyDataAsync(orders);

            var result = new List<OrderParseDTO>();
            foreach (var ord in orders)
            {
                foreach (var item in ord.OrderItems)
                {
                    result.Add(new OrderParseDTO
                    {
                        Id = item.Id,
                        Address = ord.Address,
                        Area = ord.Area,
                        AreaNo = ord.AreaNo,
                        City = ord.City,
                        CityNo = ord.CityNo,
                        Count = item.Count,
                        CreateTime = now,
                        CustomerName = ord.CustomerName,
                        CustomerNo = ord.CustomerNo,
                        Discount = 0,
                        OrderNo = ord.OrderNo,
                        Price = item.Price,
                        Province = ord.Province,
                        ProvinceNo = ord.ProvinceNo,
                        Receiver = ord.Receiver,
                        ReceiverPhone = ord.ReceiverPhone,
                        SkuName = item.SkuName,
                        SkuNo = item.SkuNo,
                        SkuId = item.Id,
                        OrderId = ord.Id,
                        CustomerId = customer.Id,
                        TotalPrice = ord.TotalPrice
                    });
                }

            }
            return result;
        }

        #endregion
    }
}
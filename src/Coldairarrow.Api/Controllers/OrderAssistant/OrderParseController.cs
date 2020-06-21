using Coldairarrow.Business.OrderAssistant;
using Coldairarrow.Entity.OrderAssistant;
using Coldairarrow.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NodaTime.Extensions;
using Microsoft.Extensions.Options;
using Coldairarrow.Api.Configs;

namespace Coldairarrow.Api.Controllers.OrderAssistant
{
    [Route("/OrderAssistant/Order/[action]")]
    public class OrderParseController : BaseApiController
    {
        #region DI

        public OrderParseController(IOrderBusiness orderBus, ICustomerBusiness customerBus, IOptions<Seperator> seperatorOptions)
        {
            _orderBus = orderBus;
            _customerBus = customerBus;

            _seperator = seperatorOptions.Value;
        }

        IOrderBusiness _orderBus { get; }
        ICustomerBusiness _customerBus { get; }

        Seperator _seperator { get; }

        #endregion

        #region 提交

        /// <summary>
        /// 第二步：解析
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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

                //分割出 关键词*数量 的数组
                var wordNCountStringArray = item.skuKeyWords.Split(_seperator.SkuKeySeperatorArray, StringSplitOptions.RemoveEmptyEntries).Select(sk => sk.Trim()).ToList();

                var wordCountDic = new Dictionary<string, int>();

                wordNCountStringArray.ForEach(str =>
                {
                    //关键词  数量
                    var wordNCount = str.Split(_seperator.SkuCountSeperatorArray, StringSplitOptions.RemoveEmptyEntries).Select(it => it.Trim()).ToList();

                    var word = wordNCount[0];
                    var count = wordNCount.Count > 1 ? wordNCount[1].ToInt() : 0;
                    count = count == 0 ? 1 : count;
                    if (wordCountDic.ContainsKey(word))
                    {
                        wordCountDic[word] = wordCountDic[word] + count;
                    }
                    else
                    {
                        wordCountDic.Add(word, count);
                    }

                });
                wordCountDic.ForEach(keyNcount =>
                {
                    var key = keyNcount.Key;

                    var findSku = customerSkuList.Where(cs => cs.Sku.KeywordList.Any(kw => kw.Trim() == key))
                          .OrderBy(cs => cs.SkuId).FirstOrDefault();

                    if (findSku == null)
                    {
                        orderItems.Add(new OrderItem()
                        {
                            Id = IdHelper.GetId(),
                            Count = keyNcount.Value,
                            OrderId = id,
                            Price = 0,
                            SkuId = string.Empty,
                            SkuName = item.skuKeyWords,
                            SkuNo = string.Empty,
                            IsError = true,
                            ErrorMessage = $"客户{customer.CustomerName},匹配不到关键词{key};" + item.errorMessage
                        });
                    }
                    else
                    {
                        var isError = item.isError || keyNcount.Value <= 0;
                        var errorMessage = keyNcount.Value <= 0 ? "商品数量解析异常;" : string.Empty;
                        errorMessage += item.isError ? item.errorMessage : string.Empty;
                        orderItems.Add(new OrderItem()
                        {
                            Id = IdHelper.GetId(),
                            Count = keyNcount.Value,
                            OrderId = id,
                            Price = findSku.Price,
                            SkuId = findSku.Id,
                            SkuName = findSku.Sku.SkuName,
                            SkuNo = findSku.Sku.SkuNo,

                            IsError = isError,
                            ErrorMessage = errorMessage
                        });
                    }
                });

                var order = new Order()
                {
                    Id = id,
                    OrderNo = GetOrderNum(id),
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
                        SkuId = item.SkuId,
                        OrderId = ord.Id,
                        CustomerId = customer.Id,
                        TotalPrice = ord.TotalPrice,
                        IsError = item.IsError,
                        ErrorMessage = item.ErrorMessage
                    });
                }

            }
            return result;
        }

        private static string GetOrderNum(string id)
        {
            var longId = long.Parse(id);
            var ticks = (longId >> 22) + 1288834974657L;
            
            var time = ticks.UtcToChinaTime();

            return $"WD-{time.ToString("yyMMddHHmmssfff")}{(longId & 0xfff).ToString("X3")}";
        }



        /// <summary>
        /// 第三步保存
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task SaveList(List<OrderParseDTO> list)
        {
            var addOrders = new List<Order>();
            var updateOrders = new List<Order>();
            if (list == null || !list.Any())
            {
                throw new BusException("没有可保存的数据");
            }

            //todo 客户id ，sku id合法性校验

            //var orderItemIds = list.Select(li => li.Id).ToList();
            //var existsItemIds = await _orderBus.GetExistsIdsAsync(orderItemIds);

            var op = HttpContext.RequestServices.GetService<IOperator>();

            var creatorId = op?.UserId;
            var creatorRealName = op?.Property?.RealName;
            var createTime = DateTime.Now;


            /*.Where(it => !existsItemIds.Contains(it.Id))*/
            list
            .OrderBy(it => it.OrderNo).GroupBy(it => it.OrderId).ForEach(ord =>
              {
                  var items = ord.ToList();
                  var firstItem = items.FirstOrDefault();

                  var order = new Order()
                  {
                      Id = firstItem.OrderId,
                      OrderNo = firstItem.OrderNo,
                      CreateTime = createTime,
                      CreatorId = creatorId,
                      CreatorRealName = creatorRealName,
                      CustomerId = firstItem.CustomerId,
                      CustomerName = firstItem.CustomerName,
                      CustomerNo = firstItem.CustomerNo,
                      TotalPrice = Math.Round(items.Sum(itm => itm.Price * itm.Count), 2, MidpointRounding.AwayFromZero),
                      Discount = firstItem.Discount,
                      Province = firstItem.Province,
                      ProvinceNo = firstItem.ProvinceNo,
                      City = firstItem.City,
                      CityNo = firstItem.CityNo,
                      Area = firstItem.Area,
                      AreaNo = firstItem.AreaNo,
                      Address = firstItem.Address,
                      Receiver = firstItem.Receiver,
                      ReceiverPhone = firstItem.ReceiverPhone,

                      OrderItems = items.Select(itm => new OrderItem()
                      {
                          Id = itm.Id,
                          CreateTime = createTime,
                          CreatorId = creatorId,
                          CreatorRealName = creatorRealName,
                          Count = itm.Count,
                          OrderId = firstItem.OrderId,
                          Price = itm.Price,
                          SkuId = itm.SkuId,
                          SkuName = itm.SkuName,
                          SkuNo = itm.SkuNo
                      })?.ToList() ?? new List<OrderItem>()
                  };
                  addOrders.Add(order);
              });

            if (addOrders.Any())
            {
                await _orderBus.AddManyDataAsync(addOrders);
            }
        }
        #endregion
    }
}
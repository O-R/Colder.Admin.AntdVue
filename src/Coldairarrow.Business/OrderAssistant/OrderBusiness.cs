using Coldairarrow.Entity.OrderAssistant;
using Coldairarrow.Util;
using EFCore.Sharding;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Coldairarrow.Business.OrderAssistant
{
    public class OrderBusiness : BaseBusiness<Order>, IOrderBusiness, ITransientDependency
    {
        public OrderBusiness(IRepository repository)
            : base(repository)
        {
        }

        #region 外部接口

        public async Task<PageResult<Order>> GetDataListAsync(PageInput<ConditionDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<Order>();
            var search = input.Search;

            //筛选
            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<Order, bool>(
                    ParsingConfig.Default, false, $@"{search.Condition}.Contains(@0)", search.Keyword);
                where = where.And(newWhere);
            }

            return await q.Where(where).GetPageResultAsync(input);
        }


        public async Task<PageResult<OrderItem>> GetOrderItemListAsync(PageInput<ConditionDTO> input)
        {
            var q = Service.GetIQueryable<OrderItem>().Include(it => it.Order);
            var where = LinqHelper.True<OrderItem>();
            var search = input.Search;

            //筛选
            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<OrderItem, bool>(
                    ParsingConfig.Default, false, $@"{search.Condition}.Contains(@0)", search.Keyword);
                where = where.And(newWhere);
            }

            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<Order> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task<OrderItem> GetTheItemAsync(string id)
        {
            return await Service.GetIQueryable<OrderItem>().Include(it => it.Order).FirstOrDefaultAsync(it => it.Id == id);
        }

        public async Task AddDataAsync(Order data)
        {
            await InsertAsync(data);
        }

        public async Task UpdateDataAsync(Order data)
        {
            await UpdateAsync(data);
        }

        public async Task DeleteDataAsync(List<string> ids)
        {
            await DeleteAsync(ids);
        }

        public async Task AddManyDataAsync(List<Order> orders)
        {
            await InsertAsync(orders);
        }
        public async Task UpdateManyDataAsync(List<Order> orders)
        {
            await UpdateAsync(orders);
        }

        public async Task<List<string>> GetExistsIdsAsync(List<string> inputItemIds)
        {
            var ids = new List<string>();
            if (inputItemIds == null || !inputItemIds.Any())
            {
                return ids;
            }
            return (await Service.GetIQueryable<OrderItem>()
                .Where(ord => inputItemIds.Contains(ord.Id))
                .Select(ord => ord.Id).ToListAsync()) ?? ids;
        }

        public async Task UpdateItemAsync(OrderItem item)
        {
            var order = await Service.GetIQueryable<Order>()
                 .Where(ord => ord.Id == item.OrderId)
                 .Include(ord => ord.OrderItems).FirstOrDefaultAsync();

            if (order == null)
            {
                throw new BusException("找不到订单数据");
            }
            var entity = order.OrderItems?.FirstOrDefault(it => it.Id == item.Id);
            order.Province = item.Order.Province;
            order.City = item.Order.City;
            order.Area = item.Order.Area;
            order.Address = item.Order.Address;
            order.Receiver = item.Order.Receiver;
            order.ReceiverPhone = item.Order.ReceiverPhone;

            if (entity != null)
            {
                entity.SkuId = item.SkuId;
                entity.SkuNo = item.SkuNo;
                entity.SkuName = item.SkuName;
                entity.Price = item.Price;
                entity.Count = item.Count;

            }

            order.TotalPrice = Math.Round(order.OrderItems?.Sum(it => it.Count * it.Price) ?? 0, 2, MidpointRounding.AwayFromZero);

            await Service.UpdateAsync(entity);
            await UpdateAsync(order);
        }

        #endregion

        #region 私有成员

        #endregion
    }
}
using Coldairarrow.Entity.OrderAssistant;
using Coldairarrow.Util;
using EFCore.Sharding;
using LinqKit;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Order> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
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

        #endregion

        #region 私有成员

        #endregion
    }
}
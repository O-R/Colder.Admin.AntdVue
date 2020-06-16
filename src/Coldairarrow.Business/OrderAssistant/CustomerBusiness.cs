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
    public class CustomerBusiness : BaseBusiness<Customer>, ICustomerBusiness, ITransientDependency
    {
        public CustomerBusiness(IRepository repository)
            : base(repository)
        {
        }

        #region 外部接口

        public async Task<PageResult<Customer>> GetDataListAsync(PageInput<ConditionDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<Customer>();
            var search = input.Search;

            //筛选
            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<Customer, bool>(
                    ParsingConfig.Default, false, $@"{search.Condition}.Contains(@0)", search.Keyword);
                where = where.And(newWhere);
            }

            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<Customer> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task AddDataAsync(Customer data)
        {
            await InsertAsync(data);
        }

        public async Task UpdateDataAsync(Customer data)
        {
            await UpdateAsync(data);
        }

        [Transactional(System.Data.IsolationLevel.RepeatableRead)]
        public async Task DeleteDataAsync(List<string> ids)
        {
            await Service.DeleteAsync<CustomerSku>(cs => ids.Contains(cs.CustomerId));
            await DeleteAsync(ids);
        }

        public async Task<List<CustomerSku>> GetCustomerSkuList(string customerId)
        {
            var result = await Service.GetIQueryable<CustomerSku>()
                  .Include(cs => cs.Sku)
                  .Where(cs => cs.CustomerId == customerId)
                  .ToListAsync();

            if (result != null && result.Any())
            {
                return result.OrderBy(it => it.Id).ToList();
            }
            return new List<CustomerSku>();
        }

        #endregion

        #region 私有成员

        #endregion
    }
}
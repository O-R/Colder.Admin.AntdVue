﻿using Coldairarrow.Entity.OrderAssistant;
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
    public class SkuBusiness : BaseBusiness<Sku>, ISkuBusiness, ITransientDependency
    {
        public SkuBusiness(IRepository repository)
            : base(repository)
        {
        }

        #region 外部接口

        public async Task<PageResult<Sku>> GetDataListAsync(PageInput<ConditionDTO> input)
        {
            var q = GetIQueryable();
            var where = LinqHelper.True<Sku>();
            var search = input.Search;

            //筛选
            if (!search.Condition.IsNullOrEmpty() && !search.Keyword.IsNullOrEmpty())
            {
                var newWhere = DynamicExpressionParser.ParseLambda<Sku, bool>(
                    ParsingConfig.Default, false, $@"{search.Condition}.Contains(@0)", search.Keyword);
                where = where.And(newWhere);
            }

            return await q.Where(where).GetPageResultAsync(input);
        }

        public async Task<Sku> GetTheDataAsync(string id)
        {
            return await GetEntityAsync(id);
        }

        public async Task AddDataAsync(Sku data)
        {
            await InsertAsync(data);
        }

        public async Task UpdateDataAsync(Sku data)
        {
            await UpdateAsync(data);
        }

        [Transactional(System.Data.IsolationLevel.RepeatableRead)]
        public async Task DeleteDataAsync(List<string> ids)
        {
            await Service.DeleteAsync<CustomerSku>(cs => ids.Contains(cs.SkuId));
            await DeleteAsync(ids);
        }

        public async Task<Sku> GetSkuWithPriceAsync(string id)
        {
            return await Service.GetIQueryable<Sku>()
                 .Include(sku => sku.SkuCustomers)
                 .Where(sku => sku.Id == id).FirstOrDefaultAsync();
        }

        [Transactional(System.Data.IsolationLevel.RepeatableRead)]
        public async Task SaveDataChangeAsync(Sku data, List<CustomerSku> addList, List<CustomerSku> updateList, List<CustomerSku> deleteList)
        {
            await UpdateDataAsync(data);
            if (addList.Any())
            {
                await Service.InsertAsync(addList);
            }
            if (updateList.Any())
            {
                await Service.UpdateAsync(updateList);
            }
            if (deleteList.Any())
            {
                await Service.DeleteAsync(deleteList);
            }
        }

        public async Task AddDataListAsync(List<Sku> list)
        {
            await Service.InsertAsync(list);
        }

        #endregion

        #region 私有成员

        #endregion
    }
}
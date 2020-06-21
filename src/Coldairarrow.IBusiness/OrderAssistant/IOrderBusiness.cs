using Coldairarrow.Entity.OrderAssistant;
using Coldairarrow.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coldairarrow.Business.OrderAssistant
{
    public interface IOrderBusiness
    {
        Task<List<string>> GetExistsIdsAsync(List<string> inputItemIds);
        Task<PageResult<Order>> GetDataListAsync(PageInput<ConditionDTO> input);

        Task<Order> GetTheDataAsync(string id);
        Task AddDataAsync(Order data);
        Task UpdateDataAsync(Order data);
        Task DeleteDataAsync(List<string> ids);

        Task AddManyDataAsync(List<Order> orders);
        Task UpdateManyDataAsync(List<Order> orders);
    }


    public class OrderParseInputDTO
    {
        public OrderParseInputDTO()
        {
            this.Orders = new List<OrderParseInputItemDTO>();
        }

        /// <summary>
        /// 客户Id
        /// </summary>
        public string CustomerId { get; set; }


        public List<OrderParseInputItemDTO> Orders { get; set; }
    }

    public class OrderParseInputItemDTO
    {
        public int index { get; set; }
        public string province { get; set; }
        public string provinceCode { get; set; }
        public string city { get; set; }
        public string cityCode { get; set; }
        public string county { get; set; }
        public string countyCode { get; set; }
        public string street { get; set; }
        public string streetCode { get; set; }
        public string address { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
        public string skuKeyWords { get; set; }
        public string fullAddress { get; set; }



        /// <summary>
        /// 是否解析失败
        /// </summary>
        public bool isError { get; set; }

        /// <summary>
        /// 解析失败原因
        /// </summary>
        public string errorMessage { get; set; }

        public List<Sku> Skus { get; set; }

    }
    public class OrderParseDTO
    {
        /// <summary>
        /// sku id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 订单Id
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public string Province { get; set; }
        public string ProvinceNo { get; set; }
        public string City { get; set; }
        public string CityNo { get; set; }
        public string Area { get; set; }
        public string AreaNo { get; set; }
        public string Address { get; set; }
        public string Receiver { get; set; }
        public string ReceiverPhone { get; set; }

        public string SkuId { get; set; }
        public string SkuNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string SkuName { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public string CustomerNo { get; set; }


        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Discount { get; set; }

        public decimal TotalPrice { get; set; }


        /// <summary>
        /// 是否解析失败
        /// </summary>
        public bool IsError { get; set; }

        /// <summary>
        /// 解析失败原因
        /// </summary>
        public string ErrorMessage { get; set; }


    }
}
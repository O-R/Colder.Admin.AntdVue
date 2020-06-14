using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.OrderAssistant
{
    /// <summary>
    /// Order
    /// </summary>
    [Table("Order")]
    public class Order
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key, Column(Order = 1)]
        public String Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人Id
        /// </summary>
        public String CreatorId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public String CreatorRealName { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public Boolean Deleted { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public String OrderNo { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public String CustomerNo { get; set; }

        /// <summary>
        /// 客户名
        /// </summary>
        public String CustomerName { get; set; }

        /// <summary>
        /// 客户Id
        /// </summary>
        public String CustomerId { get; set; }

        /// <summary>
        /// 所属客户
        /// </summary>
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        /// <summary>
        /// ProvinceNo
        /// </summary>
        public String ProvinceNo { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public String Province { get; set; }

        /// <summary>
        /// CityNo
        /// </summary>
        public String CityNo { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        public String City { get; set; }

        /// <summary>
        /// AreaNo
        /// </summary>
        public String AreaNo { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        public String Area { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public String Address { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        public String Receiver { get; set; }

        /// <summary>
        /// 收货人手机号
        /// </summary>
        public String ReceiverPhone { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public Decimal Discount { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public Decimal TotalPrice { get; set; }

        /// <summary>
        /// 订单详情
        /// </summary>
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
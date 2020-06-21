using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Assistant.CodeFirst.Data.Entity
{
    public class OrderItem : BaseEntity
    {
        [StringLength(50)]
        public string OrderId { get; set; }

        /// <summary>
        /// 订单
        /// </summary>
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        /// <summary>
        /// sku编号
        /// </summary>
        [StringLength(50)]
        public string SkuNo { get; set; }

        /// <summary>
        /// skuId
        /// </summary>
        [StringLength(50)]
        public string SkuId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [StringLength(100)]
        public string SkuName { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Count { get; set; }

    }
}

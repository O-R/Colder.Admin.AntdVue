using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.OrderAssistant
{
    /// <summary>
    /// OrderItem
    /// </summary>
    [Table("OrderItem")]
    public class OrderItem
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
        /// OrderId
        /// </summary>
        public String OrderId { get; set; }

        /// <summary>
        /// sku编号
        /// </summary>
        public String SkuNo { get; set; }

        /// <summary>
        /// SkuId
        /// </summary>
        public String SkuId { get; set; }

        /// <summary>
        /// sku名称
        /// </summary>
        public String SkuName { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public Decimal Price { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public Int32 Count { get; set; }

    }
}
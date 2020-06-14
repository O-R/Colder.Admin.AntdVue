using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.OrderAssistant
{
    /// <summary>
    /// CustomerSku
    /// </summary>
    [Table("CustomerSku")]
    public class CustomerSku
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
        /// 客户Id
        /// </summary>
        public String CustomerId { get; set; }

        /// <summary>
        /// SkuId
        /// </summary>
        public String SkuId { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public Decimal Price { get; set; }


        /// <summary>
        /// 客户
        /// </summary>
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        /// <summary>
        /// sku
        /// </summary>
        [ForeignKey("SkuId")]
        public Sku Sku { get; set; }

    }
}
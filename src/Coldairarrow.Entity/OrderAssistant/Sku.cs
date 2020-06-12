using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Coldairarrow.Entity.OrderAssistant
{
    /// <summary>
    /// Sku
    /// </summary>
    [Table("Sku")]
    public class Sku
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
        /// sku编号
        /// </summary>
        public String SkuNo { get; set; }

        /// <summary>
        /// sku名称
        /// </summary>
        public String SkuName { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        public String KeyWords { get; set; }

    }
}
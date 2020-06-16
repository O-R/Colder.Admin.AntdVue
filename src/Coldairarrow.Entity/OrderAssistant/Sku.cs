using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

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


        /// <summary>
        /// 关联客户
        /// </summary>
        public List<CustomerSku> SkuCustomers { get; set; } = new List<CustomerSku>();

        [NotMapped]
        public List<string> KeywordList
        {
            get
            {
                return this.KeyWords?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()??new List<string>();
            }
        }

    }
}
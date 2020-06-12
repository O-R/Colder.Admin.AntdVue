using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Assistant.CodeFirst.Data.Entity
{
    public class Sku : BaseEntity
    {
        /// <summary>
        /// sku编号
        /// </summary>
        [StringLength(50)]
        public string SkuNo { get; set; }


        /// <summary>
        /// 商品名称
        /// </summary>
        [StringLength(100)]
        public string SkuName { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        [StringLength(2000)]
        public string KeyWords { get; set; }
    }
}

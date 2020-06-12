using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Assistant.CodeFirst.Data.Entity
{
    public class CustomerSku : BaseEntity
    {
        /// <summary>
        /// 客户ID
        /// </summary>
        [StringLength(50)]
        public string CustomerId { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }


        /// <summary>
        /// skuID
        /// </summary>
        [StringLength(50)]
        public string SkuId { get; set; }

        /// <summary>
        /// sku
        /// </summary>
        [ForeignKey("SkuId")]
        public Sku Sku { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
    }
}

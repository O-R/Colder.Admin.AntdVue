using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Assistant.CodeFirst.Data.Entity
{
    [Table("x_sku")]
    public class Sku : BaseEntity
    {
        /// <summary>
        /// sku编号
        /// </summary>
        [StringLength(50)]
        public string Number { get; set; }


        /// <summary>
        /// 商品名称
        /// </summary>
        [StringLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// 关键词
        /// </summary>
        [StringLength(2000)]
        public string KeyWords { get; set; }
    }
}

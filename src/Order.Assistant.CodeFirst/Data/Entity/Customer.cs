using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Assistant.CodeFirst.Data.Entity
{
    [Table("x_customer")]
    public class Customer : BaseEntity
    {
        /// <summary>
        /// 客户编号
        /// </summary>
        [StringLength(50)]
        public string Number { get; set; }


        /// <summary>
        /// 客户名称
        /// </summary>
        [StringLength(50)]
        public string Name { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Assistant.CodeFirst.Data.Entity
{
    public class Customer : BaseEntity
    {
        /// <summary>
        /// 客户编号
        /// </summary>
        [StringLength(50)]
        public string CustomerNo { get; set; }


        /// <summary>
        /// 客户名称
        /// </summary>
        [StringLength(50)]
        public string CustomerName { get; set; }
    }
}

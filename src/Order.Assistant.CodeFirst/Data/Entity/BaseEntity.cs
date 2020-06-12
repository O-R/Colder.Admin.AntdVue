using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Assistant.CodeFirst.Data.Entity
{
    public class BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key, Column(Order = 1)]
        [StringLength(50)]
        public String Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建人Id
        /// </summary>
        [StringLength(50)]
        public String CreatorId { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        [StringLength(50)]
        public String CreatorRealName { get; set; }

        /// <summary>
        /// 否已删除
        /// </summary>
        public Boolean Deleted { get; set; }
    }
}

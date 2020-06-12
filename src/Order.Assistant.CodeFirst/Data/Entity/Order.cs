using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Assistant.CodeFirst.Data.Entity
{
    [Table("x_order")]
    public class Order : BaseEntity
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [StringLength(50)]
        public string OrderNo { get; set; }

        /// <summary>
        /// 客户账号
        /// </summary>
        [StringLength(50)]
        public string CustomerNo { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        [StringLength(50)]
        public string CustomerName { get; set; }

        /// <summary>
        /// 客户Id
        /// </summary>
        [StringLength(50)]
        public string CustomerId { get; set; }

        /// <summary>
        /// 所属客户
        /// </summary>
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        /// <summary>
        /// 省编号
        /// </summary>
        [StringLength(50)]
        public string ProvinceNo { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        [StringLength(50)]
        public string Province { get; set; }


        /// <summary>
        /// 市编号
        /// </summary>
        [StringLength(50)]
        public string CityNo { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        [StringLength(50)]
        public string City { get; set; }

        /// <summary>
        /// 区编号
        /// </summary>
        [StringLength(50)]
        public string AreaNo { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        [StringLength(50)]
        public string Area { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        [StringLength(200)]
        public string Address { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        [StringLength(50)]
        public string Receiver { get; set; }

        /// <summary>
        /// 收货人电话
        /// </summary>
        [StringLength(50)]
        public string ReceiverPhone { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }


        /// <summary>
        /// 订单详情
        /// </summary>
        public List<OrderItem> OrderItems { get; set; }


    }
}

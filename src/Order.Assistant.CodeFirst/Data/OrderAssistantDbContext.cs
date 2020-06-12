using Microsoft.EntityFrameworkCore;
using Order.Assistant.CodeFirst.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.Assistant.CodeFirst
{
    public class OrderAssistantDbContext : DbContext
    {
        public OrderAssistantDbContext(DbContextOptions<OrderAssistantDbContext> options)
               : base(options)
        {
        }

        /// <summary>
        /// 客户表
        /// </summary>
        public DbSet<Customer> Customer { get; set; }

        /// <summary>
        /// Sku表
        /// </summary>
        public DbSet<Sku> Sku { get; set; }


        /// <summary>
        /// 客户sku表
        /// </summary>
        public DbSet<CustomerSku> CustomerSku { get; set; }

        /// <summary>
        /// 订单表
        /// </summary>
        public DbSet<Data.Entity.Order> Order { get; set; }

        /// <summary>
        /// 订单明细表
        /// </summary>
        public DbSet<OrderItem> OrderItem { get; set; }
    }
}

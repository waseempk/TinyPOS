using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinyPOSApp.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("sku")]
        public string Sku { get; set; } = string.Empty;

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("cost_price")]
        public decimal? CostPrice { get; set; }

        [Column("sale_price")]
        public decimal SalePrice { get; set; }

        [Column("tax_percent")]
        public decimal? TaxPercent { get; set; }

        [Column("discount_percent")]
        public decimal? DiscountPercent { get; set; }

        [Column("apply_bulk_discount")]
        public bool? ApplyBulkDiscount { get; set; }

        [Column("stock")]
        public int? Stock { get; set; }

        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}

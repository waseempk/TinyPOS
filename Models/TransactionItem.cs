using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TinyPOSApp.Models
{
    [Table("transaction_items")]
    public class TransactionItem
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("transaction_id")]
        public Guid? TransactionId { get; set; }

        [Column("product_id")]
        public Guid? ProductId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("unit_price")]
        public decimal UnitPrice { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }
    }
}

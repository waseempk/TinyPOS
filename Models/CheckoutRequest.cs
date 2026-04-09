using System;
using System.Collections.Generic;

namespace TinyPOSApp.Models
{
    public class CheckoutRequest
    {
        public decimal TotalAmount { get; set; }
        public decimal CashReceived { get; set; }
        public decimal ChangeDue { get; set; }
        public List<CartItem> Items { get; set; } = new();
    }

    public class CartItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

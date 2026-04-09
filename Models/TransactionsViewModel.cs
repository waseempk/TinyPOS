using System;
using System.Collections.Generic;

namespace TinyPOSApp.Models
{
    public class TransactionsViewModel
    {
        public List<TransactionMasterItem> History { get; set; } = new List<TransactionMasterItem>();
        public TransactionDetailItem? SelectedTransaction { get; set; }
    }

    public class TransactionMasterItem
    {
        public Guid Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public string? CashierName { get; set; }
        public string? Status { get; set; }
    }

    public class TransactionDetailItem
    {
        public Guid Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? CashReceived { get; set; }
        public decimal? ChangeDue { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public string? CashierName { get; set; }
        public List<TransactionDetailLineItem> Items { get; set; } = new List<TransactionDetailLineItem>();
    }

    public class TransactionDetailLineItem
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}

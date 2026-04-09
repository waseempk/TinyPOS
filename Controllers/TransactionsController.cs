using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TinyPOSApp.Data;
using TinyPOSApp.Models;

namespace TinyPOSApp.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly TinyPOSContext _context;

        public TransactionsController(TinyPOSContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(Guid? id)
        {
            var viewModel = new TransactionsViewModel();

            // 1. Fetch History for Master List
            var transactionsRaw = await _context.Transactions
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            // Fetch Profiles for Cashier mapped to UserId
            var userIds = transactionsRaw.Where(t => t.UserId.HasValue).Select(t => t.UserId.Value).Distinct().ToList();
            var profilesRaw = await _context.Profiles.Where(p => userIds.Contains(p.Id)).ToListAsync();
            var profileDict = profilesRaw.ToDictionary(p => p.Id, p => $"{p.FirstName} {p.LastName}");

            foreach (var txn in transactionsRaw)
            {
                viewModel.History.Add(new TransactionMasterItem
                {
                    Id = txn.Id,
                    CreatedAt = txn.CreatedAt,
                    TotalAmount = txn.TotalAmount,
                    Status = txn.Status,
                    CashierName = txn.UserId.HasValue && profileDict.ContainsKey(txn.UserId.Value) 
                        ? profileDict[txn.UserId.Value] 
                        : "System"
                });
            }

            // 2. Fetch Selected Transaction Detail (if any)
            if (id.HasValue)
            {
                var selectedRaw = transactionsRaw.FirstOrDefault(t => t.Id == id.Value);
                if (selectedRaw != null)
                {
                    var itemsRaw = await _context.TransactionItems
                        .Where(ti => ti.TransactionId == id.Value)
                        .ToListAsync();
                    
                    var productIds = itemsRaw.Where(i => i.ProductId.HasValue).Select(i => i.ProductId.Value).Distinct().ToList();
                    var productsDict = await _context.Products
                        .Where(p => productIds.Contains(p.Id))
                        .ToDictionaryAsync(p => p.Id, p => p);

                    var cashierName = selectedRaw.UserId.HasValue && profileDict.ContainsKey(selectedRaw.UserId.Value) 
                        ? profileDict[selectedRaw.UserId.Value] 
                        : "System";

                    var detail = new TransactionDetailItem
                    {
                        Id = selectedRaw.Id,
                        CreatedAt = selectedRaw.CreatedAt,
                        TotalAmount = selectedRaw.TotalAmount,
                        CashReceived = selectedRaw.CashReceived,
                        ChangeDue = selectedRaw.ChangeDue,
                        PaymentMethod = selectedRaw.PaymentMethod,
                        Status = selectedRaw.Status,
                        CashierName = cashierName
                    };

                    foreach (var lineItem in itemsRaw)
                    {
                        var prod = lineItem.ProductId.HasValue && productsDict.ContainsKey(lineItem.ProductId.Value)
                            ? productsDict[lineItem.ProductId.Value]
                            : null;

                        detail.Items.Add(new TransactionDetailLineItem
                        {
                            ProductId = lineItem.ProductId ?? Guid.Empty,
                            ProductName = prod?.Name ?? "Unknown Item",
                            Sku = prod?.Sku ?? "N/A",
                            Quantity = lineItem.Quantity,
                            UnitPrice = lineItem.UnitPrice,
                            Subtotal = lineItem.Subtotal
                        });
                    }

                    viewModel.SelectedTransaction = detail;
                }
            }

            return View(viewModel);
        }
    }
}

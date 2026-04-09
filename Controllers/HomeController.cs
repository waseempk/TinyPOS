using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinyPOSApp.Data;
using TinyPOSApp.Models;

namespace TinyPOSApp.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly TinyPOSContext _context;

    public HomeController(TinyPOSContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.ToListAsync();
        return View(products);
    }

    [HttpPost]
    [Route("Home/Checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        if (request == null || !request.Items.Any())
        {
            return BadRequest("Invalid transaction.");
        }

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        Guid? userId = null;
        if (Guid.TryParse(userIdString, out var parsedId))
        {
            userId = parsedId;
        }

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TotalAmount = request.TotalAmount,
            CashReceived = request.CashReceived,
            ChangeDue = request.ChangeDue,
            PaymentMethod = "CASH",
            Status = "COMPLETED",
            CreatedAt = DateTime.UtcNow
        };

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        foreach (var item in request.Items)
        {
            var txItem = new TransactionItem
            {
                Id = Guid.NewGuid(),
                TransactionId = transaction.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Subtotal = item.Quantity * item.UnitPrice
            };
            _context.TransactionItems.Add(txItem);

            // Simple stock reduction
            var product = await _context.Products.FindAsync(item.ProductId);
            if(product != null)
            {
                product.Stock = (product.Stock ?? 0) - item.Quantity;
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new { success = true, transactionId = transaction.Id });
    }
}

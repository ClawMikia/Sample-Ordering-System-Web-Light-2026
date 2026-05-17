using Microsoft.AspNetCore.Mvc;
using OrderFlowMVC.Models;
using OrderFlowMVC.Services;
using OrderFlowMVC.ViewModels;

namespace OrderFlowMVC.Controllers;

public class OrdersController : Controller
{
    private readonly IApiService _api;

    public OrdersController(IApiService api) => _api = api;

    private bool IsAuthenticated => !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));
    private bool IsAdmin => HttpContext.Session.GetString("Role") == "Admin";

    public async Task<IActionResult> Index(string? status)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");

        var orders = await _api.GetOrdersAsync();

        if (!string.IsNullOrEmpty(status))
            orders = orders.Where(o => o.Status == status).ToList();

        ViewBag.SelectedStatus = status;
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");
        var order = await _api.GetOrderAsync(id);
        if (order is null) return NotFound();
        return View(order);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");

        var customers = await _api.GetCustomersAsync();
        var products  = await _api.GetProductsAsync();

        var vm = new CreateOrderViewModel
        {
            Customers = customers.Where(c => c.IsActive).ToList(),
            Products  = products.Where(p => p.IsActive && p.StockQuantity > 0).ToList()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOrderRequest model)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");

        if (model.Items == null || model.Items.Count == 0)
        {
            TempData["Error"] = "Please add at least one item to the order.";
            return RedirectToAction(nameof(Create));
        }

        var result = await _api.CreateOrderAsync(model);
        if (result is null)
        {
            TempData["Error"] = "Failed to create order. Check stock availability.";
            return RedirectToAction(nameof(Create));
        }

        TempData["Success"] = $"Order #{result.Id} created successfully.";
        return RedirectToAction(nameof(Details), new { id = result.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return Forbid();

        var result = await _api.UpdateOrderStatusAsync(id, status);
        TempData[result is not null ? "Success" : "Error"] = result is not null
            ? $"Order #{id} status updated to {status}."
            : "Failed to update order status.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAuthenticated) return RedirectToAction("Login", "Auth");
        if (!IsAdmin) return Forbid();

        var success = await _api.DeleteOrderAsync(id);
        TempData[success ? "Success" : "Error"] = success
            ? "Order deleted successfully."
            : "Failed to delete order.";
        return RedirectToAction(nameof(Index));
    }
}

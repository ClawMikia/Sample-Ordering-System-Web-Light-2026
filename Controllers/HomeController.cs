using Microsoft.AspNetCore.Mvc;
using OrderFlowMVC.Services;
using OrderFlowMVC.ViewModels;

namespace OrderFlowMVC.Controllers;

public class HomeController : Controller
{
    private readonly IApiService _api;
    public HomeController(IApiService api) => _api = api;

    public async Task<IActionResult> Index()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
            return RedirectToAction("Login", "Auth");

        var customers = await _api.GetCustomersAsync();
        var products  = await _api.GetProductsAsync();
        var orders    = await _api.GetOrdersAsync();

        var vm = new DashboardViewModel
        {
            TotalCustomers   = customers.Count,
            ActiveCustomers  = customers.Count(c => c.IsActive),
            TotalProducts    = products.Count,
            LowStockProducts = products.Count(p => p.StockQuantity < 10),
            TotalOrders      = orders.Count,
            PendingOrders    = orders.Count(o => o.Status == "Pending"),
            TotalRevenue     = orders.Where(o => o.Status == "Delivered").Sum(o => o.TotalAmount),
            RecentOrders     = orders.OrderByDescending(o => o.OrderDate).Take(5).ToList(),
            TopProducts      = products.OrderByDescending(p => p.Price).Take(6).ToList(),
        };
        return View(vm);
    }

    public IActionResult Privacy() => View();
    public IActionResult Error() => View();
}

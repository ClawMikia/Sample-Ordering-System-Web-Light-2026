using OrderFlowMVC.Models;

namespace OrderFlowMVC.ViewModels;

public class DashboardViewModel
{
    public int     TotalCustomers   { get; set; }
    public int     ActiveCustomers  { get; set; }
    public int     TotalProducts    { get; set; }
    public int     LowStockProducts { get; set; }
    public int     TotalOrders      { get; set; }
    public int     PendingOrders    { get; set; }
    public decimal TotalRevenue     { get; set; }

    public List<OrderDto>   RecentOrders { get; set; } = new();
    public List<ProductDto> TopProducts  { get; set; } = new();
}

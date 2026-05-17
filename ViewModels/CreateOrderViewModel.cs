using OrderFlowMVC.Models;

namespace OrderFlowMVC.ViewModels;

public class CreateOrderViewModel
{
    public List<CustomerDto> Customers { get; set; } = new();
    public List<ProductDto>  Products  { get; set; } = new();
    public CreateOrderRequest Order    { get; set; } = new();
}

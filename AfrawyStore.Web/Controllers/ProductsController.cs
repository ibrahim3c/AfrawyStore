using AfrawyStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AfrawyStore.Web.Controllers;

public class ProductsController : Controller
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllProductsAsync();
        return View(products);
    }
}

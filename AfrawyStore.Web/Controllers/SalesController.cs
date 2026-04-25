using Microsoft.AspNetCore.Mvc;

namespace AfrawyStore.Web.Controllers;

public class SalesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

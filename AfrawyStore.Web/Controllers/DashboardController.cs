using Microsoft.AspNetCore.Mvc;

namespace AfrawyStore.Web.Controllers;

public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

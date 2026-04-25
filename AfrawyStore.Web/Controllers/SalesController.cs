using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfrawyStore.Web.Controllers;

[Authorize]
public class SalesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

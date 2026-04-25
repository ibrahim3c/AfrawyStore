using AfrawyStore.Application.DTOs;
using AfrawyStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AfrawyStore.Web.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllUsersAsync();
        return View(users);
    }

    public IActionResult Create()
    {
        return View(new UserCreateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateDto model)
    {
        if (ModelState.IsValid)
        {
            var success = await _userService.CreateUserAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "تم إضافة المستخدم بنجاح.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("Username", "اسم المستخدم موجود مسبقاً.");
        }
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();

        var editDto = new UserEditDto
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            IsActive = user.IsActive
        };

        return View(editDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UserEditDto model)
    {
        if (id != model.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            var success = await _userService.UpdateUserAsync(model);
            if (success)
            {
                TempData["SuccessMessage"] = "تم تعديل المستخدم بنجاح.";
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError("Username", "اسم المستخدم موجود مسبقاً أو حدث خطأ أثناء الحفظ.");
        }
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var success = await _userService.ToggleUserStatusAsync(id);
        if (success)
        {
            TempData["SuccessMessage"] = "تم تغيير حالة المستخدم بنجاح.";
        }
        else
        {
            TempData["ErrorMessage"] = "حدث خطأ أثناء تغيير حالة المستخدم.";
        }
        return RedirectToAction(nameof(Index));
    }
}

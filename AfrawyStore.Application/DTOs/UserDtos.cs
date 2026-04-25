using System.ComponentModel.DataAnnotations;
using AfrawyStore.Domain.Enums;

namespace AfrawyStore.Application.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class UserCreateDto
{
    [Required(ErrorMessage = "اسم المستخدم مطلوب")]
    [MinLength(4, ErrorMessage = "اسم المستخدم يجب أن يكون 4 أحرف على الأقل")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    [MinLength(6, ErrorMessage = "كلمة المرور يجب أن تكون 6 أحرف على الأقل")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "الاسم الكامل مطلوب")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "الصلاحية مطلوبة")]
    public UserRole Role { get; set; }
    
    public bool IsActive { get; set; } = true;
}

public class UserEditDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "اسم المستخدم مطلوب")]
    [MinLength(4, ErrorMessage = "اسم المستخدم يجب أن يكون 4 أحرف على الأقل")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "الاسم الكامل مطلوب")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "الصلاحية مطلوبة")]
    public UserRole Role { get; set; }

    public bool IsActive { get; set; }
}

public class LoginDto
{
    [Required(ErrorMessage = "اسم المستخدم مطلوب")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}

public class ChangePasswordDto
{
    [Required(ErrorMessage = "كلمة المرور الحالية مطلوبة")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
    [MinLength(6, ErrorMessage = "كلمة المرور يجب أن تكون 6 أحرف على الأقل")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
    [Compare("NewPassword", ErrorMessage = "كلمة المرور غير متطابقة")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

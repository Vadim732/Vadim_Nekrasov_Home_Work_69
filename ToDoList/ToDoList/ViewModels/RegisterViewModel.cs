using System.ComponentModel.DataAnnotations;

namespace ToDoList.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Enter your name!")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Email is required!")]
    [EmailAddress(ErrorMessage = "Invalid email address!")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password is required!")]
    [MinLength(8)]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    [Required(ErrorMessage = "Confirm password is required!")]
    [DataType(DataType.Password)]
    [MinLength(8)]
    [Compare("Password", ErrorMessage = "Passwords do not match!")]
    public string ConfirmPassword { get; set; }
}
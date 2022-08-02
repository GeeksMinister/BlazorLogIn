

namespace BlazorLogIn.Models;

public class AuthenticationUserModel
{
    [Required(ErrorMessage = "ID is required")]
    public string Id { get; set; }

    [Required(ErrorMessage = "Type in password to login")]
    public string Password { get; set; }
}


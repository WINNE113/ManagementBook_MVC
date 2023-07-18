using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.Authentication.Login
{
    public class Login
    {
        [Required(ErrorMessage ="User Name is required")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}

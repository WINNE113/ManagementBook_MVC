using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.Authentication.SignUp
{
    public class RegisterUser
    {
        [Required(ErrorMessage ="User Name is required")]
        public string? UserName { get; set; }
        [Required(ErrorMessage ="Email is required")]
        public string? Email { get; set; }
        [Required(ErrorMessage ="Password must have Uppercase and Character")]
        public string? Password { get; set; }
    }
}

using BookStore.Models.Authentication.SignUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models.ViewModels
{
    public class RegisterUserVM
    {
        public string Role { get; set; }
        public RegisterUser? RegisterUser { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Models.ModelsToRequest
{
    public class Login2FARequest
    {
        public string? OtpCode { get; set; }
        public string? UserName { get; set; }
    }
}

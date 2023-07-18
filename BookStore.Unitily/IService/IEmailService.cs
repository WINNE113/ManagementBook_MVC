using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagementService.Models;

namespace BookStore.Unitily
{
    public interface IEmailService
    {
        void SendEmail(Message message);
    }
}

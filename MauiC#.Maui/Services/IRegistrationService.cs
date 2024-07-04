using MauiC_.Maui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiC_.Maui.Services
{
    public interface IRegistrationService
    {
        Task<User> Register(string login, string email, string password, string fuulName, string photoPath);
        Task<User> GetUserByName(string name);
    }
}

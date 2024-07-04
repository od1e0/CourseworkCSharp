using System;
using System.Collections.Generic;
using System.Linq;
using MauiC_.Maui.Models;
using System.Text;
using System.Threading.Tasks;

namespace MauiC_.Maui.Services
{
    public interface IUserService
    {
        Task<User> UpdateUser(User user);


    }
}

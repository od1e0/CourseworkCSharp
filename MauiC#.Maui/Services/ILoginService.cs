using MauiC_.Maui.Models;

namespace MauiC_.Maui.Services;

public interface ILoginService
{
    Task<User> Login(string email, string password);
}
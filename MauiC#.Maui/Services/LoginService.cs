using MauiC_.Maui.Models;
using System.Net.Http.Json;

namespace MauiC_.Maui.Services;

public class LoginService : ILoginService
{
    public async Task<User> Login(string email, string password)
    {
        _ = new User();
        var client = new HttpClient();
        string url = "https://localhost:7012/api/User/login/" + email + "/" + password;
        client.BaseAddress = new Uri(url);
        HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
        if (response.IsSuccessStatusCode)
        {
            User user = await response.Content.ReadFromJsonAsync<User>();
            return await Task.FromResult(user!);
        }
        return null!;
    }
}

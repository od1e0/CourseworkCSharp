using MauiC_.Maui.Models;
using System.Net.Http.Json;

namespace MauiC_.Maui.Services;

public class LoginService : ILoginService
{
    public async Task<User> Login(string email, string password)
    {
        _ = new User();
        var client = new HttpClient();
        string url = "http://courseworkformaui.somee.com/api/User/login/" + email + "/" + password;
        client.BaseAddress = new Uri(url);
        HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
        if (response.IsSuccessStatusCode)
        {
            User user = await response.Content.ReadFromJsonAsync<User>();
            return await Task.FromResult(user!);
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "All fields required", "Ok");
        }
        return null!;
    }
}

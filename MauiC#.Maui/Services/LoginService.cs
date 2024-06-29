using MauiC_.Maui.Models;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MauiC_.Maui.Services
{
    public class LoginService : ILoginService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<User> Login(string email, string password)
        {
            try
            {
                string passwordHash = HashPassword(password);

                string url = $"http://courseworkformaui.somee.com/api/User/login/{email}/{passwordHash}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<User>();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Invalid email or password", "Ok");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to login: {ex.Message}", "Ok");
            }

            return null;
        }

        private string HashPassword(string password)
        {
            using (var sha256Hash = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}

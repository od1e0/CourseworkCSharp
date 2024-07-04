using MauiC_.Maui.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Diagnostics;
using System.Text;
using System.Security.Cryptography;
using System.Net;

namespace MauiC_.Maui.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<User> Register(string login, string email, string password, string fullName, string photoPath)
        {
            string url = "http://courseworkformaui.somee.com/api/User/";

            var newUser = new User
            {
                Name = login,
                Email = email,
                Password = HashPassword(password),
                FullName = fullName,
                LevelProgress = 1,
                PhotoPath = photoPath,
                AttractionsCount = 0
            };

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(url, newUser);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    User user = JsonSerializer.Deserialize<User>(responseContent);
                    return user;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Ошибка регистрации", "ОК");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", "Ошибка регистрации", "ОК");
            }
            return null;
        }

        public async Task<User> GetUserByName(string name)
        {
            string url = $"http://courseworkformaui.somee.com/api/User/name/{name}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    User user = JsonSerializer.Deserialize<User>(responseContent);
                    return user;
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    Debug.WriteLine($"Failed to get user by name. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while getting user by name: {ex.Message}");
            }

            return null;
        }



        private string HashPassword(string password)
        {
            using (var sha256Hash = SHA256.Create())
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

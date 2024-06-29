using MauiC_.Maui.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Diagnostics;
using System.Text;

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
                PhotoPath = photoPath,
                LevelProgress = 0,
                AttractionsCount = 0,
                Achievements = new List<Achievement>(),
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
                    await Shell.Current.DisplayAlert("Error", "Ошибка регистрации", "Ok");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", "Ошибка регистрации", "Ok");
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

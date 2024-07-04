using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MauiC_.Maui.Models;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace MauiC_.Maui.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<User> UpdateUser(User user)
        {
            try
            {
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"http://courseworkformaui.somee.com/api/User/{user.UserId}", content);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<User>();
                }
                else
                {
                    throw new Exception("Не удалось обновить информацию");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении пользователя: {ex.Message}");
            }
        }
    }
}

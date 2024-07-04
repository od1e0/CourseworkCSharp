using AdminMap.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace AdminMap.Services
{
    public class AttractionService : IAttractionService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<List<Attraction>> GetAllAttractions()
        {
            try
            {
                string url = "http://courseworkformaui.somee.com/api/Attractions";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Attraction>>();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Не удалось получить достопримечательности", "Ок");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось получить достопримечательности: {ex.Message}", "Ок");
            }

            return new List<Attraction>();
        }

        public async Task AddAttraction(Attraction attraction)
        {
            try
            {
                string url = "http://courseworkformaui.somee.com/api/Attractions";
                var response = await _httpClient.PostAsJsonAsync(url, attraction);

                if (response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Успех", "Достопримечательность успешно добавлена", "Ок");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Не удалось добавить достопримечательность", "Ок");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось добавить достопримечательность: {ex.Message}", "Ок");
            }
        }

        public async Task UpdateAttraction(Attraction attraction)
        {
            try
            {
                string url = $"http://courseworkformaui.somee.com/api/Attractions/{attraction.Id}";
                var response = await _httpClient.PutAsJsonAsync(url, attraction);

                if (response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Успех", "Достопримечательность успешно обновлена", "Ок");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Не удалось обновить достопримечательность", "Ок");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось обновить достопримечательность: {ex.Message}", "Ок");
            }
        }

        public async Task DeleteAttraction(Attraction attraction)
        {
            try
            {
                string url = $"http://courseworkformaui.somee.com/api/Attractions/{attraction.Id}";
                var response = await _httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    await Shell.Current.DisplayAlert("Успех", "Достопримечательность успешно удалена", "Ок");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Ошибка", "Не удалось удалить достопримечательность", "Ок");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Ошибка", $"Не удалось удалить достопримечательность: {ex.Message}", "Ок");
            }
        }
    }
}

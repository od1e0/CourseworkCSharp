using MauiC_.Maui.Models;
using Microsoft.Maui.Controls;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using System.Net.Http;
using System.Diagnostics;
using System.Text;
using MauiC_.Maui.ViewModels;

namespace MauiC_.Maui.Views
{
    public partial class AttractionPage : ContentPage
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly EventAggregator _eventAggregator;  
        public Attraction Attraction { get; set; }

        public AttractionPage(Attraction attraction)
        {
            InitializeComponent();
            Attraction = attraction;
            IncrementViewsIfFirstVisit();
            ShowAchievementIfFirstVisit();
            _eventAggregator = App.EventAggregator;
            _eventAggregator.AchievementsUpdated += OnAchievementsUpdated;
        }
        private async void OnAchievementsUpdated(object sender, EventArgs e)
        {
            // Обработка события обновления достижений
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Attraction != null)
            {
                NameAttraction.Text = Attraction.Label;
                AdressAttraction.Text = Attraction.Address;
                DescriptionAttraction.Text = Attraction.Description;
                ViewsAttraction.Text = Attraction.Views.ToString();
                ImageAttractio.Source = Attraction.ImageUrl;
            }
            else
            {
                DisplayAlert("Ошибка", "Информация не найдена", "Ок");
            }
        }

        private async void OnFrameTapped(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopAsync();
        }

        private async void ShowAchievementIfFirstVisit()
        {
            bool isFirstVisit = Preferences.Get("AttractionPageVisited", false);

            if (isFirstVisit)
            {
                Preferences.Set("AttractionPageVisited", true);

                AchievementImage.Source = "achievement_placeholder.png";
                AchievementText.Text = "Первооткрыватель!";
                await AddAchievementToDatabase(App.user.UserId, "achievement_placeholder.png", "Первооткрыватель!");

                _eventAggregator.NotifyAchievementsUpdated();

                AchievementNotification.IsVisible = true;
                await AchievementNotification.FadeTo(1, 2000);

                await Task.Delay(2000);

                await AchievementNotification.FadeTo(0, 1000);
                AchievementNotification.IsVisible = false;
            }
        }

        private async Task AddAchievementToDatabase(int userId, string imageSource, string textContent)
        {
            try
            {
                var newAchievement = new Achievement
                {
                    UserId = userId,
                    AchievementImageSource = imageSource,
                    AchievementTextContent = textContent,
                };

                var json = JsonConvert.SerializeObject(newAchievement);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("http://courseworkformaui.somee.com/api/achievements", content);

                if (!response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Error", "Failed to save achievement.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ThreeDModelPage(Attraction));
        }

        private void OnDescriptionScrollViewScrolled(object sender, ScrolledEventArgs e)
        {

            if (e.ScrollY > 50)
            {
                View3DButton.IsVisible = false;
            }
            else
            {
                View3DButton.IsVisible = true;
            }
        }

        private async void IncrementViewsIfFirstVisit()
        {
            string visitKey = $"AttractionVisited_{Attraction.Id}_{App.user.UserId}";
            bool visited = Preferences.Get(visitKey, false);

            if (!visited)
            {
                Preferences.Set(visitKey, true);
                Attraction.IncrementViews();
                ViewsAttraction.Text = Attraction.Views.ToString();
                await UpdateAttractionViewsOnServer(Attraction.Id);
                await UpdateUserStatiticsViewsOnServer(App.user.UserId, 5);
            }
        }

        private async Task UpdateAttractionViewsOnServer(int attractionId)
        {
            string url = $"http://courseworkformaui.somee.com/api/Attractions/{attractionId}/incrementViews";
            var response = await _httpClient.PutAsync(url, null);

            if (!response.IsSuccessStatusCode)
            { 
                await DisplayAlert("Error", "Failed to update view count on the server.", "OK");
            }
        }

        private async Task UpdateUserStatiticsViewsOnServer(int userId, double progress)
        {
            string url = $"http://courseworkformaui.somee.com/api/User/{userId}/addLevelProgress/{progress}";
            var response = await _httpClient.PutAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                Debug.Write("Failed to update view count on the server.");
            }

            string url1 = $"http://courseworkformaui.somee.com/api/User/{userId}/incrementViews";
            var response1 = await _httpClient.PutAsync(url, null);

            if (!response1.IsSuccessStatusCode)
            {
                Debug.Write("Failed to update view count on the server.");
            }
        }
    }
}

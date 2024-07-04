using CommunityToolkit.Mvvm.ComponentModel;
using MauiC_.Maui.Models;
using MauiC_.Maui.Services;
using System.Collections.ObjectModel;
using System.Net.Http.Json;

namespace MauiC_.Maui.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private readonly IPhotoService _photoService;
        private readonly HttpClient _httpClient;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string fullName;

        private double levelProgress;
        public double LevelProgress
        {
            get => levelProgress;
            set
            {
                if (levelProgress != value)
                {
                    levelProgress = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NextLevelProgress));
                    OnPropertyChanged(nameof(ProgressBarProgress));
                }
            }
        }

        public string NextLevelProgress => $"До следующего уровня: {100 - (levelProgress-1 % 10) * 10}%";
        public double ProgressBarProgress => 0.1 + levelProgress / 100.0 * 0.9;

        [ObservableProperty]
        private int attractionsCount;

        [ObservableProperty]
        private string _profileImageSource;

        [ObservableProperty]
        private string _photoBase64;

        [ObservableProperty]
        private ObservableCollection<Achievement> achievements;

        public ProfileViewModel()
        {
            _photoService = new PhotoService();
            _httpClient = new HttpClient();
            RefreshProfile();
            App.EventAggregator.AchievementsUpdated += OnAchievementsUpdated;
        }

        private async void OnAchievementsUpdated(object sender, EventArgs e)
        {
            RefreshProfile();
        }

        public async void RefreshProfile()
        {
            User user = App.user;
            Name = user.Name;
            Email = user.Email;
            FullName = user.FullName;
            LevelProgress = user.LevelProgress;
            AttractionsCount = user.AttractionsCount;
            try
            {
                var response = await _httpClient.GetAsync($"http://courseworkformaui.somee.com/api/Achievements/user/{user.UserId}");

                response.EnsureSuccessStatusCode(); 

                Achievements = await response.Content.ReadFromJsonAsync<ObservableCollection<Achievement>>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении достижений: {ex.Message}");
            }
        }
    }
}

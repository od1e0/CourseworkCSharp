using CommunityToolkit.Mvvm.ComponentModel;
using MauiC_.Maui.Models;
using System.Collections.ObjectModel;

namespace MauiC_.Maui.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string password;

        [ObservableProperty]
        private string fullName;

        [ObservableProperty]
        private double levelProgress;

        [ObservableProperty]
        private int attractionsCount;

        [ObservableProperty]
        private string profileImageSource;

        [ObservableProperty]
        private ObservableCollection<Achievement> achievements;

        public ProfileViewModel()
        {
            User user = App.user;
            Name = user.Name;
            Email = user.Email;
            FullName = user.FullName;
            LevelProgress = user.LevelProgress;
            AttractionsCount = user.AttractionsCount;
            ProfileImageSource = "dotnet_bot.svg";
            Achievements = new ObservableCollection<Achievement>
            {
                new Achievement { AchievementId = 1, AchievementImageSource = "achievement_placeholder.png", AchievementTextContent = "First Achievement" },
                new Achievement { AchievementId = 2, AchievementImageSource = "achievement_placeholder.png", AchievementTextContent = "Second Achievement" }
            };
        }
    }
}

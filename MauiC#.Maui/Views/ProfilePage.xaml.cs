using MauiC_.Maui.Models;
using System.Collections.ObjectModel;

namespace MauiC_.Maui.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ObservableCollection<Achievement> Achievements { get; set; }

        public ProfilePage()
        {
            InitializeComponent();
            LoadAchievements();
            BindingContext = this;
        }

        private void LoadAchievements()
        {
            Achievements = new ObservableCollection<Achievement>
            {
                new Achievement { AchievementImageSource = "achievement_placeholder.png", AchievementTextContent = "First Visit!" },
                new Achievement { AchievementImageSource = "achievement_placeholder.png", AchievementTextContent = "Explorer!" },
                new Achievement { AchievementImageSource = "achievement_placeholder.png", AchievementTextContent = "Master Traveler!" }
            };

            AchievementsCollectionView.ItemsSource = Achievements;
        }
    }
}

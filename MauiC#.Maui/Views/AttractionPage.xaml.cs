using MauiC_.Maui.Models;
using Microsoft.Maui.Controls;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace MauiC_.Maui.Views
{
    public partial class AttractionPage : ContentPage
    {
        public Attraction Attraction { get; set; }

        public AttractionPage(Attraction attraction)
        {
            InitializeComponent();
            Attraction = attraction;
            ShowAchievementIfFirstVisit();
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
            bool isFirstVisit = !Preferences.Get("AttractionPageVisited", false);

            if (isFirstVisit)
            {
                Preferences.Set("AttractionPageVisited", true);

                AchievementImage.Source = "achievement_placeholder.png";
                AchievementText.Text = "Congratulations! You visited your first attraction!";

                AchievementNotification.IsVisible = true;
                await AchievementNotification.FadeTo(1, 2000);

                await Task.Delay(2000);

                await AchievementNotification.FadeTo(0, 1000);
                AchievementNotification.IsVisible = false;
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ThreeDModelPage());
        }
    }

}

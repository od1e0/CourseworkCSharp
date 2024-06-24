namespace MauiC_.Maui.Views;

public partial class AttractionPage : ContentPage
{
	public AttractionPage()
	{
		InitializeComponent();
        ShowAchievementIfFirstVisit();

    }

    private async void OnFrameTapped(object sender, EventArgs e)
    {
        await Shell.Current.Navigation.PopAsync();
    }

    private async void ShowAchievementIfFirstVisit()
    {

        bool isFirstVisit = true;

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
namespace MauiC_.Maui.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}

    private async void PrivacyAlert(object sender, EventArgs e)
    {
        await Shell.Current.DisplayAlert(
            "Our Privacy Policy",
            "We take the utmost care to ensure that GDPR laws are followed.",
            "I understand");
    }
}
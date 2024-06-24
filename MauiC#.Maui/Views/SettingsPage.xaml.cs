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

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Выберите изображение профиля"
            });

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                ProfileImage.Source = ImageSource.FromStream(() => new MemoryStream(memoryStream.ToArray()));
            }
        }
        catch (Exception ex)
        {

        }
    }

}
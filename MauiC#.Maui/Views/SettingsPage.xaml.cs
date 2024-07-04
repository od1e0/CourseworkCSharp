using MauiC_.Maui.ViewModels;

namespace MauiC_.Maui.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
        BindingContext = new SettingsViewModel();
	}

    private async void PrivacyAlert(object sender, EventArgs e)
    {
        await DisplayAlert(
            "���� �������� ������������������",
            "�� ��������� ��� ������, ����� ���������� ���������� ������� GDPR.",
            "� �������");
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "�������� ����������� �������"
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
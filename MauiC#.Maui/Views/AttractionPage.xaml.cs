namespace MauiC_.Maui.Views;

public partial class AttractionPage : ContentPage
{
	public AttractionPage()
	{
		InitializeComponent();
	}

    private async void OnFrameTapped(object sender, EventArgs e)
    {
        // ������� ������� ��������
        await Shell.Current.Navigation.PopAsync();
    }
}
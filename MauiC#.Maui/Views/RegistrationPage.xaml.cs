using MauiC_.Maui.Services;
using MauiC_.Maui.Models;
using Newtonsoft.Json;
using Microsoft.Maui.Storage;

namespace MauiC_.Maui.Views;

public partial class RegistrationPage : ContentPage
{
    private readonly RegistrationService registerService = new RegistrationService();
    private string _photoPath;

    public RegistrationPage()
    {
        InitializeComponent();
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }
}




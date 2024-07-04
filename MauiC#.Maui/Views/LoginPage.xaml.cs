using MauiC_.Maui.Models;
using MauiC_.Maui.Services;
using Newtonsoft.Json;

namespace MauiC_.Maui.Views;

public partial class LoginPage : ContentPage
{

    readonly ILoginService loginService = new LoginService();

    public LoginPage()
	{
		InitializeComponent();
	}


    private async void OnRegisterButtonClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//RegisterPage");
    }
}
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

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                if (!string.IsNullOrWhiteSpace(username.Text) && !string.IsNullOrWhiteSpace(password.Text))
                {
                    User user = await loginService.Login(username.Text, password.Text);
                    if (user == null)
                    {
                        await Shell.Current.DisplayAlert("Error", "Username/Password is incorrect", "Ok");
                        return;
                    }
                    if (Preferences.ContainsKey(nameof(App.user)))
                    {
                        Preferences.Remove(nameof(App.user));
                    }
                    string userDetails = JsonConvert.SerializeObject(user);
                    Preferences.Set(nameof(App.user), userDetails);
                    App.user = user;
                    await Shell.Current.GoToAsync("//ProfilePage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "All fields required", "Ok");
                    return;
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No Internet Access", "Ok");
                return;
            }

        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
            return;
        }
    }
}
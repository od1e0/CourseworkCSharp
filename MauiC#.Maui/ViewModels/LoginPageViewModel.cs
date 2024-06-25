using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiC_.Maui.Models;
using MauiC_.Maui.Services;
using MauiC_.Maui.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiC_.Maui.ViewModels;

public partial class LoginPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string _userName;

    [ObservableProperty]
    private string _password;

    readonly ILoginService loginService = new LoginService();

    [RelayCommand]
    public async void Login()
    {
        try
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                if (!string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Password))
                {
                    User user = await loginService.Login(UserName, Password);
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
                    await Shell.Current.GoToAsync(nameof(HomePage));
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

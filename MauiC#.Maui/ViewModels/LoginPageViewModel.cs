using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiC_.Maui.Models;
using MauiC_.Maui.Services;
using MauiC_.Maui.Views;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MauiC_.Maui.ViewModels;

public partial class LoginPageViewModel : ObservableObject
{
    [ObservableProperty]
    private string _userName;

    [ObservableProperty]
    private string _password;

    [ObservableProperty]
    private string _errorText;

    [ObservableProperty]
    private bool _isErrorVisible;

    readonly ILoginService loginService = new LoginService();

    [RelayCommand]
    public async void Login()
    {
        try
        {
            IsErrorVisible = false;

            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                ShowError("Нет доступа к Интернету");
                return;
            }

            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            {
                ShowError("Все поля обязательны для заполнения");
                return;
            }

            User user = await loginService.Login(UserName, Password);
            if (user == null)
            {
                ShowError("Неверное имя пользователя или пароль");
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
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }

    private void ShowError(string message)
    {
        ErrorText = message;
        IsErrorVisible = true;
    }
}

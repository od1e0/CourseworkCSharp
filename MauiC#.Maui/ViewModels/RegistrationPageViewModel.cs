using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiC_.Maui.Services;
using MauiC_.Maui.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text.Json;

namespace MauiC_.Maui.ViewModels
{
    public partial class RegistrationPageViewModel : ObservableObject
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationPageViewModel()
        {
            _registrationService = new RegistrationService();
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        private string _photoPath;
        public string PhotoPath
        {
            get => _photoPath;
            set => SetProperty(ref _photoPath, value);
        }

        [RelayCommand]
        public async Task RegisterAsync()
        {
            try
            {
                if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
                {
                    if (!string.IsNullOrWhiteSpace(Name) &&
                        !string.IsNullOrWhiteSpace(Email) &&
                        !string.IsNullOrWhiteSpace(Password) &&
                        !string.IsNullOrWhiteSpace(ConfirmPassword))
                    {
                        if (Password != ConfirmPassword)
                        {
                            await Shell.Current.DisplayAlert("Error", "Passwords do not match", "Ok");
                            return;
                        }

                        Debug.WriteLine("Starting registration process...");
                        Debug.WriteLine($"Name: {Name}, Email: {Email}, PhotoPath: {PhotoPath}");

                        User user = await _registrationService.Register(Name, Email, Password, Name, PhotoPath);
                        await Shell.Current.DisplayAlert("Error", $"{user}", "Ok");
                        if (user == null)
                        {
                            await Shell.Current.DisplayAlert("Error", "Registration failed", "Ok");
                            Debug.WriteLine("User is null after registration attempt.");
                            return;
                        }

                        if (Preferences.ContainsKey(nameof(App.user)))
                        {
                            Preferences.Remove(nameof(App.user));
                        }
                        string userDetails = JsonConvert.SerializeObject(user);
                        Preferences.Set(nameof(App.user), userDetails);
                        App.user = user;
                        await Shell.Current.GoToAsync("//LoginPage");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "All fields required", "Ok");
                        Debug.WriteLine("Required fields are missing.");
                        return;
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "No Internet Access", "Ok");
                    Debug.WriteLine("No Internet Access.");
                    return;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "Ok");
                Debug.WriteLine($"Exception occurred: {ex.Message}");
            }
        }



        [RelayCommand]
        private async Task SelectPhotoAsync()
        {
            try
            {
                FileResult photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select a photo"
                });

                if (photo != null)
                {
                    PhotoPath = photo.FullPath;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}

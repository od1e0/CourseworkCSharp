using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiC_.Maui.Services;
using MauiC_.Maui.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MauiC_.Maui.ViewModels
{
    public partial class RegistrationPageViewModel : ObservableObject
    {
        private readonly IRegistrationService _registrationService;
        private readonly IPhotoService _photoService;

        public RegistrationPageViewModel()
        {
            _registrationService = new RegistrationService();
            _photoService = new PhotoService();
        }

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _fullName;

        [ObservableProperty]
        private string _email;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _confirmPassword;

        [ObservableProperty]
        private string _photoPath;

        [ObservableProperty]
        private string _errorText;

        [ObservableProperty]
        private bool _isErrorVisible;

        [RelayCommand]
        public async Task RegisterAsync()
        {
            try
            {
                IsErrorVisible = false;

                if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                {
                    ShowError("Нет доступа к Интернету");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) ||
                    string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
                {
                    ShowError("Все поля обязательны для заполнения");
                    return;
                }

                if (Name.Length < 4)
                {
                    ShowError("Имя должно быть не менее 4 символов");
                    return;
                }

                if (Password.Length < 6)
                {
                    ShowError("Пароль должен быть не менее 6 символов");
                    return;
                }

                if (Password != ConfirmPassword)
                {
                    ShowError("Пароли не совпадают");
                    return;
                }

                User existingUser = await _registrationService.GetUserByName(Name);
                if (existingUser != null)
                {
                    ShowError("Пользователь с таким именем уже зарегистрирован");
                    return;
                }

                if (!IsValidEmail(Email))
                {
                    ShowError("Некорректный формат email");
                    return;
                }

                string photoUrl = string.Empty;
                string fileName = string.Empty;
                if (!string.IsNullOrWhiteSpace(PhotoPath))
                {
                    var extension = Path.GetExtension(PhotoPath);
                    fileName = $"{Guid.NewGuid()}{extension}"; ;
                    using (var stream = File.OpenRead(PhotoPath))
                    {
                        photoUrl = await _photoService.UploadPhoto(stream, fileName);
                    }
                }


                User user = await _registrationService.Register(Name, Email, Password, Name, fileName);
                if (user == null)
                {
                    ShowError("Не удалось зарегистрировать пользователя");
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

        [RelayCommand]
        private async Task SelectPhotoAsync()
        {
            try
            {
                FileResult photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Выберите фото"
                });

                if (photo != null)
                {
                    PhotoPath = photo.FullPath;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private bool IsValidEmail(string email)
        {
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }
    }
}

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using MauiC_.Maui.Models;
using Newtonsoft.Json;
using MauiC_.Maui.Services;
using System.Windows.Input;

namespace MauiC_.Maui.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;
        private readonly IPhotoService _photoService;

        [ObservableProperty]
        private string _profileImageSource = App.user?.PhotoPath ?? "dotnet_bot.svg";

        [ObservableProperty]
        private string _name = App.user?.Name;

        [ObservableProperty]
        private string _fullName = App.user?.FullName;

        [ObservableProperty]
        private string _photo;

        [ObservableProperty]
        private string _email = App.user?.Email;
        [ObservableProperty]
        private string _photoPath;

        [ObservableProperty]
        private string _password = App.user?.Password;

        public ICommand ClearPasswordCommand => new Command(() =>
        {
            Password = string.Empty;
        });

        public SettingsViewModel()
        {
            Photo = "http://courseworkformaui.somee.com/api/Image/" + App.user.PhotoPath;
            _userService = new UserService();
            _photoService = new PhotoService();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://courseworkformaui.somee.com/api/User/")
            };
        }

        [RelayCommand]
        public async void ChangeProfilePicture()
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
                Debug.WriteLine($"Error selecting profile picture: {ex}");
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Произошла ошибка при выборе изображения: {ex.Message}", "ОК");
            }

        }

        [RelayCommand]
        public async void Save()
        {
            try
            {
                var updatedUser = new User
                {
                    UserId = App.user.UserId,
                    FullName = FullName,
                    Name = Name,
                    LevelProgress = App.user.LevelProgress,
                    AttractionsCount = App.user.AttractionsCount,
                    Achievements = new List<Achievement>(),
                    Email = Email,
                };

                if (!string.IsNullOrEmpty(Password) && HashPassword(Password) != App.user.Password)
                {
                    updatedUser.Password = HashPassword(Password);
                }
                else
                {
                    updatedUser.Password = App.user.Password;
                }



                string photoUrl = string.Empty;
                string fileName = string.Empty;
                if (!string.IsNullOrWhiteSpace(PhotoPath) && PhotoPath != App.user.PhotoPath)
                {

                    if (!string.IsNullOrWhiteSpace(App.user.PhotoPath))
                    {
                        await _photoService.DeletePhoto(App.user.PhotoPath);
                    }

                    var extension = Path.GetExtension(PhotoPath);
                    fileName = $"{Guid.NewGuid()}{extension}"; ;
                    using (var stream1 = File.OpenRead(PhotoPath))
                    {
                        photoUrl = await _photoService.UploadPhoto(stream1, fileName);
                        updatedUser.PhotoPath = fileName;
                        Photo = "http://courseworkformaui.somee.com/api/Image/" + fileName;
                    }
                }
                else
                {
                    updatedUser.PhotoPath = App.user.PhotoPath;
                }

                await UpdateUser(updatedUser);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving user information: {ex}");
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Произошла ошибка при сохранении информации: {ex.Message}", "ОК");
            }
        }

        [RelayCommand]
        public async void Exit()
        {
            App.user = null;
            await Shell.Current.GoToAsync("//LoginPage");
        }

        private async Task UpdateUser(User updatedUser)
        {
            try
            {
                var json = JsonConvert.SerializeObject(updatedUser);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{updatedUser.UserId}", content);
                response.EnsureSuccessStatusCode();

                App.user = updatedUser;
                await Application.Current.MainPage.DisplayAlert("Успех", "Информация обновлена", "ОК");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating user on server: {ex}");
                await Application.Current.MainPage.DisplayAlert("Ошибка", $"Не удалось обновить информацию: {ex.Message}", "ОК");
            }
        }




        private string HashPassword(string password)
        {
            using (var sha256Hash = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}

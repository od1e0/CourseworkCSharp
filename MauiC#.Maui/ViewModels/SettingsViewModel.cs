using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace MauiC_.Maui.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private string profileImageSource = "dotnet_bot.svg"; 

        public SettingsViewModel()
        {
            ChangeProfilePictureCommand = new AsyncRelayCommand(ChangeProfilePicture);
        }

        public IAsyncRelayCommand ChangeProfilePictureCommand { get; }

        private async Task ChangeProfilePicture()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    FileTypes = FilePickerFileType.Images,
                    PickerTitle = "Select an image"
                });

                if (result != null)
                {
                    ProfileImageSource = result.FullPath;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
    }
}

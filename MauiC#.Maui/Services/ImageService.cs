using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using FileSystem = Xamarin.Essentials.FileSystem;

namespace MauiC_.Maui.Services;
public class ImageService
{
    public async Task<string> SaveImageBase64ToFileAsync(string base64String, string fileName)
    {
        if (string.IsNullOrEmpty(base64String))
        {
            await Shell.Current.DisplayAlert("Error", "Base64 string is null or empty", "OK");
            return null;
        }

        byte[] imageBytes;
        try
        {
            imageBytes = Convert.FromBase64String(base64String);
        }
        catch (FormatException ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Base64 conversion failed: {ex.Message}", "OK");
            return null;
        }

        string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

        try
        {
            await File.WriteAllBytesAsync(filePath, imageBytes);
            await Shell.Current.DisplayAlert("Info", $"File saved: {filePath}", "OK");
            return filePath;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"File save failed: {ex.Message}", "OK");
            return null;
        }
    }

    public ImageSource LoadImageFromFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            Shell.Current.DisplayAlert("Info", $"File found: {filePath}", "OK");
            return ImageSource.FromFile(filePath);
        }
        else
        {
            Shell.Current.DisplayAlert("Error", $"File not found: {filePath}", "OK");
        }

        return null;
    }
}


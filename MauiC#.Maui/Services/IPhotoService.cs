using MauiC_.Maui.Models;
using System.IO;
using System.Threading.Tasks;

namespace MauiC_.Maui.Services;

public interface IPhotoService
{
    Task<string> UploadPhoto(Stream photoStream, string fileName);
    Task<PhotoResponse> GetPhotoByName(string fileName);
    Task DeletePhoto(string fileName);
}

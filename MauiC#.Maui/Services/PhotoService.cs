using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using MauiC_.Maui.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static System.Net.WebRequestMethods;

namespace MauiC_.Maui.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly HttpClient _httpClient;

        public PhotoService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> UploadPhoto(Stream photoStream, string fileName)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(photoStream)
                {
                    Headers =
                    {
                        ContentLength = photoStream.Length,
                        ContentType = new MediaTypeHeaderValue("image/jpeg")
                    }
                };

                content.Add(fileContent, "file", fileName);

                var response = await _httpClient.PostAsync("http://courseworkformaui.somee.com/api/Image/upload", content);

                if (response.IsSuccessStatusCode)
                {
                    string pathPhoto = "http://courseworkformaui.somee.com/api/Image/upload/" + fileName;
                    var result = await response.Content.ReadAsStringAsync();
                    var uploadResult = JsonConvert.DeserializeObject<UploadResult>(result);
                    return pathPhoto;
                }
                else
                {
                    throw new Exception("Не удалось загрузить фото");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при загрузке фото: {ex.Message}");
            }


        }

        public async Task DeletePhoto(string fileName)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"http://courseworkformaui.somee.com/api/Image/delete?fileName={fileName}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Не удалось удалить фото");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при удалении фото: {ex.Message}");
            }
        }

        public async Task<PhotoResponse> GetPhotoByName(string fileName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://courseworkformaui.somee.com/api/Image/getImageByName?fileName={fileName}");
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(responseBody);
                    string photoContent = json["content"].ToString();
                    return new PhotoResponse { Content = photoContent };
                }
                else
                {
                    throw new Exception("Не удалось получить фото");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении фото: {ex.Message}");
            }
        }
 
        private class UploadResult
        {
            public string FilePath { get; set; }
        }
    }
}

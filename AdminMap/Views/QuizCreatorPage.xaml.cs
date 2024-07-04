using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using AdminMap.Models;
using Microsoft.Maui.Controls;

namespace AdminMap.Views
{
    public partial class QuizCreatorPage : ContentPage
    {
        public QuizCreatorPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadQuizzes();
        }

        private async void LoadQuizzes()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("http://courseworkformaui.somee.com/api/quiz/all");
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var quizzes = JsonSerializer.Deserialize<List<QuizFile>>(json, options);

                    if (quizzes == null || quizzes.Count == 0)
                    {
                        await DisplayAlert("Ошибка", "Не удалось загрузить квизы или квизов нет", "OK");
                        return;
                    }

                    QuizListView.ItemsSource = quizzes;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка при загрузке тестов: {ex.Message}", "OK");
            }
        }

        private async void OnQuizSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is QuizFile selectedQuiz)
            {
                await Navigation.PushAsync(new EditQuizPage(selectedQuiz.FilePath, selectedQuiz.FileName));
            }
        }

        private async void OnDeleteQuizClicked(object sender, EventArgs e)
        {
            var button = sender as Button;
            var filePath = button.CommandParameter as string;

            var confirm = await DisplayAlert("Подтверждение", "Вы уверены, что хотите удалить этот тест?", "Да", "Нет");

            if (confirm)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        var response = await client.DeleteAsync($"http://courseworkformaui.somee.com/api/quiz/delete?filePath={filePath}");
                        response.EnsureSuccessStatusCode();

                        await DisplayAlert("Успех", "Тест успешно удален.", "OK");
                        LoadQuizzes(); 
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ошибка", $"Произошла ошибка при удалении теста: {ex.Message}", "OK");
                }
            }
        }

        private async void OnAddNewQuizClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditQuizPage(null, null));
        }
    }
}

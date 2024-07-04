using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Xml.Linq;
using Microsoft.Maui.Storage;
using MauiC_.Maui.Models;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Net.Http;

namespace MauiC_.Maui.Views
{
    public partial class QuizPage : ContentPage
    {
        private List<Question> _questions;
        private int _currentQuestionIndex = 0;
        private System.Timers.Timer _timer;
        private static readonly HttpClient _httpClient = new HttpClient();
        private int _timeLeft;
        private double _correctAnswersCount = 0; 
        private DateTime _nextUpdate;
        private bool _quizCompletedToday;

        public QuizPage()
        {
            InitializeComponent();
            LoadRandomQuizFromServer();
            LoadQuizState();
            CalculateTimeToNextUpdate();
            UpdateStartButton();
        }

        private async void LoadRandomQuizFromServer()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("http://courseworkformaui.somee.com/api/quiz/random");
                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();

                    JObject jsonResponse = JObject.Parse(responseContent);
                    string xmlContent = jsonResponse["content"].ToString();

                    xmlContent = xmlContent.Trim();

                    var doc = XDocument.Parse(xmlContent);
                    _questions = new List<Question>();

                    foreach (var q in doc.Descendants("Question"))
                    {
                        var questionText = q.Element("Text")?.Value;
                        var options = new List<Option>();

                        foreach (var o in q.Element("Options").Descendants("Option"))
                        {
                            options.Add(new Option
                            {
                                Text = o.Value,
                                IsCorrect = bool.Parse(o.Attribute("Correct").Value)
                            });
                        }

                        _questions.Add(new Question { Text = questionText, Options = options });
                    }

                    _questions = _questions.OrderBy(x => Guid.NewGuid()).ToList();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка при загрузке викторины: {ex.Message}", "OK");
            }
        }

        private void LoadQuizState()
        {
            var lastCompletedDate = Preferences.Get("LastCompletedDate", DateTime.MinValue);
            _quizCompletedToday = lastCompletedDate.Date == DateTime.Now.Date;
        }

        private void SaveQuizState()
        {
            Preferences.Set("LastCompletedDate", DateTime.Now);
            _quizCompletedToday = true;
        }

        private void CalculateTimeToNextUpdate()
        {
            var now = DateTime.Now;
            var moscowTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(now, "Russian Standard Time");
            var nextUpdate = new DateTime(moscowTime.Year, moscowTime.Month, moscowTime.Day, 0, 0, 0).AddDays(1);

            _timeLeft = (int)(nextUpdate - moscowTime).TotalSeconds;
        }

        private void UpdateStartButton()
        {
            if (_quizCompletedToday)
            {
                _timer = new System.Timers.Timer(1000);
                _timer.Elapsed += OnTimerElapsed;
                _timer.Start();
            }
            else
            {
                StartButton.Text = "Начать викторину!";
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _timeLeft--;

            if (_timeLeft <= 0)
            {
                _timer.Stop();
                Device.BeginInvokeOnMainThread(() =>
                {
                    StartButton.Text = "Начать викторину!";
                    _quizCompletedToday = false;
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var timeSpan = TimeSpan.FromSeconds(_timeLeft);
                    StartButton.Text = $"Обновление через: {timeSpan:hh\\:mm\\:ss}";
                });
            }
        }

        private void StartButton_Clicked(object sender, EventArgs e)
        {
            if (_quizCompletedToday)
            {
                DisplayAlert("Подождите", "Вы уже прошли викторину сегодня. Приходите завтра для новой викторины.", "OK");
                return;
            }

            WelcomeLabel.IsVisible = false;
            image.IsVisible = false;
            StartButton.IsVisible = false;
            TimerLabel.IsVisible = true;

            DisplayCurrentQuestion();
        }

        private async void DisplayCurrentQuestion()
        {
            if (_questions == null || _questions.Count == 0)
                return;

            if (_currentQuestionIndex >= _questions.Count)
            {
                DisplayAlert("Поздравляем!", $"Вы прошли все вопросы викторины.\nПравильных ответов: {_correctAnswersCount} из {_questions.Count}.", "OK");
                SaveQuizState();
                await UpdateUserProgressOnServer(App.user.UserId, _correctAnswersCount);
                _correctAnswersCount = 0; 
                WelcomeLabel.IsVisible = true;
                StartButton.IsVisible = true;
                image.IsVisible = true;
                TimerLabel.IsVisible = false;
                QuestionLabel.IsVisible = false;
                OptionsLayout.IsVisible = false;
                UpdateStartButton();
                return;
            }

            var currentQuestion = _questions[_currentQuestionIndex];
            QuestionLabel.Text = currentQuestion.Text;

            OptionsLayout.Children.Clear();
            foreach (var option in currentQuestion.Options)
            {
                var button = new Button { Text = option.Text, Margin = new Thickness(0, 10, 0, 0), WidthRequest = 300, FontSize = 16 };
                button.Clicked += OnOptionClicked;
                OptionsLayout.Children.Add(button);
            }

            QuestionLabel.IsVisible = true;
            OptionsLayout.IsVisible = true;
        }

        private async Task UpdateUserProgressOnServer(int userId, double progress)
        {
            string url = $"http://courseworkformaui.somee.com/api/User/{userId}/addLevelProgress/{progress}";
            var response = await _httpClient.PutAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                Debug.Write("Failed to update view count on the server.");
            }
        }

        private void OnOptionClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                var selectedOption = button.Text;
                var currentQuestion = _questions[_currentQuestionIndex];

                var isCorrect = currentQuestion.Options
                    .FirstOrDefault(o => o.Text == selectedOption)?.IsCorrect ?? false;

                button.BackgroundColor = isCorrect ? Color.FromHex("#008000") : Color.FromHex("#FF0000");
                button.TextColor = Color.FromHex("#FFFFFF");

                if (isCorrect)
                {
                    _correctAnswersCount++;
                }

                Device.StartTimer(TimeSpan.FromSeconds(0.3), () =>
                {
                    _currentQuestionIndex++;
                    DisplayCurrentQuestion();
                    return false;
                });
            }
        }
    }
}

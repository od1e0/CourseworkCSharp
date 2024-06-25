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

namespace MauiC_.Maui.Views
{
    public partial class QuizPage : ContentPage
    {
        private List<Question> _questions;
        private int _currentQuestionIndex = 0;
        private System.Timers.Timer _timer;
        private int _timeLeft; 
        private DateTime _nextUpdate;
        private bool _quizCompletedToday = false;

        public QuizPage()
        {
            InitializeComponent();
            LoadRandomQuiz();
            LoadQuizState();
            CalculateTimeToNextUpdate();
            UpdateStartButton();
        }

        private void LoadRandomQuiz()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames()
                                        .Where(r => r.EndsWith(".xml") && r.Contains("Resources.Quiz"))
                                        .ToList();

            if (resourceNames.Count == 0)
            {
                DisplayAlert("Ошибка", "Не удалось найти файлы викторин.", "OK");
                return;
            }

            var random = new Random();
            var randomQuizResource = resourceNames[random.Next(resourceNames.Count)];

            using (var stream = assembly.GetManifestResourceStream(randomQuizResource))
            {
                var doc = XDocument.Load(stream);
                _questions = new List<Question>();

                foreach (var q in doc.Descendants("Question"))
                {
                    var questionText = q.Element("Text")?.Value;
                    var options = new List<Option>();

                    foreach (var o in q.Descendants("Option"))
                    {
                        options.Add(new Option
                        {
                            Text = o.Value,
                            IsCorrect = bool.Parse(o.Attribute("Correct").Value)
                        });
                    }

                    _questions.Add(new Question { Text = questionText, Options = options });
                }
            }
            _questions = _questions.OrderBy(x => random.Next()).ToList();
        }

        private void LoadQuizState()
        {
            var lastCompletedDate = Preferences.Get("LastCompletedDate", DateTime.MinValue);
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

            _nextUpdate = new DateTime(moscowTime.Year, moscowTime.Month, moscowTime.Day, 0, 0, 0).AddDays(1);

            if (moscowTime.Hour >= 0)
            {
                _nextUpdate = _nextUpdate.AddDays(1);
            }

            _timeLeft = (int)(_nextUpdate - moscowTime).TotalSeconds;
        }

        private void UpdateStartButton()
        {
            if (_quizCompletedToday)
            {
                _timer = new System.Timers.Timer(1000); // Таймер на 1 секунду
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

        private void DisplayCurrentQuestion()
        {
            if (_questions == null || _questions.Count == 0)
                return;

            if (_currentQuestionIndex >= _questions.Count)
            {
                DisplayAlert("Поздравляем!", "Вы прошли все вопросы викторины.", "OK");
                SaveQuizState();
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

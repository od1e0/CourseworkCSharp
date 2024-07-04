using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace AdminMap.Views;

public partial class EditQuizPage : ContentPage
{
    private string _filePath;
    private string _fileName;
    private const string ApiUrl = "http://courseworkformaui.somee.com/api/quiz";

    public EditQuizPage(string filePath, string fileName)
    {
        InitializeComponent();
        _filePath = filePath;
        _fileName = fileName;
        if (!string.IsNullOrEmpty(filePath))
        {
            LoadQuiz();
        }
    }

    private async void LoadQuiz()
    {
        try
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"{ApiUrl}/getQuizByName?fileName={_fileName}");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                var json = JsonSerializer.Deserialize<JsonElement>(result);

                var content = json.GetProperty("content").GetString();

                await DisplayAlert("Debug", $"Downloaded content: {content}", "OK");

                LoadQuizFromContent(content);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Произошла ошибка при загрузке теста: {ex.Message}", "OK");
        }
    }

    private void LoadQuizFromContent(string content)
    {
        try
        {
            var document = XDocument.Parse(content);
            var questions = document.Root.Elements("Question");

            foreach (var question in questions)
            {
                var questionLayout = new StackLayout { Padding = new Thickness(0, 10) };

                var questionText = question.Element("Text")?.Value ?? string.Empty;
                var questionLabel = new Label { Text = $"Вопрос {QuestionsStack.Children.Count + 1}:" };
                var questionEntry = new Entry { Text = questionText };

                questionLayout.Children.Add(questionLabel);
                questionLayout.Children.Add(questionEntry);

                var optionsStack = new StackLayout();
                var options = question.Element("Options").Elements("Option");

                foreach (var option in options)
                {
                    var optionText = option?.Value ?? string.Empty;
                    var correctValue = option?.Attribute("Correct")?.Value ?? "false";
                    var isCorrect = bool.Parse(correctValue);

                    var optionLayout = new StackLayout { Orientation = StackOrientation.Horizontal };

                    var optionEntry = new Entry { Text = optionText };
                    var correctSwitch = new Switch { IsToggled = isCorrect, HorizontalOptions = LayoutOptions.EndAndExpand };

                    optionLayout.Children.Add(optionEntry);
                    optionLayout.Children.Add(new Label { Text = "Правильный", VerticalOptions = LayoutOptions.Center });
                    optionLayout.Children.Add(correctSwitch);

                    optionsStack.Children.Add(optionLayout);
                }

                questionLayout.Children.Add(optionsStack);

                QuestionsStack.Children.Add(questionLayout);
            }
        }
        catch (Exception ex)
        {
            Device.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert("Ошибка", $"Произошла ошибка при обработке контента теста: {ex.Message}", "OK");
            });
        }
    }

    private void OnAddQuestionClicked(object sender, EventArgs e)
    {
        var questionLayout = new StackLayout { Padding = new Thickness(0, 10) };

        var questionLabel = new Label
        {
            Text = $"Вопрос {QuestionsStack.Children.Count + 1}:",
            TextColor = Color.FromHex("#000"), 
            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            FontAttributes = FontAttributes.Bold
        };

        var questionEntry = new Entry
        {
            Placeholder = "Введите текст вопроса",
            BackgroundColor = Color.FromHex("#c5e079"),
            TextColor = Color.FromHex("#000"),
            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Entry)), // Размер шрифта
        };

        questionLayout.Children.Add(questionLabel);
        questionLayout.Children.Add(questionEntry);

        var optionsStack = new StackLayout();
        for (int i = 0; i < 4; i++)
        {
            var optionLayout = new StackLayout { Orientation = StackOrientation.Horizontal };

            var optionEntry = new Entry
            {
                Placeholder = $"Вариант ответа {i + 1}",
                BackgroundColor = Color.FromHex("#c5e079"), 
                TextColor = Color.FromHex("#000"), 
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Entry)),
            };

            var correctSwitch = new Switch
            {
                HorizontalOptions = LayoutOptions.EndAndExpand,
                ThumbColor = Color.FromHex("#7380fa"), 
                OnColor = Color.FromHex("#1123bf"), 
                IsToggled = false 
            };

            optionLayout.Children.Add(optionEntry);
            optionLayout.Children.Add(new Label
            {
                Text = "Правильный",
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.FromHex("#000"),
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)) 
            });
            optionLayout.Children.Add(correctSwitch);

            optionsStack.Children.Add(optionLayout);
        }

        questionLayout.Children.Add(optionsStack);

        QuestionsStack.Children.Add(questionLayout);
    }


    private async void OnSaveQuizClicked(object sender, EventArgs e)
    {
        if (!ValidateQuiz())
        {
            return;
        }

        try
        {
            var quizName = QuizNameEntry.Text?.Trim();
            if (string.IsNullOrWhiteSpace(quizName))
            {
                quizName = Guid.NewGuid().ToString();
            }

            var tempFilePath = Path.Combine(FileSystem.CacheDirectory, $"{quizName}.xml");

            var fileExists = await CheckIfFileExistsOnServer(tempFilePath);

            if (fileExists)
            {
                var confirm = await DisplayAlert("Подтверждение", "Файл с таким именем уже существует на сервере. Перезаписать его?", "Да", "Нет");

                if (!confirm)
                {
                    return;
                }
            }

            var quiz = new XElement("Quiz");

            foreach (var questionLayout in QuestionsStack.Children)
            {
                var stackLayout = questionLayout as StackLayout;
                var questionEntry = stackLayout.Children[1] as Entry;

                var questionElement = new XElement("Question",
                    new XElement("Text", questionEntry.Text),
                    new XElement("Options")
                );

                var optionsStack = stackLayout.Children[2] as StackLayout;

                foreach (var optionLayout in optionsStack.Children)
                {
                    var optionStack = optionLayout as StackLayout;
                    var optionEntry = optionStack.Children[0] as Entry;
                    var correctSwitch = optionStack.Children[2] as Switch;

                    var optionElement = new XElement("Option",
                        new XAttribute("Correct", correctSwitch.IsToggled.ToString().ToLower()),
                        optionEntry.Text
                    );

                    questionElement.Element("Options").Add(optionElement);
                }

                quiz.Add(questionElement);
            }

            var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), quiz);

            using (var writer = new StreamWriter(tempFilePath, false, Encoding.UTF8))
            {
                document.Save(writer);
            }

            using (var client = new HttpClient())
            {
                var form = new MultipartFormDataContent();
                form.Add(new StreamContent(new FileStream(tempFilePath, FileMode.Open)), "file", Path.GetFileName(tempFilePath));

                var response = await client.PostAsync($"{ApiUrl}/upload", form);
                response.EnsureSuccessStatusCode();
            }

            await DisplayAlert("Успех", "Тест успешно сохранен на сервере.", "OK");
            await Navigation.PopAsync();

            _fileName = Path.GetFileName(tempFilePath);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Произошла ошибка при сохранении теста: {ex.Message}", "OK");
        }
    }

    private async Task<bool> CheckIfFileExistsOnServer(string filePath)
    {
        try
        {
            using (var client = new HttpClient())
            {
                var encodedFileName = Uri.EscapeDataString(Path.GetFileName(filePath));
                var response = await client.GetAsync($"{ApiUrl}/checkFileExists?fileName={encodedFileName}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return result.Contains("Файл существует"); // Проверяем содержимое ответа, можно сделать более сложную проверку
                }
                else
                {
                    // В случае ошибки возвращаем false или обрабатываем иным способом
                    await DisplayAlert("Ошибка", $"Запрос завершился с ошибкой: {response.StatusCode}", "OK");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Произошла ошибка при проверке существования файла на сервере: {ex.Message}", "OK");
            return false;
        }
    }



    private bool ValidateQuiz()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(QuizNameEntry.Text))
        {
            QuizNameErrorLabel.Text = "Название теста не может быть пустым";
            QuizNameErrorLabel.IsVisible = true;
            isValid = false;
        }
        else if (QuestionsStack.Children.Count == 0)
        {
            QuizNameErrorLabel.Text = "Тест должен содержать хотя бы один вопрос";
            QuizNameErrorLabel.IsVisible = true;
            isValid = false;
        }
        else
        {
            QuizNameErrorLabel.IsVisible = false;
        }

        return isValid;
    }
}

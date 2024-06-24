namespace MauiC_.Maui.Components
{
    public partial class AchievementCard : ContentView
    {
        public static readonly BindableProperty AchievementImageSourceProperty = BindableProperty.Create(
            nameof(AchievementImageSource), typeof(string), typeof(AchievementCard), default(string));

        public static readonly BindableProperty AchievementTextContentProperty = BindableProperty.Create(
            nameof(AchievementTextContent), typeof(string), typeof(AchievementCard), default(string));

        public AchievementCard()
        {
            InitializeComponent();
        }

        public string AchievementImageSource
        {
            get => (string)GetValue(AchievementImageSourceProperty);
            set => SetValue(AchievementImageSourceProperty, value);
        }

        public string AchievementTextContent
        {
            get => (string)GetValue(AchievementTextContentProperty);
            set => SetValue(AchievementTextContentProperty, value);
        }
    }
}

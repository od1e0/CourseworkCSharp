using MauiC_.Maui.Models;

namespace MauiC_.Maui
{
    public partial class App : Application
    {
        public static User user;
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}

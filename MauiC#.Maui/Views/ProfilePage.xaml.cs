using MauiC_.Maui.Models;
using System.Collections.ObjectModel;

namespace MauiC_.Maui.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ObservableCollection<Achievement> Achievements { get; set; }

        public ProfilePage()
        {
            InitializeComponent();
        }
    }
}

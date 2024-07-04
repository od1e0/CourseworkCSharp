using AdminMap.Services;
using AdminMap.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AdminMap.Views
{
    public partial class AdminPage : ContentPage
    {
        private readonly IAttractionService _attractionService;

        public ObservableCollection<Attraction> Attractions { get; set; }

        public AdminPage()
        {
            InitializeComponent();

            _attractionService = new AttractionService();
            Attractions = new ObservableCollection<Attraction>();
            AttractionsListView.ItemsSource = Attractions;

            LoadAttractions();
        }

        private async void LoadAttractions()
        {
            var attractions = await _attractionService.GetAllAttractions();
            Attractions = new ObservableCollection<Attraction>(attractions);
            AttractionsListView.ItemsSource = Attractions;
        }

        private async void OnAddAttractionClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AttractionDetailPage(new Attraction()));
        }

        private async void OnAttractionSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var attraction = e.SelectedItem as Attraction;
                await Navigation.PushAsync(new AttractionDetailPage(attraction));
                ((ListView)sender).SelectedItem = null;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadAttractions();
        }
    }
}

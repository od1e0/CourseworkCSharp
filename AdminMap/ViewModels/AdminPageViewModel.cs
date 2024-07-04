using AdminMap.Models;
using AdminMap.Services;
using AdminMap.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AdminMap.ViewModels
{
    public partial class AdminPageViewModel : ObservableObject
    {
        private readonly IAttractionService _attractionService;
        private ObservableCollection<Attraction> _attractions;

        public ObservableCollection<Attraction> Attractions
        {
            get => _attractions;
            set => SetProperty(ref _attractions, value);
        }

        public IAsyncRelayCommand AddAttractionCommand { get; }
        public IAsyncRelayCommand<Attraction> SelectAttractionCommand { get; }

        public AdminPageViewModel()
        {
            _attractionService = new AttractionService();
            Attractions = new ObservableCollection<Attraction>();

            AddAttractionCommand = new AsyncRelayCommand(AddAttraction);
            SelectAttractionCommand = new AsyncRelayCommand<Attraction>(SelectAttraction);

            LoadAttractions();
        }

        private async Task LoadAttractions()
        {
            var attractions = await _attractionService.GetAllAttractions();
            Attractions.Clear();
            foreach (var attraction in attractions)
            {
                Attractions.Add(attraction);
            }
        }

        private async Task AddAttraction()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new AttractionDetailPage(new Attraction()));
        }

        [ObservableProperty]
        private Attraction _selectedAttraction;


        private async Task SelectAttraction(Attraction attraction)
        {
            SelectedAttraction = attraction;
            await Application.Current.MainPage.Navigation.PushAsync(new AttractionDetailPage(attraction));
        }
    }
}

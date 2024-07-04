using Maui.GoogleMaps;
using MauiC_.Maui.Models;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MauiC_.Maui.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private List<Attraction> _attractions;

        public HomePage()
        {
            InitializeComponent();
            InitializeMap();
        }

        private async void InitializeMap()
        {
            var belarusCenter = new Position(53.7098, 27.9534);

            mymap.MoveToRegion(MapSpan.FromCenterAndRadius(belarusCenter, Distance.FromKilometers(100)));
            await LoadPinsFromDatabase();
        }

        private async Task LoadPinsFromDatabase()
        {
            try
            {
                _attractions = await _httpClient.GetFromJsonAsync<List<Attraction>>("http://courseworkformaui.somee.com/api/Attractions");
                if (_attractions != null)
                {
                    foreach (var attraction in _attractions)
                    {
                        var pin = new Pin
                        {
                            Label = attraction.Label,
                            Address = attraction.Address,
                            Type = PinType.Place,
                            Position = new Position(attraction.Latitude, attraction.Longitude)
                        };
                        mymap.InfoWindowClicked += OnInfoWindowClicked;
                        mymap.Pins.Add(pin);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load pins: {ex.Message}", "OK");
            }
        }

        private async void OnInfoWindowClicked(object sender, InfoWindowClickedEventArgs e)
        {
            try
            {
                var pin = e.Pin;
                var attraction = _attractions?.Find(a => a.Label == pin.Label && a.Address == pin.Address && a.Latitude == pin.Position.Latitude && a.Longitude == pin.Position.Longitude);
                if (attraction != null)
                {
                    await Navigation.PushAsync(new AttractionPage(attraction));
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Navigation error: {ex.Message}", "OK");
            }
        }
    }
}

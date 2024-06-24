using Maui.GoogleMaps;
using Microsoft.Maui.Controls;
using System;

namespace MauiC_.Maui.Views;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
        InitializeMap();

    }

    private void InitializeMap()
    {
        var belarusCenter = new Position(53.7098, 27.9534);

        // Переместите карту на центр Беларуси
        mymap.MoveToRegion(MapSpan.FromCenterAndRadius(belarusCenter, Distance.FromKilometers(100)));

        // Добавьте маркер на центр Беларуси
        var pin = new Pin
        {
            Label = "Центр Беларуси",
            Address = "Беларусь",
            Type = PinType.Place,
            Position = belarusCenter
        };
        mymap.Pins.Add(pin);
        mymap.InfoWindowClicked += OnInfoWindowClicked;
    }

    private async void OnInfoWindowClicked(object sender, InfoWindowClickedEventArgs e)
    {
        // Навигация на страницу AttractionPage при клике на информационное окно маркера
        await Navigation.PushAsync(new AttractionPage());
    }

    private void Pin_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new AttractionPage());
    }
}
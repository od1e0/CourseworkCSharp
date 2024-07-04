using MauiC_.Maui.Models;
using System.Reflection;

namespace MauiC_.Maui.Views;

public partial class ThreeDModelPage : ContentPage
{
	public ThreeDModelPage(Attraction attraction)
	{
		InitializeComponent();

        WebView webView = new WebView
        {
            VerticalOptions = LayoutOptions.FillAndExpand,
            HorizontalOptions = LayoutOptions.FillAndExpand,
            IsEnabled = false,
        };

        Content = new StackLayout
        {
            Children = { webView }
        };

        webView.Source = new Uri($"{attraction.ThreeModelUrl}");
    }
}
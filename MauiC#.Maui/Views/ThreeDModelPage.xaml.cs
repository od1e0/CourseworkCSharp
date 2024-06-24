using System.Reflection;

namespace MauiC_.Maui.Views;

public partial class ThreeDModelPage : ContentPage
{
	public ThreeDModelPage()
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

        webView.Source = new Uri("https://sketchfab.com/3d-models/732a39cfbe404265be7616618ce0272c");
    }
}
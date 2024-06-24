namespace MauiC_.Maui.Components;

public partial class ARSurface : ContentView
{
    public ARSurface()
    {
        Content = new BoxView
        {
            BackgroundColor = Color.FromHex("#FFFFFF"),
            HorizontalOptions = LayoutOptions.FillAndExpand,
            VerticalOptions = LayoutOptions.FillAndExpand
        };
    }
    public async Task StartSessionAsync()
    {
        // ����� ����� ���� �������� ������ ��� ������ AR ������
        await Task.Delay(2000); // �������� �������������
        await Application.Current.MainPage.DisplayAlert("AR Session", "AR Session started.", "OK");
    }
}
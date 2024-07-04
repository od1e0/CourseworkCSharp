using AdminMap.Services;
using AdminMap.Models;

namespace AdminMap.Views;

public partial class AttractionDetailPage : ContentPage
{
    private readonly IAttractionService _attractionService;
    public Attraction Attraction { get; set; }
    public bool IsEditMode { get; set; }
    public AttractionDetailPage(Attraction attraction)
	{
		InitializeComponent();
        _attractionService = new AttractionService();
        Attraction = attraction;
        IsEditMode = !string.IsNullOrEmpty(attraction.Label);
        BindingContext = this;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (Attraction != null)
        {
            LabelEntry.Text = Attraction.Label;
            AddressEntry.Text = Attraction.Address;
            LatitudeEntry.Text = Attraction.Latitude.ToString();
            LongitudeEntry.Text = Attraction.Longitude.ToString();
            DescriptionEditor.Text = Attraction.Description;
            ImageUrlEntry.Text = Attraction.ImageUrl;
            ThreeModelUrlEntry.Text = Attraction.ThreeModelUrl;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(LabelEntry.Text) ||
            string.IsNullOrWhiteSpace(AddressEntry.Text) ||
            string.IsNullOrWhiteSpace(LatitudeEntry.Text) ||
            string.IsNullOrWhiteSpace(LongitudeEntry.Text) ||
            string.IsNullOrWhiteSpace(DescriptionEditor.Text) ||
            string.IsNullOrWhiteSpace(ImageUrlEntry.Text))
        {
            ErrorLabel.Text = "����������, ��������� ��� ����!";
            ErrorLabel.IsVisible = true;
            return;
        }

        if (!IsValidUrl(ImageUrlEntry.Text) || !IsValidUrl(ThreeModelUrlEntry.Text))
        {
            ErrorLabel.Text = "����������, ������� ���������� URL ��� ����������� ��� 3D ������!";
            ErrorLabel.IsVisible = true;
            return;
        }

        if (!await IsUrlAccessibleAsync(ImageUrlEntry.Text) || !await IsUrlAccessibleAsync(ThreeModelUrlEntry.Text))
        {
            ErrorLabel.Text = "URL ��� ����������� ��� 3D ������ ����������!";
            ErrorLabel.IsVisible = true;
            return;
        }

        ErrorLabel.IsVisible = false;

        Attraction.Label = LabelEntry.Text;
        Attraction.Address = AddressEntry.Text;
        Attraction.Latitude = double.Parse(LatitudeEntry.Text);
        Attraction.Longitude = double.Parse(LongitudeEntry.Text);
        Attraction.Description = DescriptionEditor.Text;
        Attraction.ImageUrl = ImageUrlEntry.Text;
        Attraction.ThreeModelUrl = ThreeModelUrlEntry.Text;

        try
        {
            if (IsEditMode)
            {
                await _attractionService.UpdateAttraction(Attraction);
            }
            else
            {
                await _attractionService.AddAttraction(Attraction);
            }

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("������", $"��������� ������ ��� ����������: {ex.Message}", "OK");
        }
    }

    private async Task<bool> IsUrlAccessibleAsync(string url)
    {
        try
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    private bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) &&
               (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }



    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("�������������", "�� �������, ��� ������ �������?", "��", "���");
        if (confirm)
        {
            await _attractionService.DeleteAttraction(Attraction);
            await Navigation.PopAsync();
        }
    }
}
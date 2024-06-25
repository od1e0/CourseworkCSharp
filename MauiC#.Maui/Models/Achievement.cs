namespace MauiC_.Maui.Models;

public class Achievement
{
    public int AchievementId { get; set; }
    public string AchievementImageSource { get; set; }
    public string AchievementTextContent { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}

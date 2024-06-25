

namespace MauiC_.Maui.Models;

public class User
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public double LevelProgress { get; set; }
    public int AttractionsCount { get; set; }
    public ICollection<Achievement> Achievements { get; set; }
}
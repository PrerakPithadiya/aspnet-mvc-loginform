using System.ComponentModel.DataAnnotations;

namespace loginform_with_database.Models;

public class ProfileModel
{
    public int Id { get; set; }

    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Display(Name = "Display name")]
    [MaxLength(100)]
    public string? DisplayName { get; set; }

    [Display(Name = "Avatar URL")]
    [Url]
    public string? AvatarUrl { get; set; }

    [Display(Name = "Registered At")]
    public DateTime RegisteredAt { get; set; }
}

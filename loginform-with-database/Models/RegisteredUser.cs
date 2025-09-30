using System.ComponentModel.DataAnnotations;

namespace loginform_with_database.Models;

public class RegisteredUser
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MinLength(4)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    // Optional profile fields
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
}

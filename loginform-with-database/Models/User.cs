using System.ComponentModel.DataAnnotations;

namespace loginform_with_database.Models;

public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MinLength(4)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;
}

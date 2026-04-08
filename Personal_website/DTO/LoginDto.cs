using System.ComponentModel.DataAnnotations;

namespace Personal_website.DTO;

public class LoginDto
{
    [Required]
    [StringLength(256)]
    public string Name { get; set; }
    [Required]
    public string Password { get; set; }
}
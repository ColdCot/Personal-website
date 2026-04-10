using System.ComponentModel.DataAnnotations;

namespace Personal_website.DTO;

public class MessageRequestDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string SenderName { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    [StringLength(50, MinimumLength = 2)]
    public string SenderEmail { get; set; } =  string.Empty;
    [Required]
    [StringLength(2000, MinimumLength = 2)]
    public string Text { get; set; } = String.Empty;
}
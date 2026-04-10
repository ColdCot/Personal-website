using System.ComponentModel.DataAnnotations;

namespace Personal_website.DTO;

public class MessageRequestDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string SenderName { get; set; } = string.Empty;
    [Required]
    [EmailAddress]
    public string SenderEmail { get; set; } =  string.Empty;
    [Required]
    [StringLength(2000, MinimumLength = 2)]
    public string Text { get; set; } = String.Empty;
}
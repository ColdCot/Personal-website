using System.ComponentModel.DataAnnotations;

namespace Personal_website.Models;

public class Sender
{
    public int Id { get; set; }
    [Required]
    [StringLength(50)]
    public string Name  { get; set; }
    [Required]
    [StringLength(50)]
    [EmailAddress]
    public string Email { get; set; }
}
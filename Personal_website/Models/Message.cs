using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Identity.Client;

namespace Personal_website.Models;

public class Message
{
    public int id { get; set; }
    public int senderId { get; set; }
    public string text  { get; set; }
}
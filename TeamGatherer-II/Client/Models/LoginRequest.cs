using System.ComponentModel.DataAnnotations;

namespace TeamGatherer.Client.Models;

public record LoginRequest
{
    [Required]
    public string? Login { get; set; }

    [Required]
    public string? Password { get; set; }
}
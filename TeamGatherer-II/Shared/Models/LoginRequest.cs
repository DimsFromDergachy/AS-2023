using System.ComponentModel.DataAnnotations;

namespace TeamGatherer.Shared.Models;

public record LoginRequest
{
    [Required]
    public string Login { get; set; }

    [Required]
    public string Password { get; set; }
}
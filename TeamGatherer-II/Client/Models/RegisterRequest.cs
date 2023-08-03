using System.ComponentModel.DataAnnotations;

namespace TeamGatherer.Client.Models;

public record RegisterRequest
{
    [Required]
    public string Login { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;

    public string Role { get; set; } = "User"; //Добавлять пользователя с ролью админ может только админ
}
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace AS2023Env.Models;

public record RegisterEmployeeRequest
{
    [Required]
    [SwaggerSchema("Идентификатор штатной единицы")]
    public string StaffUnitId { get; init; }
    [Required]
    [SwaggerSchema("Имя нового сотрудника")]
    public string FirstName { get; init; }
    [Required]
    [SwaggerSchema("Фамилия нового сотрудника")]
    public string LastName { get; init; }
    [Required]
    [SwaggerSchema("Электронная почта нового сотрудника")]
    public string Email { get; init; }
}
using System.ComponentModel.DataAnnotations;

namespace AS2023Env.Models;

public record RegisterEmployeeRequest(
    [Required] string StaffUnitId,
    [Required] string FirstName,
    [Required] string LastName,
    [Required] string Email
);
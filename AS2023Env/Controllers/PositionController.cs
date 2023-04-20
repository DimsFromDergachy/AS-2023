using AS2023Env.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace AS2023Env.Controllers;

[Route("/positions")]
[SwaggerTag("Должности")]
public class PositionController : BaseController<Position>
{
    public PositionController(IStorage<Position> storage, AuthService authService) : base(storage, authService) { }
}
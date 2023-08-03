using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

using TeamGatherer.Shared.Models;
using TeamGatherer.Server.Data;
using TeamGatherer.Server.Models;

namespace TeamGatherer.Server.Adapters;

public interface IAuthenticationAdapter
{
    Task<string> RegisterUser(RegisterRequest request);
    Task<string> Login(LoginRequest request);
    public IEnumerable<Claim> ParseClaimsFromJwt(string jwt);
}

public class AuthenticationAdapter : IAuthenticationAdapter
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;

    public AuthenticationAdapter(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<string> RegisterUser(RegisterRequest request)
    {
        if (_context.Users.Any(it => it.Login == request.Login))
            throw new Exception($"User with login {request.Login} already exist!");

        //Проверять пароль на сложность.

        var user = new User
        {
            Login = request.Login,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role.ToString()
        };

        try
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return GetToken(user);
        }
        catch (Exception exception)
        {
            throw new Exception($"Register failed: {exception.Message}");
        }
    }

    public async Task<string> Login(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(user => user.Login == request.Login);

        if (user == null)
            throw new Exception("User not found");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new Exception("Wrong password!");

        return GetToken(user);
    }

    private string GetToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Login),
            new(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));

        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: cred);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];

        var jsonBytes = ParseBase64WithoutPadding(payload);

        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        if (keyValuePairs != null)
            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty)));
        return claims;
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }

        return Convert.FromBase64String(base64);
    }
}
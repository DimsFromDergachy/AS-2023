namespace AS2023Env;

public class AuthService
{
    private readonly string _token;
    
    public AuthService()
    {
        string credentials = Environment.GetEnvironmentVariable("AS23_CREDENTIALS")
                             ?? throw new ArgumentException("No AS23_CREDENTIALS environment variable");
        
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(credentials);
        _token = Convert.ToBase64String(bytes);
    }

    public bool Check(string header)
    {
        return header == $"Basic {_token}";
    }
}
namespace NetflixClone.DTOs;

public class LoginDto
{
    public string? Identifier { get; set; } // email OR phone

    public string? Password { get; set; }
}
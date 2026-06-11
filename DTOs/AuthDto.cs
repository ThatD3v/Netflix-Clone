using System.ComponentModel.DataAnnotations;

namespace NetflixClone.DTOs
{
    public class CheckUserRequest
    {
        [Required]
        public string Identifier { get; set; } = string.Empty;
    }

    public class CheckUserResponse
    {
        public bool Exists { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class LoginRequest
    {
        [Required]
        public string Identifier { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }

    public class RegisterRequest
    {
        [Required]
        public string Identifier { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;


    }

    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public UserDto? User { get; set; }
        public bool HasSubscription { get; set; }        
        public SubscriptionResponse? Subscription { get; set; }  
        public bool NeedsSubscription { get; set; }

    }


    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }

    public class RefreshTokenRequest
    {
        //[Required]
        //public string AccessToken { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
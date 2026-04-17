using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace NetflixClone.Models;

public class User
{
   
    public int Id { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }
    public bool IsSubscribed { get; set; } = false;
    public int? PlanId { get; set;  }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public ICollection<Profile>? profiles { get; set; } = new List<Profile>();

}
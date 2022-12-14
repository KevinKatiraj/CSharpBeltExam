using System.ComponentModel.DataAnnotations;
namespace CSharpBeltKevin.Models;
#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations.Schema;

public class User

{
    [Key]
    public int UserId { get; set; }

    [Required]
    [MinLength(2,ErrorMessage = "First name must be 2 characters or longer!")]
    public string FirstName { get; set;}

    [Required]
    public string Email { get; set; }

    [DataType(DataType.Password)]
    [Required]
    [MinLength(8, ErrorMessage = "Password must be 8 characters or longer!")]
    public string Password { get; set; }

    public string Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public List<Request> Requests {get;set;} = new List<Request>();

    [NotMapped]
    [Compare("Password")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}


public class LoginUser
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}

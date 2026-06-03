using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndividualWorkAPI.Models;

public class Login
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int loginId { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    
    [Required]
    [ForeignKey("User")]
    public int userId { get; set; }
    public User User { get; set; }
    
}
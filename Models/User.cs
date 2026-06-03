using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using IndividualWorkAPI.CustomAttributes;

namespace IndividualWorkAPI.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int userId { get; set; }
    public string fullname { get; set; }
    public string email { get; set; }
    
    [DateOfBirth]
    public DateOnly? dateOfBirth { get; set; }
   
    
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
    public string channelName { get; set; }
    
    
    [Required]
    [ForeignKey("Role")]
    public int roleId { get; set; }
    public Role Role { get; set; }
    
    //public ICollection<Video> Videos { get; set; } = new List<Video>();
    //public ICollection<Comment> Comments { get; set; } = new List<Comment>();
   // public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    
}
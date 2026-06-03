using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndividualWorkAPI.Models;

public class Session
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int sessionId { get; set; }
    public string token { get; set; }
    
    [ForeignKey("User")]
    public int userId { get; set; }
    public User User { get; set; }
}
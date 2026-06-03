using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;

namespace IndividualWorkAPI.Models;

[Index(nameof(userId), nameof(videoId),  IsUnique = true)]
public class Reaction
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int reactionId { get; set; }
    public ReactionType reactionType { get; set; }
    
    [Required]
    [ForeignKey("User")]
    public int userId { get; set; }
    public User User { get; set; }
    
    [Required]
    [ForeignKey("Video")]
    public int? videoId { get; set; }
    public Video video { get; set; }
    
    [ForeignKey("Comment")]
    public int? commentId { get; set; }
    public Comment comment { get; set; }
}

public enum ReactionType
{
    Like,
    Dislike,
}
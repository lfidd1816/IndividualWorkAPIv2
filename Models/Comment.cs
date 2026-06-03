using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndividualWorkAPI.Models;

public class Comment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int commentId { get; set; }
    public string commentText { get; set; }
    public DateTime createDate { get; set; }

    [Required]
    [ForeignKey("User")]
    public int userId { get; set; }
    public User User { get; set; }
    
    [Required]
    [ForeignKey("Video")]
    public int videoId { get; set; }
    public Video Video { get; set; }
    
    [Required]
    [ForeignKey("ParentComment")]
    public int? parentCommentId { get; set; }
    public Comment ParentComment { get; set; }
    
    //public ICollection<Comment> Replies { get; set; } = new List<Comment>();
}
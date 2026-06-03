using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Channels;

namespace IndividualWorkAPI.Models;

public class Video
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int videoId { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public bool privacyStatus { get; set; }
    public string thumbnailUrl { get; set; }
    public string videoUrl { get; set; }
    public DateTime publishDate { get; set; }
    public DateTime updateDate { get; set; }
    public string s3Key { get; set; }
    
    
    [ForeignKey("Channel")]
    public int userId { get; set; }
    public User user { get; set; }
    
    //public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    //public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
}
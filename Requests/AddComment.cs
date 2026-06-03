using IndividualWorkAPI.Models;

namespace IndividualWorkAPI.Requests;

public class AddComment
{
    public string commentText { get; set; }
    public int videoId { get; set; }
    public int parentCommentId { get; set; }
}
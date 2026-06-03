namespace IndividualWorkAPI.Requests;

public class UpdateComment
{
    public int commentId { get; set; }
    public int videoId { get; set; }
    public string commentText { get; set; }
}
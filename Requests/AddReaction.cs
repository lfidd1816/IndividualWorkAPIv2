using IndividualWorkAPI.Models;

namespace IndividualWorkAPI.Requests;

public class AddReaction
{
    public int reactionId { get; set; }
    public int videoId { get; set; }
    public int commentId { get; set; }
    public ReactionType reactionType { get; set; }
}
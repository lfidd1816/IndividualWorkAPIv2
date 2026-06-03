using IndividualWorkAPI.Models;
using IndividualWorkAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace IndividualWorkAPI.Interfaces;

public interface IVideoService
{
    //video
    Task<IActionResult> GetVideoAsync(int id);
    Task<IActionResult> GetAllVideosAsync();
    Task<IActionResult> GetMyVideosAsync(string authorization);
    Task<IActionResult> GetUserVideosAsync(int userId);
    Task<IActionResult> InitUploadAsync(string authorization, string title, string description, long fileSize, string mimeType, bool privacyStatus);
    Task<IActionResult> ConfirmUploadAsync(int videoId, string authorization);
    Task<IActionResult> UpdateVideoAsync(UpdateVideo updatevideo, string authorization);
    Task<IActionResult> DeleteVideoAsync(int videoId, string authorization);
    
    //reactions
    Task<IActionResult> AddNewReaction(AddReaction addreaction, string authorization);
    Task<IActionResult> SwapReactionsAsync(int reactionId, string authorization);
    Task<IActionResult> DeleteReactionAsync(int reactionId, string authorization);

    //comments
    Task<IActionResult> AddNewComment(AddComment addComment, string authorization);
    Task<IActionResult> UpdateComment(UpdateComment updateComment, string authorization);
    Task<IActionResult> DeleteCommentAsync(int commentId, string authorization);
}
using IndividualWorkAPI.CustomAttributes;
using IndividualWorkAPI.Interfaces;
using IndividualWorkAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace IndividualWorkAPI.Controllers;


[Controller]
[Route("api/[controller]")]
public class VideoController : ControllerBase
{
    private readonly IVideoService _videoService;

    public VideoController(IVideoService videoService)
    {
        _videoService = videoService;
    }
    
    // ==== VIDEO ======
    
    [HttpGet("GetVideo")]
    public async Task<IActionResult> GetVideoAsync(int videoId) => await _videoService.GetVideoAsync(videoId); 
    
    [HttpGet("GetUserVideos")]
    public async Task<IActionResult> GetUserVideosAsync(int userId) => await _videoService.GetUserVideosAsync(userId);
    
    [HttpGet("GetAllVideos")]
    public async Task<IActionResult> GetAllVideosAsync() => await _videoService.GetAllVideosAsync();
    
    [Role([1, 2])]
    [HttpGet("GetMyVideos")]
    public async Task<IActionResult> GetMyVideosAsync([FromHeader] string authorization) => await _videoService.GetMyVideosAsync(authorization);

    [Role([1, 2])]
    [HttpPost("InitUpload")]
    public async Task<IActionResult> InitUpload([FromBody] InitUploadRequest req, [FromHeader] string authorization) => await _videoService.InitUploadAsync(authorization, req.title, req.description, req.fileSize, req.mimeType, req.privacyStatus);
    
    [Role([1, 2])]
    [HttpPost("UpdateVideo")]
    public async Task<IActionResult>  UpdateVideoAsync([FromBody] UpdateVideo updateVideo, [FromHeader] string authorization) => await _videoService.UpdateVideoAsync(updateVideo, authorization);
    
    [Role([1, 2])]
    [HttpPost("DeleteVideo")]
    public async Task<IActionResult> DeleteVideoAsync([FromBody] int videoId, [FromHeader] string authorization) => await _videoService.DeleteVideoAsync(videoId, authorization);
    
    
    // ====== REACTIONS ==========
    
    [Role([1, 2])]
    [HttpPost("AddNewReaction")]
    public async Task<IActionResult> AddNewReaction([FromBody]AddReaction addreaction, [FromHeader] string authorization) => await _videoService.AddNewReaction(addreaction, authorization);
    
    [Role([1, 2])]
    [HttpPost("SwapReaction")]
    public async Task<IActionResult> SwapReactionASync([FromBody] int reactionId, [FromHeader] string authorization) => await _videoService.SwapReactionsAsync(reactionId, authorization);
    
    [Role([1, 2])]
    [HttpPost("RemoveReaction")]
    public async Task<IActionResult> DeleteReactionAsync([FromBody]int reactionId, [FromHeader] string authorization) => await _videoService.DeleteReactionAsync(reactionId, authorization);
    
    // ====== COMMENTS ===========
    
    [Role([1, 2])]
    [HttpPost("AddNewComment")]
    public async Task<IActionResult> AddNewComment([FromBody] AddComment addComment, [FromHeader] string authorization) => await _videoService.AddNewComment(addComment, authorization);
    
    [Role([1, 2])]
    [HttpPost("UpdateComment")]
    public async Task<IActionResult> UpdateComment([FromBody] UpdateComment updateComment, [FromHeader] string authorization)  => await _videoService.UpdateComment(updateComment, authorization);
    
    [Role([1, 2])]
    [HttpPost("DeleteComment")]
    public async Task<IActionResult> DeleteComment([FromBody] int commnetId, [FromHeader] string authorization) => await _videoService.DeleteCommentAsync(commnetId, authorization);
}
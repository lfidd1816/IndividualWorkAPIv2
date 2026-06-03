using IndividualWorkAPI.DatabaseContext;
using IndividualWorkAPI.Interfaces;
using IndividualWorkAPI.Models;
using IndividualWorkAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndividualWorkAPI.Services;

public class VideoService : IVideoService
{
    private readonly ContextDatabase _context;
    private readonly IS3ContainerService _s3;

    public VideoService(ContextDatabase context, IS3ContainerService s3)
    {
        _context = context;
        _s3 = s3;
    }

    private async Task<User?> GetUserFromAuthAsync(string authorization)
    {
        if (string.IsNullOrEmpty(authorization)) return null;
        var token = authorization.Split(' ').LastOrDefault();
        if (string.IsNullOrEmpty(token)) return null;
        
        var session = await _context.Sessions.Include(s => s.User)
            .FirstOrDefaultAsync(s => s.token == token);
        return session?.User;
    }
    
    public async Task<IActionResult> InitUploadAsync(
        string authorization, 
        string title, 
        string description, 
        long fileSize, 
        string mimeType, 
        bool privacyStatus)
    {
        var tempSession = authorization.Split(' ').Last();
        var session = await _context.Sessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.token == tempSession);

        if (session == null) 
            return new UnauthorizedObjectResult(new { status = false, message = "Требуется авторизация" });

        var s3Key = $"videos/{session.userId}/{Guid.NewGuid()}.mp4";
        var video = new Video
        {
            title = title,
            description = description,
            videoUrl = s3Key,
            s3Key = s3Key,
            thumbnailUrl = "",
            privacyStatus = privacyStatus,
            userId = session.userId,
            publishDate = DateTime.UtcNow,
            updateDate = DateTime.UtcNow
        };

        _context.Videos.Add(video);
        await _context.SaveChangesAsync();

        var presignedUrl = await _s3.GeneratePresignedPutUrlAsync(s3Key, mimeType, 15);
        return new OkObjectResult(new { 
            status = true, 
            videoId = video.videoId, 
            presignedUrl, 
            s3Key 
        });
    }

    
    public async Task<IActionResult> ConfirmUploadAsync(int videoId, string authorization)
    {
        var user = await GetUserFromAuthAsync(authorization);
        if (user == null) 
            return new UnauthorizedObjectResult(new { status = false, message = "Требуется авторизация" });

        var video = await _context.Videos.FirstOrDefaultAsync(v => v.videoId == videoId && v.userId == user.userId);
        if (video == null) 
            return new BadRequestObjectResult(new { status = false, message = "Video not found" });

        var publicUrl = await _s3.GeneratePresignedGetUrlAsync(
            video.s3Key, 
            expirationMinutes: 525600  
        );
    
        video.videoUrl = publicUrl;
        await _context.SaveChangesAsync();

        return new OkObjectResult(new { 
            status = true, 
            message = "Upload confirmed",
            videoUrl = publicUrl 
        });
    }

    
    public async Task<IActionResult> GetVideoAsync(int id)
    {
        var video = await _context.Videos.FirstOrDefaultAsync(v => v.videoId == id);
        if (video == null) return new NotFoundObjectResult(new { status = false, message = "Video not found" });

        var comments = await _context.Comments.Include(c => c.User).Where(c => c.videoId == id).ToListAsync();
        var reactions = await _context.Reactions.Where(r => r.videoId == id).ToListAsync();
        var userReaction = reactions.FirstOrDefault()?.reactionType switch
        {
            ReactionType.Like => "Like",
            ReactionType.Dislike => "Dislike",
            _ => ""
        };

        var streamUrl = video.videoUrl.StartsWith("http") 
            ? video.videoUrl 
            : await _s3.GeneratePresignedGetUrlAsync(video.videoUrl, 60);

        return new OkObjectResult(new
        {
            status = true,
            data = new
            {
                video.videoId, video.title, video.description,
                privacyStatus = video.privacyStatus ? "public" : "private",
                video.thumbnailUrl, videoUrl = streamUrl,
                video.publishDate, video.updateDate,
                comments = comments.Select(c => new { c.commentId, c.commentText, c.createDate, commentator = c.User?.fullname }),
                reactions = new
                {
                    likes = reactions.Count(r => r.reactionType == ReactionType.Like),
                    dislikes = reactions.Count(r => r.reactionType == ReactionType.Dislike),
                    userReaction
                }
            }
        });
    }

    public async Task<IActionResult> GetAllVideosAsync()
    {
        var videos = await _context.Videos.Include(l => l.user).ToListAsync();
        return new OkObjectResult(new { status = true, message = "Video founded", videos = videos});
    }

    public async Task<IActionResult> GetMyVideosAsync(string authorization)
    {
        var user = await GetUserFromAuthAsync(authorization);
        if (user == null) return new UnauthorizedObjectResult(new { status = false, message = "Требуется авторизация" });

        var videos = await _context.Videos.Where(v => v.userId == user.userId).ToListAsync();
        return new OkObjectResult(new { status = true, videos });
    }

    public async Task<IActionResult> GetUserVideosAsync(int userId)
    {
        var videos = await _context.Videos.Where(v => v.userId == userId).ToListAsync();
        return videos.Count == 0
            ? new NotFoundObjectResult(new { status = false, message = "This user hasn't published videos" })
            : new OkObjectResult(new { status = true, message = "Video founded", videos });
    }

    public async Task<IActionResult> CreateVideoAsync(CreateVideo createVideo, string authorization)
    {
        var user = await GetUserFromAuthAsync(authorization);
        if (user == null) return new UnauthorizedObjectResult(new { status = false, message = "Требуется авторизация" });

        _context.Videos.Add(new Video
        {
            title = createVideo.title,
            description = createVideo.description,
            thumbnailUrl = createVideo.thumbnailUrl,
            videoUrl = createVideo.videoUrl,
            publishDate = DateTime.UtcNow,
            updateDate = DateTime.UtcNow,
            userId = user.userId,
            privacyStatus = createVideo.privacyStatus
        });
        await _context.SaveChangesAsync();
        return new OkObjectResult(new { status = true, message = "Video created" });
    }

    public async Task<IActionResult> UpdateVideoAsync(UpdateVideo updateVideo, string authorization)
    {
        var user = await GetUserFromAuthAsync(authorization);
        if (user == null) return new UnauthorizedObjectResult(new { status = false, message = "Требуется авторизация" });

        var video = await _context.Videos.FirstOrDefaultAsync(v => v.videoId == updateVideo.videoId && v.userId == user.userId);
        if (video == null) return new NotFoundObjectResult(new { status = false, message = "Video not found or access denied" });

        video.title = updateVideo.title ?? video.title;
        video.description = updateVideo.description ?? video.description;
        video.thumbnailUrl = updateVideo.thumbnailUrl ?? video.thumbnailUrl;
        video.privacyStatus = updateVideo.privacyStatus;
        video.updateDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return new OkObjectResult(new { status = true, message = "Video updated" });
    }

    public async Task<IActionResult> DeleteVideoAsync(int videoId, string authorization)
    {
        var user = await GetUserFromAuthAsync(authorization);
        if (user == null) return new UnauthorizedObjectResult(new { status = false, message = "Требуется авторизация" });

        var video = await _context.Videos.FirstOrDefaultAsync(v => v.videoId == videoId && v.userId == user.userId);
        if (video == null) return new NotFoundObjectResult(new { status = false, message = "Video not found or access denied" });

        if (!video.videoUrl.StartsWith("http")) await _s3.DeleteObjectAsync(video.videoUrl);
        
        _context.Videos.Remove(video);
        await _context.SaveChangesAsync();
        return new OkObjectResult(new { status = true, message = "Video deleted" });
    }

    // 🔹 REACTIONS
    public async Task<IActionResult> AddNewReaction(AddReaction addReaction, string authorization)
    {
        var user = await GetUserFromAuthAsync(authorization);
        if (user == null) return new UnauthorizedObjectResult(new { status = false, message = "Требуется авторизация" });
        
        var videoReaction = await _context.Reactions.FirstOrDefaultAsync(r => 
            r.userId == user.userId && r.videoId == addReaction.videoId);
        if (videoReaction != null) return new BadRequestObjectResult(new { status = false, message = "Reaction already exists" });
        
        var commentReaction = await _context.Reactions.FirstOrDefaultAsync(r => r.userId == user.userId && r.commentId == addReaction.commentId);
        if(commentReaction != null) return new  BadRequestObjectResult(new { status = false, message = "Reaction already exists" });
        
        _context.Reactions.Add(new Reaction { reactionType = addReaction.reactionType, userId = user.userId, videoId = addReaction.videoId, commentId = addReaction.commentId });
        await _context.SaveChangesAsync();
        
        
        return new OkObjectResult(new { status = true, message = "Reaction added" });
    }   

    public async Task<IActionResult> SwapReactionsAsync(int reactionId, string authorization)
    {
        var user = await GetUserFromAuthAsync(authorization);
        if (user == null) return new UnauthorizedObjectResult(new { status = false, message = "Требуется авторизация" });

        var reaction = await _context.Reactions.FirstOrDefaultAsync(r => r.reactionId == reactionId && r.userId == user.userId);
        if (reaction == null) return new NotFoundObjectResult(new { status = false, message = "Reaction not found" });

        reaction.reactionType = reaction.reactionType == ReactionType.Like ? ReactionType.Dislike : ReactionType.Like;
        await _context.SaveChangesAsync();
        return new OkObjectResult(new { status = true, message = "Reaction swapped" });
    }

    public async Task<IActionResult> DeleteReactionAsync(int reactionId, string authorization)
    {
        var user = await GetUserFromAuthAsync(authorization);
        if (user == null) return new UnauthorizedObjectResult(new { status = false, message = "Требуется авторизация" });

        var reaction = await _context.Reactions.FirstOrDefaultAsync(r => r.reactionId == reactionId && r.userId == user.userId);
        if (reaction == null) return new NotFoundObjectResult(new { status = false, message = "Reaction not found" });

        _context.Reactions.Remove(reaction);
        await _context.SaveChangesAsync();
        return new OkObjectResult(new { status = true, message = "Reaction deleted" });
    }

    // 🔹 COMMENTS
    public async Task<IActionResult> AddNewComment(AddComment addComment, string authorization)
    {
        var user = await GetUserFromAuthAsync(authorization);
        if (user == null) return new UnauthorizedObjectResult(new { status = false, message = "Требуется авторизация" });

        _context.Comments.Add(new Comment
        {
            commentText = addComment.commentText,
            createDate = DateTime.UtcNow,
            userId = user.userId,
            videoId = addComment.videoId,
            parentCommentId = addComment.parentCommentId
        });
        await _context.SaveChangesAsync();
        return new OkObjectResult(new { status = true, message = "Comment added" });
    }

    public async Task<IActionResult> UpdateComment(UpdateComment updateComment, string authorization)
    {
        var user = await GetUserFromAuthAsync(authorization);
        if (user == null) return new UnauthorizedObjectResult(new { status = false, message = "Требуется авторизация" });

        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.commentId == updateComment.commentId && c.userId == user.userId);
        if (comment == null) return new NotFoundObjectResult(new { status = false, message = "Comment not found or access denied" });

        comment.commentText = updateComment.commentText;
        await _context.SaveChangesAsync();
        return new OkObjectResult(new { status = true, message = "Comment updated" });
    }

    public async Task<IActionResult> DeleteCommentAsync(int commentId, string authorization)
    {
        var user = await GetUserFromAuthAsync(authorization);
        if (user == null) return new UnauthorizedObjectResult(new { status = false, message = "Требуется авторизация" });

        var comment = await _context.Comments.Include(c => c.Video).FirstOrDefaultAsync(c => c.commentId == commentId);
        if (comment == null) return new NotFoundObjectResult(new { status = false, message = "Comment not found" });

        if (comment.userId != user.userId && comment.Video?.userId != user.userId)
            return new ForbidResult();

        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return new OkObjectResult(new { status = true, message = "Comment deleted" });
    }
}
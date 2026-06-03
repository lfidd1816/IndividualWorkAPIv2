using IndividualWorkAPI.DatabaseContext;
using IndividualWorkAPI.Interfaces;
using IndividualWorkAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndividualWorkAPI.Services;

public class SearchServcie : ISearchService
{
    public readonly ContextDatabase _context;

    public SearchServcie(ContextDatabase context)
    {
        context = _context;
        
    }
    
    public async Task<IActionResult> SearchVideoByName(SearchVideo searchVideo)
    {
        var videoList = await _context.Videos.Where(v => v.title == searchVideo.title).ToListAsync();

        if (videoList.Count == 0) return new NotFoundObjectResult(new
        {
            status = false,
            message = "No videos found"
        });
        
        return new OkObjectResult(new
        {
            status = true,
            videos = videoList
        });
    }
}
using IndividualWorkAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace IndividualWorkAPI.Interfaces;

public interface ISearchService
{
    Task<IActionResult> SearchVideoByName(SearchVideo searchVideo);
}
using IndividualWorkAPI.CustomAttributes;
using IndividualWorkAPI.Interfaces;
using IndividualWorkAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace IndividualWorkAPI.Controllers;


[Controller]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    
    [Role([1, 2])]
    [HttpGet("GetProfile")]
    public async Task<IActionResult> GetProfile([FromHeader] string authorization) => await _userService.GetProfile(authorization);
    
    [HttpGet("GetUserProfile")]
    public async Task<IActionResult> GetUserProfile([FromBody] int userId) => await _userService.GetUserProfile(userId);
    
    [Role([1, 2])]
    [HttpPost("UpdateAccount")]
    public async Task<IActionResult> UpdateAcconutAsync([FromHeader] string authorization,[FromBody] UserRegister updateUser) => await _userService.UpdateAccountAsync(authorization, updateUser);
    
    [Role([1, 2])]
    [HttpPost("DeleteAccount")]
    public async Task<IActionResult> DeleteAccountAsync([FromHeader] string authorization) => await _userService.DeleteAccountAsync(authorization);
}
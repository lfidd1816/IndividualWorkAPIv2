using IndividualWorkAPI.Interfaces;
using IndividualWorkAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace IndividualWorkAPI.Controllers;

[Controller]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegister registerUser) => await _authService.RegisterUserAsync(registerUser);
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] UserAuth userAuth) => await _authService.AuthorizeUserAsync(userAuth);
}
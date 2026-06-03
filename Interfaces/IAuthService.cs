using IndividualWorkAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace IndividualWorkAPI.Interfaces;

public interface IAuthService
{
    Task<IActionResult> RegisterUserAsync(UserRegister registerUser);
    Task<IActionResult> AuthorizeUserAsync(UserAuth userAuth);
}
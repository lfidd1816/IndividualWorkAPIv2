using IndividualWorkAPI.Models;
using IndividualWorkAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace IndividualWorkAPI.Interfaces;

public interface IUserService
{
    Task<IActionResult> GetUserProfile(int userId);
    Task<IActionResult> GetProfile(string authorization);
    Task<IActionResult> UpdateAccountAsync(string authorization, UserRegister updateUser);
    Task<IActionResult> DeleteAccountAsync(string authorization);
}
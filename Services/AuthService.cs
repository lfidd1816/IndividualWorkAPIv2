using System.ComponentModel.DataAnnotations;
using IndividualWorkAPI.DatabaseContext;
using IndividualWorkAPI.Interfaces;
using IndividualWorkAPI.Models;
using IndividualWorkAPI.Requests;
using IndividualWorkAPI.UniversalMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndividualWorkAPI.Services;

public class AuthService : IAuthService
{
        private readonly ContextDatabase _context;
        private readonly JWTTokenGenerator _jwtTokenGenerate;
    
        public AuthService(ContextDatabase contextDb, JWTTokenGenerator jwtTokenGenerate)
        {
            _context = contextDb;
            _jwtTokenGenerate = jwtTokenGenerate;
        }
    
        public async Task<IActionResult> RegisterUserAsync(UserRegister registerUser)
        {
            if (registerUser == null)
                return new BadRequestObjectResult(new { status = false, message = "Тело запроса пустое или имеет неверный формат" });

            if (_context == null)
                return new StatusCodeResult(500); 
            if (_context.Logins == null)
                return new StatusCodeResult(500); 

            var email = registerUser.email?.Trim();
            if (string.IsNullOrWhiteSpace(email))
                return new BadRequestObjectResult(new { status = false, message = "Email обязателен" });

            if (await _context.Logins.AnyAsync(x => x.email == email))
                return new BadRequestObjectResult(new { status = false, message = "Пользователь с таким Email уже существует" });

            
            var newLogin = new Login
            {
                email = registerUser.email,
                password = registerUser.password, 
                User = new User
                {
                    email = registerUser.email,
                    fullname = registerUser.fullname,
                    dateOfBirth = registerUser.dateOfBirth,
                    roleId = 2,
                    channelName = string.IsNullOrWhiteSpace(registerUser.channelName) 
                        ? registerUser.fullname 
                        : registerUser.channelName,
                    createdAt = DateTime.UtcNow,
                    updatedAt = DateTime.UtcNow
                }
            };

            await _context.Logins.AddAsync(newLogin);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new { status = true, message = "Вы успешно зарегистрировались" });
        }

    public async Task<IActionResult> AuthorizeUserAsync(UserAuth userAuth)
    {
        var login = await _context.Logins.Include(l => l.User).
            ThenInclude(u => u.Role).
            FirstOrDefaultAsync(x => x.email == userAuth.email && x.password == userAuth.password);
        if (login == null)
            return new UnauthorizedObjectResult(new {status = false, message = "Неверный логин или пароль"});
        
        var token = _jwtTokenGenerate.GenerateToken(login.User.userId, login.User.roleId);

        _context.Sessions.RemoveRange(_context.Sessions.Where(s => s.userId == login.User.userId));
        
        _context.Sessions.Add(new Session()
        {
            token = token,
            userId = login.User.userId,
        });
        
        await _context.SaveChangesAsync();
        
        return new OkObjectResult(new
        {
            status = true,
            message = "Вы успешно вошли",
            token = token, 
            userId = login.User.userId,
            roleId = login.User.roleId
        });
    }
}
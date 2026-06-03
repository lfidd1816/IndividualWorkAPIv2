using IndividualWorkAPI.DatabaseContext;
using IndividualWorkAPI.Interfaces;
using IndividualWorkAPI.Requests;
using IndividualWorkAPI.UniversalMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndividualWorkAPI.Services;

public class UserService : IUserService
{
    private readonly JWTTokenGenerator _jwtTokenGenerate;
    private readonly ContextDatabase _context;

    public UserService(JWTTokenGenerator jwtTokenGenerate, ContextDatabase context)
    {
        _jwtTokenGenerate = jwtTokenGenerate;
        _context = context;
    }

    public async Task<IActionResult> GetUserProfile(int userId)
    {
        var userProfile = await _context.Logins
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.userId == userId);

        if (userProfile == null)
            return new NotFoundObjectResult(new
            {
                status = false,
                message = "user not definded"
            });
        
        var videoList = await _context.Videos.Where(v => v.userId == userId).ToListAsync();

        if (videoList.Count == 0)
        {
            return new NotFoundObjectResult(new 
            { 
                status = false,
            });
        }

        return new OkObjectResult(new
        {
            status = true,
            message = "Данные профиля",
            data = new
            {
                userid = userProfile.User.userId,
                email = userProfile.email,
                fullname = userProfile.User.fullname, 
                dateOfBirth = userProfile.User.dateOfBirth,
                channelName = userProfile.User.channelName,
                updatedAt = userProfile.User.updatedAt,
                video = videoList
            }
        });
    }

    public async Task<IActionResult> GetProfile(string authorization)
    {
        var tempSession = authorization.Split(' ').Last();
    
        var session = await _context.Sessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.token == tempSession);
    
        if (session == null)
        {
            return new UnauthorizedObjectResult(new 
            { 
                status = false, 
                message = "Сессия не найдена" 
            });
        }
    
        var userProfile = await _context.Logins
            .Include(l => l.User)
            .FirstOrDefaultAsync(l => l.userId == session.userId); 
    
        if (userProfile == null)
        {
            return new NotFoundObjectResult(new 
            { 
                status = false, 
                message = "Пользователь не найден" 
            });
        }
        
        var videoList = await  _context.Videos.Where(v => v.userId == session.userId).ToListAsync();
    
        return new OkObjectResult(new
        {
            status = true,
            message = "Данные профиля",
            data = new
            {
                userid = userProfile.User.userId,
                email = userProfile.email,
                fullname = userProfile.User.fullname, 
                dateOfBirth = userProfile.User.dateOfBirth,
                channelName = userProfile.User.channelName,
                updatedAt = userProfile.User.updatedAt,
                video = videoList
            }
        });
    }

    public async Task<IActionResult> UpdateAccountAsync(string authorization, UserRegister updateUser)
    {
        var tempSession = authorization.Split(' ').Last();
        var thisUser = await _context.Sessions.Include(s => s.User).FirstOrDefaultAsync(s => s.token == tempSession);
        if (thisUser == null)
        {
            return new UnauthorizedObjectResult(new
            {
                status = false,
                message = "Сессия не найдена"
            });
        }
        
        var getUser = await _context.Logins.Include(l =>  l.User).FirstOrDefaultAsync(l => l.userId == thisUser.userId);

        /*if (getUser.email == thisUser.User.email)
        {
            if (await _context.Logins.Include(l => l.User)
                    .AnyAsync(l => l.email == updateUser.email))
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Данный Email уже занят"
                });
            }
        }*/

        if (await _context.Logins.Include(l => l.User).AnyAsync(l => l.User.channelName == updateUser.channelName))
        {
            return new NotFoundObjectResult(new
            {
                status = false,
                message = "Данное название канала уже занято"
            });
        }

        if (updateUser.password == null)
        {
            updateUser.password = getUser.password;
        }
        
        getUser.email = updateUser.email;
        getUser.User.email = updateUser.email;
        getUser.User.fullname = updateUser.fullname;
        getUser.User.dateOfBirth = updateUser.dateOfBirth;
        getUser.User.channelName = updateUser.channelName;
        getUser.password = updateUser.password;
        getUser.User.updatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        return new OkObjectResult(new
        {
            status = true,
            
            message = "Вы успешно обновили данные"
        });
    }

    public async Task<IActionResult> DeleteAccountAsync(string authorization)
    {
        var tempSession = authorization.Split(' ').Last();
        var thisUser = await _context.Sessions.Include(s => s.User).FirstOrDefaultAsync(s => s.token == tempSession);
        if (thisUser == null)
        {
            return new UnauthorizedObjectResult(new
            {
                status = false,
                message = "Сессия не найдена"
            });
        }

        var getUser = await _context.Logins.Include(l => l.User).FirstOrDefaultAsync(l => l.User.userId == thisUser.userId);

        if (getUser == null)
        {
            return new NotFoundObjectResult(new
            {
                status = false,
                message = "User is deleted"
            });
        }

        _context.Logins.Remove(getUser);
        _context.Users.Remove(getUser.User);

        await _context.SaveChangesAsync();

        return new OkObjectResult(new
        {
            status = true,
            message = "Вы успешно удалили аккаунт"
        });
    }
}
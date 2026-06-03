using IndividualWorkAPI.CustomAttributes;

namespace IndividualWorkAPI.Requests;

public class UserRegister
{
    public string email { get; set; }
    public string password { get; set; }
    public string fullname { get; set; }
    
    [DateOfBirth]
    public DateOnly? dateOfBirth { get; set; }
    public string? channelName  { get; set; }
    
    
}
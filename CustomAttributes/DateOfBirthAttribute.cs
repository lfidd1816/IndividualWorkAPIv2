using System.ComponentModel.DataAnnotations;

namespace IndividualWorkAPI.CustomAttributes;

public class DateOfBirthAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext context)
    {
        if (value is DateOnly date)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var minDate = new DateOnly(1900, 1, 1);
            
            if (date > today)
                return new ValidationResult("Дата рождения не может быть в будущем");
            
            if (date < minDate)
                return new ValidationResult("Дата рождения слишком ранняя");
        }
        return ValidationResult.Success;
    }
}
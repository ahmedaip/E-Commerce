using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Project.CustomValidations
{
    public class MinMaxLengthAttribute : ValidationAttribute
    {
        private readonly int MinLength;
        private readonly int MaxLength;

        public MinMaxLengthAttribute(int min, int max)
        {
            MaxLength = max;
            MinLength = min;
        }
        public override bool IsValid(object? value)
        {
            if (value is string result)
            {
                if (result.Length >= MinLength && result.Length <= MaxLength)
                    return true;
            }
            return false;
        }
    }
}

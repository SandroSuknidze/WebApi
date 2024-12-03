using System.ComponentModel.DataAnnotations;

namespace WebApi.Validators
{
    public class ExactLengthLongAttribute : ValidationAttribute
    {
        private readonly int _exactLength;

        public ExactLengthLongAttribute(int exactLength)
        {
            _exactLength = exactLength;
            ErrorMessage = $"The field must be exactly {_exactLength} digits long.";
        }

        public override bool IsValid(object value)
        {
            if (value == null) return true;
            

            if (value is long longValue)
            {
                return longValue.ToString().Length == _exactLength;
            }

            return false;
        }

    }
}

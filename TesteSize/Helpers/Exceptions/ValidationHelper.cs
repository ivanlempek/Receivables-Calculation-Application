namespace Helpers.Exceptions
{
    public class ValidationHelper
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }

        public ValidationHelper(bool isValid, string errorMessage = null)
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }
    }
}

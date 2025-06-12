namespace LaCasa.Models
{
    public class BookingValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }

        public BookingValidationResult(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }
    }
}
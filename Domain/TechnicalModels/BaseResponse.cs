using Domain.TechnicalEnums;
using FluentValidation.Results;
using System.Text;

namespace Domain.TechnicalModels
{
    public class BaseResponse
    {
        public bool Success { get; set; } = false;
        public ResponseStatus Status { get; set; } = ResponseStatus.NotDefined;
        public string Message { get; set; } = string.Empty;
        public dynamic? Payload { get; set; }
        public List<ValidationFailure> RawValidationErrors { get; set; } = new List<ValidationFailure>();


        public BaseResponse()
        {
            Status = ResponseStatus.Success;
            Success = true;
        }
        public BaseResponse(dynamic payload)
        {
            Status = ResponseStatus.Success;
            Success = true;
            Payload = payload;
        }
        public BaseResponse(bool success, ResponseStatus status, string message)
        {
            Success = success;
            Message = message;
            Status = status;
        }
        public BaseResponse(ValidationResult validationResult)
        {
            RawValidationErrors = validationResult.Errors;
            Success = validationResult.Errors.Count == 0;
            Message = MappErrorMessages(validationResult.Errors);
            Status = ResponseStatus.ValidationError;
        }


        private string MappErrorMessages(List<ValidationFailure> rawValidationErrors)
        {
            var stringBuilder = new StringBuilder();
            foreach (var rawValidationError in rawValidationErrors)
            {
                stringBuilder.AppendLine(rawValidationError.ToString());
            }

            return stringBuilder.ToString();
        }
    }
}

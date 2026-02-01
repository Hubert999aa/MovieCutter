namespace Domain.TechnicalEnums
{
    public enum ResponseStatus
    {
        NotDefined = 0,
        Success = 1,
        NotFound = 2,
        ValidationError = 3,
        AuthError = 4,
        DatabaseOperationError = 5,
    }
}

using FluentValidation.Results;

namespace Itau.Transfer.Domain.Exception;

public class BadRequestException : System.Exception
{
    public IReadOnlyCollection<ValidationFailure>? Errors { get; }
    public BadRequestException(string message) : base(message)
    {
        Errors = new List<ValidationFailure>();
    }

    public BadRequestException(List<ValidationFailure> errors)
    {
        Errors = errors;
    }
}
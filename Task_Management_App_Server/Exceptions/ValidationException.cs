using FluentValidation.Results;

namespace Task_Management_App.Exceptions;

public class ValidationException : Exception
{
    public IEnumerable<ValidationFailure> Errors { get; }

    //Constructor that takes a message and a collection of validation failures.
    public ValidationException(string message, IEnumerable<ValidationFailure> errors)
        : base(message) // call the base class constructor with the error message.
    {
    Errors = errors;  //Initialize the Errors property with the provided validation failures.                          
    }
}
﻿using FluentValidation.Results;

namespace Task_Management_App.Exceptions;

public class ValidationException : Exception
{
    public IEnumerable<ValidationFailure> Errors { get; }

    public ValidationException(string message, IEnumerable<ValidationFailure> errors)
        : base(message)
    {
        Errors = errors;
    }
}
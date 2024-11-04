using System.ComponentModel.DataAnnotations;

namespace Task_Management_App.Models;

public class ErrorResponse
{
    public bool Success { get; set; } = false;
    public IEnumerable<string> Errors { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; }

    public ErrorResponse(IEnumerable<string> errors, int statusCode, string message)
    {
        Errors = errors;
        StatusCode = statusCode;
        Message = message;
    }
}
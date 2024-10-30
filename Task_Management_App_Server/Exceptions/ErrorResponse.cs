using System.ComponentModel.DataAnnotations;

namespace Task_Management_App.Exceptions;

public class ErrorResponse
{
    [StringLength(10)]
    public int StatusCode { get; set; }
    [StringLength(50)]
    public string Message { get; set; } = string.Empty; 
    [StringLength(200)]
    public string? Details { get; set; }
}
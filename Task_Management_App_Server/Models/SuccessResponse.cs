﻿namespace Task_Management_App.Models;

public class SuccessResponse
{
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public SuccessResponse(string message, int statusCode = 200)
    {
        Message = message;
        StatusCode = statusCode;
    }
}
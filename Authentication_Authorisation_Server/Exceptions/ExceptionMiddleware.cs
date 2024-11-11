using System.Net;
using Authentication_Authorisation.Models;
using Microsoft.IdentityModel.Tokens;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Authentication_Authorisation.Exceptions;
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next; //Delegate for the next middleware in the pipeline.
    private readonly ILogger<ExceptionMiddleware> _logger; //Logger for logging error details.
    
    // Constructor that accepts the next middleware and a loger.
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    // method that processes the HTTP context.
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); //Call the next middleware int the pipeline
        }
        catch (Exception ex)
        {
            _logger.LogError($"Something went wrong: {ex}");
            await HandleExceptionAsync(context, ex);
        }
    }

    //Methode to handle the exception and format the error response.
    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var response = context.Response; // Get the Http response object.
        response.ContentType = "application/json"; // The response content is type JSON.
        ErrorResponse errorResponse; // Variable to hold the error response.
        
        // Determine the type of the Exception.
        switch (ex)
        {
            case ValidationException validationEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse = new ErrorResponse(validationEx.Errors.Select(e => e.ErrorMessage), response.StatusCode, "Validation failure");
                break;

            case NotFoundException notFoundEx:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse = new ErrorResponse(new[] { notFoundEx.Message }, response.StatusCode, "Not Found");
                break;
            case AuthenticationException authenticationException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse = new ErrorResponse(
                    new[] { authenticationException.Message },
                    response.StatusCode,
                    "Invalid email or password.");
                break;
            
            case InvalidTokenException  invalidTokenEx:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse = new ErrorResponse(new[] { invalidTokenEx.Message }, response.StatusCode,
                    "Unauthorized");
                break;
            
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse = new ErrorResponse(new[] { ex.Message}, response.StatusCode, "Internal Server Error"); 
                break;
        }

        var result = JsonSerializer.Serialize(errorResponse); //Serialize the error response to JSON.
        return response.WriteAsync(result); //Write the JSON to the HTTP response.
    }

    
}
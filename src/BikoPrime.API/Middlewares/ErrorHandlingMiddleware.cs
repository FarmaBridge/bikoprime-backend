namespace BikoPrime.API.Middlewares;

using System.Net;
using BikoPrime.API.Models;
using BikoPrime.Domain.Exceptions;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse();

        switch (exception)
        {
            case DomainException domainEx:
                context.Response.StatusCode = domainEx.Code switch
                {
                    "EMAIL_IN_USE" => (int)HttpStatusCode.Conflict,
                    "USERNAME_IN_USE" => (int)HttpStatusCode.Conflict,
                    "INVALID_CREDENTIALS" => (int)HttpStatusCode.Unauthorized,
                    "INVALID_GOOGLE_TOKEN" => (int)HttpStatusCode.Unauthorized,
                    "INVALID_REFRESH_TOKEN" => (int)HttpStatusCode.Unauthorized,
                    _ => (int)HttpStatusCode.BadRequest
                };
                response.StatusCode = context.Response.StatusCode;
                response.Error = domainEx.Code;
                response.Message = domainEx.Message;
                break;

            case FluentValidation.ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.StatusCode = context.Response.StatusCode;
                response.Error = "VALIDATION_ERROR";
                response.Message = validationEx.Errors.First().ErrorMessage;
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.StatusCode = context.Response.StatusCode;
                response.Error = "INTERNAL_SERVER_ERROR";
                response.Message = "Um erro interno ocorreu.";
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}

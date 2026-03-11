namespace BikoPrime.API.Middlewares;

using System.Net;
using BikoPrime.API.Models;
using BikoPrime.Domain.Exceptions;
using FluentValidation;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = context.TraceIdentifier;
        context.Items["TraceId"] = traceId;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, traceId);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception, string traceId)
    {
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse { TraceId = traceId };

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
                _logger.LogWarning(
                    "DomainException occurred | TraceId: {TraceId} | Code: {Code} | Message: {Message}",
                    traceId, domainEx.Code, domainEx.Message);
                break;

            case ValidationException validationEx:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.StatusCode = context.Response.StatusCode;
                response.Error = "VALIDATION_ERROR";
                response.Message = "Um ou mais campos são inválidos.";
                response.Details = validationEx.Errors
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                    .ToList();
                _logger.LogWarning(
                    "ValidationException occurred | TraceId: {TraceId} | Errors: {Errors}",
                    traceId, string.Join("; ", response.Details));
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.StatusCode = context.Response.StatusCode;
                response.Error = "INTERNAL_SERVER_ERROR";
                response.Message = "Um erro interno ocorreu. Consulte o TraceId para mais informações.";
                _logger.LogError(
                    exception,
                    "Unhandled exception occurred | TraceId: {TraceId} | Type: {ExceptionType} | Message: {Message}",
                    traceId, exception.GetType().Name, exception.Message);
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}

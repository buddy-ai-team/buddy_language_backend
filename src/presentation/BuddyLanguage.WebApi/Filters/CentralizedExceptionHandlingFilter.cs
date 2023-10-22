using System.Net;
using BuddyLanguage.Domain.Exceptions;
using BuddyLanguage.Domain.Exceptions.Role;
using BuddyLanguage.HttpModels.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BuddyLanguage.WebApi.Filters;

public class CentralizedExceptionHandlingFilter 
    : Attribute, IExceptionFilter, IOrderedFilter
{
    public int Order { get; set; }

    public void OnException(ExceptionContext context)
    {
        var message = TryGetUserMessageFromException(context);
        HttpStatusCode statusCode = HttpStatusCode.Conflict;
        if (message != null)
        {
            context.Result = new ObjectResult(new ErrorResponse(message, statusCode))
            {
                StatusCode=(409)
            };
            context.ExceptionHandled = true;
        }
    }

    private string? TryGetUserMessageFromException(ExceptionContext context)
    {
        return context.Exception switch
        {
            RoleNotFoundException => "Роль с таким именем не наёдена",
            RoleAlreadyExistsException => "Роль с таким имененм уже существует!",
            DomainException => "Необработанная ошибка!",
            _ => null
        };
    }
    
}
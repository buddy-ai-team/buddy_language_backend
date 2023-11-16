using System.Net;

namespace BuddyLanguage.HttpModels.Responses;

public record ErrorResponse(string Message, HttpStatusCode? StatusCode = null);

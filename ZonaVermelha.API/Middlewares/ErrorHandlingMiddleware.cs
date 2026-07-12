using System.Text.Json;
using ZonaVermelha.Domain.Exceptions;

namespace ZonaVermelha.API.Middlewares;

public class ErrorHandlingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await TratarExcecaoAsync(context, ex);
        }
    }

    private static async Task TratarExcecaoAsync(HttpContext context, Exception ex)
    {
        if (ex is ValidacaoException ve)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            var json = JsonSerializer.Serialize(new { erros = ve.Erros });
            await context.Response.WriteAsync(json);
        }
        else if (ex is NotFoundException nfe)
        {
            context.Response.StatusCode = 404;
            context.Response.ContentType = "application/json";
            var json = JsonSerializer.Serialize(new { erro = nfe.Message });
            await context.Response.WriteAsync(json);
        }
        else
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var json = JsonSerializer.Serialize(new { erro = "Ocorreu um erro interno. Tente novamente mais tarde." });
            await context.Response.WriteAsync(json);
        }
    }
}

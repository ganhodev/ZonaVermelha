using ZonaVermelha.Communication.Requests;
using ZonaVermelha.Communication.Responses;
using ZonaVermelha.Domain;
using ZonaVermelha.Infrastructure;

namespace ZonaVermelha.API.Services;

public class UsuarioService(ZonaVermelhaDbContext dbContext)
{
    public async Task<ResponseUsuarioJson> CriarUsuarioAsync(RequestUsuarioJson request)
    {
        var usuario = new Usuario(request.Nome, request.Email);
        dbContext.Usuarios.Add(usuario);
        await dbContext.SaveChangesAsync();

        return new ResponseUsuarioJson
        {
            Id = usuario.IdUsuario,
            Nome = usuario.Nome,
            Email = usuario.Email
        };
    }
}

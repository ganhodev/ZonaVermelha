using ZonaVermelha.Communication.Requests;
using ZonaVermelha.Communication.Responses;
using ZonaVermelha.Infrastructure;

namespace ZonaVermelha.API.Services;
//async/await permite que a thread não fique bloqueada esperando.
public class RelatoService(ZonaVermelhaDbContext dbContext)
{
    public async Task<ResponseRelatoJson> CriarRelatoAsync(RequestRelatoJson requestRelatoJson)
    {
        throw new NotImplementedException();
    }
}
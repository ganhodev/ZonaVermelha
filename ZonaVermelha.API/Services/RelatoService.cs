using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ZonaVermelha.API.Hubs;
using ZonaVermelha.Communication.Requests;
using ZonaVermelha.Communication.Responses;
using ZonaVermelha.Domain;
using ZonaVermelha.Domain.Exceptions;
using ZonaVermelha.Infrastructure;

namespace ZonaVermelha.API.Services;
//async/await permite que a thread não fique bloqueada esperando.
public class RelatoService(ZonaVermelhaDbContext dbContext, IHubContext<ZonasHub> hubContext)
{
    public async Task<ResponseRelatoJson> CriarRelatoAsync(RequestRelatoJson requestRelatoJson)
    {
        //Validação dos dados de entrada
        var erros = new List<string>();

        if (string.IsNullOrWhiteSpace(requestRelatoJson.Descricao))
            erros.Add("Descrição é obrigatória.");

        if (requestRelatoJson.Latitude < -90 || requestRelatoJson.Latitude > 90)
            erros.Add("Latitude deve estar entre -90 e 90.");

        if (requestRelatoJson.Longitude < -180 || requestRelatoJson.Longitude > 180)
            erros.Add("Longitude deve estar entre -180 e 180.");

        if (requestRelatoJson.UsuarioId == Guid.Empty)
            erros.Add("UsuarioId é obrigatório.");

        if (erros.Count > 0)
            throw new ValidacaoException(erros);


        //Verificar se o usuário existe
        var usuarioExiste = await dbContext.Usuarios
        .AnyAsync(u => u.IdUsuario == requestRelatoJson.UsuarioId);

        if (!usuarioExiste)
            throw new NotFoundException("Usuário não encontrado.");


        var zonas = await dbContext.Zonas.ToListAsync();
        //Aqui fica um objeto anônimo com { Zona, Distancia }, se encontrou alguma zona dentro do raio
        //null, se nenhuma zona satisfez a condição do .Where()

        var zonaMaisProxima = zonas
            .Select(z => new
            {
                Zona = z,
                Distancia = CalculadoraDistancia.CalcularDistanciaMetros(
                    requestRelatoJson.Latitude,
                    requestRelatoJson.Longitude,
                    z.Latitude,
                    z.Longitude)
            })
            .Where(d => d.Distancia < d.Zona.RaioMetros)
            .OrderBy(d => d.Distancia).FirstOrDefault();

        Zona zonaFinal;

        if (zonaMaisProxima == null)
        {
            zonaFinal = new Zona(requestRelatoJson.Latitude, requestRelatoJson.Longitude)
            {
                RaioMetros = 200,
                NivelIntensidadeZona = 1,
                CriadaEm = DateTime.UtcNow,
                UltimaAtividade = DateTime.UtcNow
            };
            dbContext.Zonas.Add(zonaFinal);
        } else
        {
            zonaFinal = zonaMaisProxima.Zona;
            zonaFinal.NivelIntensidadeZona++;
            zonaFinal.UltimaAtividade = DateTime.UtcNow;
        }
        var relato = new Relato(requestRelatoJson.Descricao, requestRelatoJson.Latitude, requestRelatoJson.Longitude, requestRelatoJson.UsuarioId);
        relato.ZonaId = zonaFinal.IdZona;

        dbContext.Relatos.Add(relato);

        await dbContext.SaveChangesAsync();


        await hubContext.Clients
    .Group(zonaFinal.IdZona.ToString())
    .SendAsync("ZonaAtualizada", new
    {
        zonaFinal.IdZona,
        zonaFinal.Latitude,
        zonaFinal.Longitude,
        zonaFinal.NivelIntensidadeZona,
        zonaFinal.UltimaAtividade
    });

        //Notifica todos os clientes que estão no grupo da zona(com os dados atualizados da zona)
        return new ResponseRelatoJson
        {
            IdRelato = relato.IdRelato,
            Descricao = relato.Descricao,
            Latitude = relato.Latitude,
            Longitude = relato.Longitude,
            CriadoEm = relato.CriadoEm,
            ZonaId = relato.ZonaId
        };
    }
}

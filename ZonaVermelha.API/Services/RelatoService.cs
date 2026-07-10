using ZonaVermelha.Communication.Requests;
using ZonaVermelha.Communication.Responses;
using ZonaVermelha.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ZonaVermelha.Domain;

namespace ZonaVermelha.API.Services;
//async/await permite que a thread não fique bloqueada esperando.
public class RelatoService(ZonaVermelhaDbContext dbContext)
{
    public async Task<ResponseRelatoJson> CriarRelatoAsync(RequestRelatoJson requestRelatoJson)
    {
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
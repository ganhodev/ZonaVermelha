using Microsoft.EntityFrameworkCore;
using ZonaVermelha.Communication.Responses;
using ZonaVermelha.Domain;
using ZonaVermelha.Infrastructure;

namespace ZonaVermelha.API.Services;

public class ZonaService(ZonaVermelhaDbContext dbContext)
{
    public async Task<List<ResponseZonaJson>> BuscarZonasProximasAsync(double latitude, double longitude) 
    {
        var zonas = await dbContext.Zonas.ToListAsync();

        var zonasRaio5Km = zonas
            .Select(zona => new
            {
                Zona = zona,
                Distancia = CalculadoraDistancia.CalcularDistanciaMetros(latitude, longitude, zona.Latitude, zona.Longitude)
            }
            ).Where(z => z.Distancia <= 5000).Select(z => new ResponseZonaJson
            {
                IdZona = z.Zona.IdZona,
                Latitude = z.Zona.Latitude,
                Longitude = z.Zona.Longitude,
                RaioMetros = z.Zona.RaioMetros,
                NivelIntensidadeZona = z.Zona.NivelIntensidadeZona,
                CriadaEm = z.Zona.CriadaEm,
                UltimaAtividade = z.Zona.UltimaAtividade
            })
            .ToList();
        return zonasRaio5Km;
        }   
 }

 
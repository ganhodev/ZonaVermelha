using Microsoft.AspNetCore.SignalR;
using ZonaVermelha.API.Hubs;
using ZonaVermelha.Infrastructure;
using Microsoft.EntityFrameworkCore;
namespace ZonaVermelha.API.BackgroundServices;

/*BackGroundService - É uma classe que roda em segundo plano, independente das requisições HTTP —
ela fica rodando enquanto a aplicação está rodando,
executando uma tarefa repetidamente em intervalos definidos
*/

public class ZonaExpiracaoService(IServiceProvider services, IHubContext<ZonasHub> hubContext) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //!stoppingToken garante que o loop continua rodando até a aplicação ser encerrada
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ZonaVermelhaDbContext>();

            //busca zonas expiradas
            var limite = DateTime.UtcNow.AddHours(-6);
            var zonasExpiradas = await dbContext.Zonas
                .Where(z => z.UltimaAtividade < limite)
                .ToListAsync();


            foreach (var zona in zonasExpiradas)
            {
                // Desassocia os relatos da zona (preserva o histórico, não deleta)
                var relatos = await dbContext.Relatos
                    .Where(r => r.ZonaId == zona.IdZona)
                    .ToListAsync();

                foreach (var relato in relatos)
                    relato.ZonaId = null;

                // Remove a zona
                dbContext.Zonas.Remove(zona);

                // Notifica clientes que a zona expirou
                await hubContext.Clients
                    .Group(zona.IdZona.ToString())
                    .SendAsync("ZonaExpirada", zona.IdZona);
            }

            await dbContext.SaveChangesAsync(stoppingToken);


            // TimeSpan faz ele esperar 5 minutos entre cada verificação.
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}

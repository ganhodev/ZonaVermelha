using Microsoft.AspNetCore.SignalR;

namespace ZonaVermelha.API.Hubs;
    //Protocolo WebSocket atualização em tempo real(mensagens de zonas criadas) 
public class ZonasHub : Hub
{
    public async Task EntrarNaRegiao(string regiaoId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, regiaoId);
    }
}


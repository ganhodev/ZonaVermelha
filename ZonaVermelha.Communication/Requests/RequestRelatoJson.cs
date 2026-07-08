namespace ZonaVermelha.Communication.Requests;

//o record gera automaticamente as propriedades, o constructor, e tudo mais.
public record RequestRelatoJson(string Descricao, double Latitude, double Longitude, Guid UsuarioId);

namespace ZonaVermelha.Domain;

public class Relato(string descricao, double latitude, double longitude, Guid usuarioId)
{
    //criar identificadores únicos
    public Guid IdRelato { get; set; } = Guid.NewGuid();
    public string Descricao { get; set; } = descricao;
    public double Latitude { get; set; } = latitude;
    public double Longitude { get; set; } = longitude;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public Guid UsuarioId { get; set; } = usuarioId;
    public Guid ZonaId { get; set; }
    public Zona Zona { get; set; } = null!;
}

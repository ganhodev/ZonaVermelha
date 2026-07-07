namespace ZonaVermelha.Domain;

public class Relato
{
    //criar identificadores únicos
    public Guid IdRelato { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public double Latitude { get; set; } = double.NaN;
    public double Longitude { get; set; } = double.NaN;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public Guid UsuarioId { get; set; }
    public Guid ZonaId { get; set; }
    public Zona Zona { get; set; } = null!;
}

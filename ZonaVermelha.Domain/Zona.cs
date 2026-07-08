namespace ZonaVermelha.Domain;

public class Zona(double latitude, double longitude)
{
    public Guid IdZona { get; set; } = Guid.NewGuid();
    public double Latitude { get; set; } = latitude;
    public double Longitude { get; set; } = longitude;
    public double RaioMetros { get; set; } = 200;
    public int NivelIntensidadeZona { get; set; } = 1;
    public DateTime CriadaEm { get; set; } = DateTime.UtcNow;
    public DateTime UltimaAtividade { get; set; } = DateTime.UtcNow;
    public ICollection<Relato> RelatosZona { get; set; } = [];
}

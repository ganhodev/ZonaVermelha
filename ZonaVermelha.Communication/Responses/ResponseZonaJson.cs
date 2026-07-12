namespace ZonaVermelha.Communication.Responses;

public class ResponseZonaJson
{
    public Guid IdZona { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double RaioMetros { get; set; }
    public int NivelIntensidadeZona { get; set; }
    public DateTime CriadaEm { get; set; }
    public DateTime UltimaAtividade { get; set; }
}

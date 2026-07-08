namespace ZonaVermelha.Communication.Responses;

public class ResponseRelatoJson
{
    public Guid IdRelato { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime CriadoEm { get; set; }
    public Guid ZonaId { get; set; }
}

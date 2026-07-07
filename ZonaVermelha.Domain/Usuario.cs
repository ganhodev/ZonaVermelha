namespace ZonaVermelha.Domain;

public class Usuario
{
    public Guid IdUsuario{ get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICollection<Relato> Relatos { get; set; } = [];
}

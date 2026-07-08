namespace ZonaVermelha.Domain;

public class Usuario(string nome, string email)
{
    public Guid IdUsuario{ get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = nome;
    public string Email { get; set; } = email;
    public ICollection<Relato> Relatos { get; set; } = [];
}

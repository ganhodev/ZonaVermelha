using Microsoft.EntityFrameworkCore;
using ZonaVermelha.Domain;

namespace ZonaVermelha.Infrastructure;

public class ZonaVermelhaDbContext(DbContextOptions<ZonaVermelhaDbContext> options) : DbContext(options)
{
    //DbSet representa uma tabela do banco de dados
    public DbSet<Relato> Relatos { get; set; }
    public DbSet<Zona> Zonas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
}

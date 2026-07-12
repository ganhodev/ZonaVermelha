namespace ZonaVermelha.Domain.Exceptions;
//classe para quando algo não é encontrado no banco (ex: usuário não existe):
public class NotFoundException : ZonaVermelhaException
{
    public NotFoundException(string mensagem) : base(mensagem) { }
}


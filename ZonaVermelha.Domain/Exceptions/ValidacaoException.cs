namespace ZonaVermelha.Domain.Exceptions;

// classe para erros de validação de entrada (campos inválidos, vazios, fora do range).
public class ValidacaoException : ZonaVermelhaException
{
    public List<string> Erros { get; }

    public ValidacaoException(List<string> erros) : base(string.Empty)
    {
        Erros = erros;
    }
}

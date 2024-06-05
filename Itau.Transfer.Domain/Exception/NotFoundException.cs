namespace Itau.Transfer.Domain.Exception;

public class NotFoundException : System.Exception
{
    public NotFoundException(string typeName) : base($"{typeName}")
    {
    }
}
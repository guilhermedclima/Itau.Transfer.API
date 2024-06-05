namespace Itau.Transfer.Domain.Exception;

public class HttpClientRequestException : System.Exception
{
    public HttpClientRequestException(string typeName) : base($"{typeName}")
    {
    }
}
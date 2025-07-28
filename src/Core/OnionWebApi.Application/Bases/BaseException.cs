namespace OnionWebApi.Application.Bases;
public class BaseException : ApplicationException
{
    public BaseException() { }
    public BaseException(string message) : base(message) { }
}

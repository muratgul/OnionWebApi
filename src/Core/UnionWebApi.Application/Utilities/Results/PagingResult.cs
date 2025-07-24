namespace UnionWebApi.Application.Utilities.Results;

public class PagingResult<T> : Result, IPagingResult<T>
{
    public PagingResult(List<T> data, int totalItemCount, bool success, string message) : base(success, message)
    {
        Data = data;
        TotalItemCount = totalItemCount;
    }

    public IEnumerable<T> Data { get; }
    public int TotalItemCount { get; }
}
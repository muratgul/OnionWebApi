namespace OnionWebApi.Application.Utilities.Results;

public interface IPagingResult<T> : IResult
{
    /// <summary>
    /// data list
    /// </summary>
    IEnumerable<T> Data { get; }

    /// <summary>
    /// total number of records
    /// </summary>
    int TotalItemCount { get; }
}
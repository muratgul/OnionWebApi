namespace UnionWebApi.Application.RequestParameters;
public class PagingParameter
{
    public int PageSize { get; set; } = 10000000;
    public int PageNumber { get; set; } = 1;
}

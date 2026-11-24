namespace LibrarySystem.Results;

public class PaginatedList<T>
{
    private const int MaxPageSize = 20;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;
    public int TotalRecords { get; set; }
    public int TotalPage { get; set; }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 10 : value);
    }

    public IEnumerable<T>? Data { get; set; }

    // Builds a page and calculates TotalPage in one place
    public static PaginatedList<T> Create(IEnumerable<T> data, int pageNumber, int pageSize, int totalRecords)
    {
        var result = new PaginatedList<T>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = totalRecords,
            Data = data
        };
        result.TotalPage = (int)Math.Ceiling(totalRecords / (double)result.PageSize);
        return result;
    }
}
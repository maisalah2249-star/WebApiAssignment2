namespace LibrarySystem.Dtos.BookDtos;

public class BookQueryParams
{
    private const int MaxPageSize = 20;
    private int _pageNumber = 1;
    private int _pageSize = 10;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 10 : value);
    }

    /// <summary>Search in book title or ISBN.</summary>
    public string? Search { get; set; }

    /// <summary>Filter by author.</summary>
    public int? AuthorId { get; set; }

    /// <summary>Filter by availability: true = not currently borrowed, false = currently borrowed.</summary>
    public bool? IsAvailable { get; set; }

    /// <summary>Sort: title (default), title_desc, isbn.</summary>
    public string? Sort { get; set; }
}
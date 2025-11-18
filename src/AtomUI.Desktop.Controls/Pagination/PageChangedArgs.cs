namespace AtomUI.Desktop.Controls;

public class PageChangedArgs
{
    public PageChangedArgs(int pageNumber, int totalPages, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = totalPages;
        PageSize = pageSize;
    }
    
    public int PageSize { get; }
    public int TotalPages { get; }
    public int PageNumber { get; }
}
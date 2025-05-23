namespace AtomUI.Controls;

internal class PageNavRequestArgs
{
    public PageNavRequestArgs(PaginationNavItem item, int index, int pageNumber)
    {
        NavItem    = item;
        Index      = index;
        PageNumber = pageNumber;
    }

    public PaginationNavItem NavItem { get; }

    public int Index { get; }
    public int PageNumber { get; }
}
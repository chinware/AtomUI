namespace AtomUI.Controls;

public class BreadcrumbNavigateEventArgs : EventArgs
{
    public BreadcrumbItem BreadcrumbItem { get; }

    public BreadcrumbNavigateEventArgs(BreadcrumbItem breadcrumbItem)
    {
        BreadcrumbItem = breadcrumbItem;
    }
}
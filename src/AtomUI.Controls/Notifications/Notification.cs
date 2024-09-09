using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AtomUI.Controls;

public class Notification : INotification, INotifyPropertyChanged
{
    private object? _content;
    private PathIcon? _icon;
    private bool _showProgress;
    private string _title;

    public Notification(string title,
        object? content,
        NotificationType type = NotificationType.Information,
        PathIcon? icon = null,
        TimeSpan? expiration = null,
        bool showProgress = false,
        Action? onClick = null,
        Action? onClose = null)
    {
        _title       = title;
        _content     = content;
        _icon        = icon;
        Type         = type;
        Expiration   = expiration.HasValue ? expiration.Value : TimeSpan.FromSeconds(5);
        ShowProgress = showProgress;
        OnClick      = onClick;
        OnClose      = onClose;
    }

    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }

    public object? Content
    {
        get => _content;
        set
        {
            if (!ReferenceEquals(_content, value))
            {
                _content = value;
                OnPropertyChanged();
            }
        }
    }

    public PathIcon? Icon
    {
        get => _icon;
        set
        {
            if (!ReferenceEquals(_icon, value))
            {
                _icon = value;
                OnPropertyChanged();
            }
        }
    }

    public bool ShowProgress
    {
        get => _showProgress;
        set
        {
            if (_showProgress != value)
            {
                _showProgress = value;
                OnPropertyChanged();
            }
        }
    }

    public NotificationType Type { get; set; }

    public TimeSpan Expiration { get; set; }

    public Action? OnClick { get; set; }

    public Action? OnClose { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
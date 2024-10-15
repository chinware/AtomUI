﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using AtomUI.IconPkg;

namespace AtomUI.Controls;

public class Message : IMessage, INotifyPropertyChanged
{
    private string _content;
    private Icon? _icon;

    public Message(string content,
                   MessageType type = MessageType.Information,
                   Icon? icon = null,
                   TimeSpan? expiration = null,
                   Action? onClose = null)
    {
        _content   = content;
        _icon      = icon;
        Type       = type;
        Expiration = expiration.HasValue ? expiration.Value : TimeSpan.FromSeconds(5);
        OnClose    = onClose;
    }

    public string Content
    {
        get => _content;

        set
        {
            if (_content != value)
            {
                _content = value;
                OnPropertyChanged();
            }
        }
    }

    public Icon? Icon
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

    public MessageType Type { get; set; }

    public TimeSpan Expiration { get; set; }

    public Action? OnClose { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
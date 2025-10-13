using Avalonia.Input;

namespace AtomUI.Controls;

public interface IMenuItemData : ITreeNode<IMenuItemData>
{
    KeyGesture? InputGesture { get; }
}
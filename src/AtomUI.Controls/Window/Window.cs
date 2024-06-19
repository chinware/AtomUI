using AtomUI.TokenSystem;

namespace AtomUI.Controls.Window;

using AvaloniaWindow = Avalonia.Controls.Window;

public class Window : AvaloniaWindow, ITokenIdProvider
{
   string ITokenIdProvider.TokenId => nameof(Window);
}
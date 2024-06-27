using AtomUI.TokenSystem;
using Avalonia;

namespace AtomUI.Controls;

using AvaloniaFlyoutPresenter = Avalonia.Controls.FlyoutPresenter;

public partial class FlyoutPresenter : AvaloniaFlyoutPresenter, ITokenIdProvider
{
   string ITokenIdProvider.TokenId => FlyoutPresenterToken.ID;
   

}
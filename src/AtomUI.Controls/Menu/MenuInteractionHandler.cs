using Avalonia.Controls.Platform;
using Avalonia.Input;

namespace AtomUI.Controls;

public class MenuInteractionHandler : DefaultMenuInteractionHandler
{
   public MenuInteractionHandler(bool isContextMenu)
      : base(isContextMenu)
   {
   }

   public MenuInteractionHandler(bool isContextMenu,
                                 IInputManager? inputManager,
                                 Action<Action, TimeSpan> delayRun)
      : base(isContextMenu, inputManager, delayRun)
   {
   }
}
using Avalonia;

namespace AtomUI.Controls.DialogPositioning;

    public record struct PopupPositionerParameters
    {
        public Size Size { get; set; }
        public Point Offset { get; set; }
    }

public interface IDialogPositioner
{
    void Update(PopupPositionerParameters parameters);
}
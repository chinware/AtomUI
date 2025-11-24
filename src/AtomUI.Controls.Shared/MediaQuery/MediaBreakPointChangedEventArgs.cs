namespace AtomUI.Controls;

public class MediaBreakPointChangedEventArgs : EventArgs
{
    public MediaBreakPointChangedEventArgs(MediaBreakPoint breakPoint)
    {
        MediaBreakPoint = breakPoint;
    }
    
    public MediaBreakPoint MediaBreakPoint { get; }
}
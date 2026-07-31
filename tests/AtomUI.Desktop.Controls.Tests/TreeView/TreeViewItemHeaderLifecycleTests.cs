using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.TreeView;

public class TreeViewItemHeaderLifecycleTests
{
    [Fact]
    public void OnApplyTemplate_Unsubscribes_Previous_HeaderContentFrame()
    {
        var header = new TestTreeViewItemHeader();
        var oldFrame = new Border();
        var newFrame = new Border();

        header.ApplyHeaderContentFrame(oldFrame);
        RaisePointerEntered(oldFrame);

        Assert.True(header.IsHover);

        header.IsHover = false;
        header.ApplyHeaderContentFrame(newFrame);
        RaisePointerEntered(oldFrame);

        Assert.False(header.IsHover);
    }

    private static void RaisePointerEntered(Control source)
    {
        source.RaiseEvent(new PointerEventArgs(
            InputElement.PointerEnteredEvent,
            source,
            new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true),
            source,
            default,
            0,
            new PointerPointProperties(RawInputModifiers.None, PointerUpdateKind.LeftButtonPressed),
            KeyModifiers.None));
    }

    private sealed class TestTreeViewItemHeader : TreeViewItemHeader
    {
        public void ApplyHeaderContentFrame(Border headerContentFrame)
        {
            var nameScope = new NameScope();
            nameScope.Register("PART_HeaderContentFrame", headerContentFrame);

            OnApplyTemplate(new TemplateAppliedEventArgs(nameScope));
        }
    }
}

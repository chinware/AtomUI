using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.NavMenu;

public class NavMenuInteractionHandlerTests
{
    [Fact]
    public void Default_Handler_Detach_Disposes_Pending_Delayed_Open()
    {
        var scheduledRuns = new List<TrackingDisposable>();
        var handler = new DefaultNavMenuInteractionHandler(null, (_, _) =>
        {
            var disposable = new TrackingDisposable();
            scheduledRuns.Add(disposable);
            return disposable;
        });
        var menu = new AtomUI.Desktop.Controls.NavMenu();

        handler.AttachCore(menu);
        handler.OpenWithDelay(new NavMenuItem());

        scheduledRuns.Count.ShouldBe(1);
        scheduledRuns[0].IsDisposed.ShouldBeFalse();

        handler.DetachCore(menu);

        scheduledRuns[0].IsDisposed.ShouldBeTrue();
    }

    private sealed class TrackingDisposable : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}

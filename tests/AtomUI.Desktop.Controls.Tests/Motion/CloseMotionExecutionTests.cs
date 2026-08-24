using AtomUI.Controls.Primitives;
using AtomUI.MotionScene;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Motion;

public class CloseMotionExecutionTests
{
    static CloseMotionExecutionTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void MessageCard_Retemplate_During_Pending_Close_Starts_One_Exit_Motion()
    {
        var card = new TestMessageCard
        {
            IsMotionEnabled         = true,
            OpenCloseMotionDuration = TimeSpan.Zero
        };
        var actor = new MotionActor();
        var preStartCount = 0;
        actor.PreStart += (_, _) => preStartCount++;

        card.Close();
        card.ApplyMotionActor(actor);
        Dispatcher.UIThread.RunJobs();

        preStartCount.ShouldBe(1);
        card.IsClosed.ShouldBeTrue();
    }

    [Fact]
    public void NotificationCard_Retemplate_During_Pending_Close_Starts_One_Exit_Motion()
    {
        using var manager = new WindowNotificationManager();
        var card = new TestNotificationCard(manager)
        {
            IsMotionEnabled         = true,
            OpenCloseMotionDuration = TimeSpan.Zero
        };
        var actor = new MotionActor();
        var preStartCount = 0;
        actor.PreStart += (_, _) => preStartCount++;

        card.Close();
        card.ApplyMotionActor(actor);
        Dispatcher.UIThread.RunJobs();

        preStartCount.ShouldBe(1);
        card.IsClosed.ShouldBeTrue();
    }

    private sealed class TestMessageCard : MessageCard
    {
        internal void ApplyMotionActor(BaseMotionActor actor)
        {
            var nameScope = new NameScope();
            nameScope.Register(BaseMotionActor.MotionActorPart, actor);
            OnApplyTemplate(new TemplateAppliedEventArgs(nameScope));
        }
    }

    private sealed class TestNotificationCard : NotificationCard
    {
        internal TestNotificationCard(WindowNotificationManager manager)
            : base(manager)
        {
        }

        internal void ApplyMotionActor(BaseMotionActor actor)
        {
            var nameScope = new NameScope();
            nameScope.Register(BaseMotionActor.MotionActorPart, actor);
            OnApplyTemplate(new TemplateAppliedEventArgs(nameScope));
        }
    }
}

using System.Reflection;
using AtomUIGallery.ShowCases.Form;
using AtomUIGallery.ShowCases.TabControl;
using AtomUIGallery.ShowCases.TabStrip;
using AtomUIGallery.Workspace.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Localization;

public class LanguageSubscriptionLifecycleTests
{
    static LanguageSubscriptionLifecycleTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Visual_Language_Subscriptions_Release_Their_Exact_Manager_On_Detach()
    {
        var application = Application.Current.ShouldNotBeNull();
        var languageManager = application.GetLanguageManager().ShouldNotBeNull();
        Control[] controls =
        [
            new TabControlShowCase(),
            new TabStripShowCase(),
            new CaseNavigation()
        ];

        foreach (var control in controls)
        {
            var subscribedManager = control.GetType().GetField(
                "_subscribedLanguageManager",
                BindingFlags.Instance | BindingFlags.NonPublic).ShouldNotBeNull();
            var window = new Window { Content = control };
            try
            {
                window.Show();
                Dispatcher.UIThread.RunJobs();
                subscribedManager.GetValue(control).ShouldBeSameAs(languageManager);

                window.Content = null;
                Dispatcher.UIThread.RunJobs();
                subscribedManager.GetValue(control).ShouldBeNull();
            }
            finally
            {
                window.Close();
            }
        }
    }

    [Fact]
    public void Dynamic_Passenger_Form_Item_Disposes_Localization_Binding_On_Detach()
    {
        VerifyPassengerFormItemBindingIsDisposedOnDetach();
    }

    private static void VerifyPassengerFormItemBindingIsDisposedOnDetach()
    {
        var factory = typeof(FormShowCase).GetMethod(
            "CreatePassengerFormItem",
            BindingFlags.Static | BindingFlags.NonPublic).ShouldNotBeNull();
        var formItem = factory!.Invoke(null, null).ShouldBeAssignableTo<Control>()!;
        var form = new AtomUI.Desktop.Controls.Form();
        form.Items.Add(formItem);
        var window = new Window { Content = form };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            form.Items.Remove(formItem);
            Dispatcher.UIThread.RunJobs();
            window.Content = null;
            Dispatcher.UIThread.RunJobs();

            var bindingField = formItem.GetType().GetField(
                "_labelBinding",
                BindingFlags.Instance | BindingFlags.NonPublic);
            bindingField.ShouldNotBeNull();
            bindingField!.GetValue(formItem).ShouldBeNull();

        }
        finally
        {
            window.Close();
        }
    }
}

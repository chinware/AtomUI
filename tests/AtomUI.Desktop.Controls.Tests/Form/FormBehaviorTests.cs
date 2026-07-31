using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Form;

public class FormBehaviorTests
{
    static FormBehaviorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void FormItemDecorator_ExtraProperty_Is_Registered_On_Decorator()
    {
        FormItemDecorator.ExtraProperty.OwnerType.ShouldBe(typeof(FormItemDecorator));
    }

    [Fact]
    public void FormTheme_Binds_Validating_Feedback_To_Validating_Property()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/Form/Themes/FormTheme.axaml");

        source.ShouldContain("ValidatingFeedback=\"{Binding ValidatingFeedback}\"");
        source.ShouldNotContain("ValidatingFeedback=\"{Binding WarningFeedback}\"");
    }

    [Fact]
    public void Form_Default_ValidateTrigger_Is_OnChanged()
    {
        var form = new global::AtomUI.Desktop.Controls.Form();

        form.ValidateTrigger.ShouldBe(FormValidateTrigger.OnChanged);
    }

    [Fact]
    public async Task FormActionsItem_Without_Validators_Does_Not_Require_FormItemAware_Content()
    {
        var formItem = new FormActionsItem
        {
            Content = new Avalonia.Controls.Button()
        };

        await formItem.ValidateValueAsync(CancellationToken.None);

        formItem.ValidateStatus.ShouldBe(FormValidateStatus.Success);
        formItem.ValidateResult.ShouldBe(FormValidateResult.Success);
    }

    [Fact]
    public void FormValidatorProvider_Is_Compatible_With_FormItem_Validators()
    {
        var provider = new FormValidatorProvider();

        provider.ShouldBeAssignableTo<IList<IFormValidator>>();

        var validators = (IList<IFormValidator>)provider;
        validators.Add(new FormStringNotEmptyValidator());

        var formItem = new FormItem
        {
            Validators = validators
        };

        formItem.Validators.ShouldBeSameAs(provider);
        formItem.Validators.Count.ShouldBe(1);
    }

    [Fact]
    public void FormItem_Validates_On_Content_Change_When_Default_Trigger_Is_OnChanged()
    {
        var content = new FeedbackAwareFormControl();
        var formItem = new FormItem
        {
            OwnerForm  = new global::AtomUI.Desktop.Controls.Form(),
            LabelText  = "Name",
            FieldName  = "name",
            Content    = content,
            Validators = [new StaticValidator(FormValidateResult.Error)]
        };

        content.RaiseValueChanged();
        Dispatcher.UIThread.RunJobs();

        formItem.ValidateStatus.ShouldBe(FormValidateStatus.Error);
        formItem.ValidateResult.ShouldBe(FormValidateResult.Error);
    }

    [Fact]
    public async Task FormItem_Default_OnChanged_Revalidates_After_Submit_Error()
    {
        var content = new FeedbackAwareFormControl();
        var formItem = new FormItem
        {
            OwnerForm  = new global::AtomUI.Desktop.Controls.Form(),
            LabelText  = "Name",
            FieldName  = "name",
            Content    = content,
            Validators = [new FormStringNotEmptyValidator { Message = "required" }]
        };

        await formItem.ValidateValueAsync(CancellationToken.None);
        formItem.ValidateStatus.ShouldBe(FormValidateStatus.Error);
        DataValidationErrors.GetHasErrors(content).ShouldBeTrue();

        content.Value = "AtomUI";
        content.RaiseValueChanged();
        RunLayoutJobs();

        formItem.ValidateStatus.ShouldBe(FormValidateStatus.Success);
        formItem.ValidateResult.ShouldBe(FormValidateResult.Success);
        DataValidationErrors.GetHasErrors(content).ShouldBeFalse();
    }

    [Fact]
    public void FormItem_Explicit_OnSubmit_Does_Not_Validate_On_Content_Change()
    {
        var content = new FeedbackAwareFormControl();
        var formItem = new FormItem
        {
            OwnerForm       = new global::AtomUI.Desktop.Controls.Form(),
            LabelText       = "Name",
            FieldName       = "name",
            Content         = content,
            ValidateTrigger = FormValidateTrigger.OnSubmit,
            Validators      = [new StaticValidator(FormValidateResult.Error)]
        };

        content.RaiseValueChanged();
        RunLayoutJobs();

        formItem.ValidateStatus.ShouldBe(FormValidateStatus.Default);
        formItem.ValidateResult.ShouldBe(FormValidateResult.Success);
    }

    [Fact]
    public void FormItemDecorator_Replays_Feedback_To_New_Child_And_Releases_Old_Child()
    {
        var oldChild  = new FeedbackAwareFormControl();
        var newChild  = new FeedbackAwareFormControl();
        var feedback  = new FormValidateFeedback();
        var decorator = new FormItemDecorator
        {
            Child = oldChild
        };

        ((IFormItemFeedbackAware)decorator).SetFeedbackControl(feedback);
        oldChild.Feedback.ShouldBeSameAs(feedback);

        decorator.Child = newChild;

        oldChild.Feedback.ShouldBeNull();
        newChild.Feedback.ShouldBeSameAs(feedback);
    }

    [Fact]
    public void FormItem_Releases_Feedback_From_Old_Content_When_Content_Changes()
    {
        var oldContent = new FeedbackAwareFormControl();
        var newContent = new FeedbackAwareFormControl();
        var formItem = new FormItem
        {
            OwnerForm                 = new global::AtomUI.Desktop.Controls.Form(),
            IsValidateFeedbackEnabled = true,
            FeedbackTemplate          = new FuncDataTemplate<global::AtomUI.Desktop.Controls.Form>((_, _) => new FormValidateFeedback()),
            Content                   = oldContent
        };

        oldContent.Feedback.ShouldNotBeNull();

        formItem.Content = newContent;

        oldContent.Feedback.ShouldBeNull();
        newContent.Feedback.ShouldNotBeNull();
        newContent.Feedback.ShouldBeSameAs(oldContent.LastReleasedFeedback);
    }

    [Fact]
    public async Task FormItem_Ignores_Result_From_Canceled_Validation()
    {
        var firstResult  = new TaskCompletionSource<FormValidateResult>();
        var secondResult = new TaskCompletionSource<FormValidateResult>();
        var validator    = new QueuedValidator(firstResult.Task, secondResult.Task);
        var formItem = new FormItem
        {
            FieldName  = "name",
            LabelText  = "Name",
            Content    = new FeedbackAwareFormControl(),
            Validators = [validator]
        };
        using var firstCancellation = new CancellationTokenSource();

        var firstValidation = formItem.ValidateValueAsync(firstCancellation.Token);
        firstCancellation.Cancel();

        var secondValidation = formItem.ValidateValueAsync(CancellationToken.None);
        secondResult.SetResult(FormValidateResult.Success);
        await secondValidation.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        formItem.ValidateStatus.ShouldBe(FormValidateStatus.Success);

        firstResult.SetResult(FormValidateResult.Error);
        await firstValidation.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        formItem.ValidateStatus.ShouldBe(FormValidateStatus.Success);
    }

    [Fact]
    public async Task FormItem_Reset_Cancels_Pending_Validation_Result()
    {
        var result = new TaskCompletionSource<FormValidateResult>();
        var content = new FeedbackAwareFormControl();
        var formItem = new FormItem
        {
            FieldName  = "name",
            LabelText  = "Name",
            Content    = content,
            Validators = [new QueuedValidator(result.Task)]
        };

        var validation = formItem.ValidateValueAsync(CancellationToken.None);
        formItem.ValidateStatus.ShouldBe(FormValidateStatus.Validating);

        formItem.ResetItemValue();
        formItem.ValidateStatus.ShouldBe(FormValidateStatus.Default);
        content.LastValidateStatus.ShouldBe(FormValidateStatus.Default);

        result.SetResult(FormValidateResult.Error);
        await validation.WaitAsync(TimeSpan.FromSeconds(5), TestContext.Current.CancellationToken);

        formItem.ValidateStatus.ShouldBe(FormValidateStatus.Default);
        content.LastValidateStatus.ShouldBe(FormValidateStatus.Default);
    }

    [Fact]
    public async Task FormItem_Parallel_Validation_Allows_Validators_To_Return_Same_Task()
    {
        var result = Task.FromResult(FormValidateResult.Success);
        var formItem = new FormItem
        {
            FieldName        = "name",
            LabelText        = "Name",
            Content          = new FeedbackAwareFormControl(),
            ValidateStrategy = FormValidateStrategy.Parallel,
            Validators =
            [
                new SameTaskValidator(result),
                new SameTaskValidator(result)
            ]
        };

        await formItem.ValidateValueAsync(CancellationToken.None);

        formItem.ValidateStatus.ShouldBe(FormValidateStatus.Success);
    }

    [Fact]
    public async Task FormItem_StopWhenFirstFailed_Keeps_Warnings_Before_Error()
    {
        var formItem = new FormItem
        {
            FieldName        = "name",
            LabelText        = "Name",
            Content          = new FeedbackAwareFormControl(),
            ValidateStrategy = FormValidateStrategy.StopWhenFirstFailed,
            Validators =
            [
                new StaticValidator(FormValidateResult.Warning, "warning"),
                new StaticValidator(FormValidateResult.Error, "error"),
                new StaticValidator(FormValidateResult.Error, "skipped")
            ]
        };

        await formItem.ValidateValueAsync(CancellationToken.None);

        formItem.ValidateStatus.ShouldBe(FormValidateStatus.Error);
        formItem.ValidateWarningMessages.ShouldBe(["warning"]);
        formItem.ValidateErrorMessages.ShouldBe(["error"]);
    }

    [Fact]
    public async Task FormItem_Validator_Error_Is_Written_To_DataValidationErrors()
    {
        var content = new FeedbackAwareFormControl();
        var formItem = new FormItem
        {
            FieldName  = "name",
            LabelText  = "Name",
            Content    = content,
            Validators = [new StaticValidator(FormValidateResult.Error, "required")]
        };

        await formItem.ValidateValueAsync(CancellationToken.None);

        DataValidationErrors.GetHasErrors(content).ShouldBeTrue();
        var errors = DataValidationErrors.GetErrors(content)!.Select(error => error.ToString()).ToList();
        errors.Any(error => error is not null && error.Contains("required")).ShouldBeTrue();
    }

    [Fact]
    public async Task FormItem_Reset_Clears_Form_Owned_Errors_But_Preserves_External_DataValidationErrors()
    {
        var content       = new FeedbackAwareFormControl();
        var externalError = new InvalidOperationException("external");
        var formItem = new FormItem
        {
            FieldName  = "name",
            LabelText  = "Name",
            Content    = content,
            Validators = [new StaticValidator(FormValidateResult.Error, "required")]
        };
        DataValidationErrors.SetError(content, externalError);

        await formItem.ValidateValueAsync(CancellationToken.None);
        DataValidationErrors.GetErrors(content)!.Count().ShouldBe(2);

        formItem.ResetItemValue();

        var remainingErrors = DataValidationErrors.GetErrors(content)!.ToList();
        remainingErrors.ShouldBe([externalError]);
        DataValidationErrors.GetHasErrors(content).ShouldBeTrue();
    }

    [Fact]
    public async Task FormItem_External_DataValidationError_Takes_Priority_Over_Form_Warning()
    {
        var content       = new FeedbackAwareFormControl();
        var externalError = new InvalidOperationException("external");
        var formItem = new FormItem
        {
            FieldName  = "name",
            LabelText  = "Name",
            Content    = content,
            Validators = [new StaticValidator(FormValidateResult.Warning, "warning")]
        };
        FormValidateStatus? changedStatus = null;
        formItem.ValidateChanged += (_, args) => changedStatus = args.Status;
        DataValidationErrors.SetError(content, externalError);

        await formItem.ValidateValueAsync(CancellationToken.None);

        formItem.ValidateStatus.ShouldBe(FormValidateStatus.Error);
        formItem.ValidateResult.ShouldBe(FormValidateResult.Error);
        changedStatus.ShouldBe(FormValidateStatus.Error);
        formItem.ValidateWarningMessages.ShouldBe(["warning"]);
        formItem.ValidateErrorMessages.ShouldNotBeNull();
        formItem.ValidateErrorMessages!.ShouldContain(message => message.Contains("external"));
    }

    [Fact]
    public async Task SubmitButton_WatchValidateResult_Disables_Until_Form_Is_Valid()
    {
        var form = new global::AtomUI.Desktop.Controls.Form();
        var formItem = new FormItem
        {
            LabelText  = "Name",
            FieldName  = "name",
            Content    = new FeedbackAwareFormControl(),
            Validators = [new StaticValidator(FormValidateResult.Success)]
        };
        var submitButton = new SubmitButton
        {
            IsWatchValidateResult = true
        };

        form.Items.Add(formItem);
        form.Items.Add(new FormActionsItem
        {
            Content = submitButton
        });

        var window = CreateWindow(form);
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            submitButton.IsEnabled.ShouldBeFalse();

            await form.ValidateAsync(CancellationToken.None);
            Dispatcher.UIThread.RunJobs();

            submitButton.IsEnabled.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void FormItem_Uses_Configured_Columns_For_ExtraExtraExtraLarge_Breakpoint()
    {
        var form = new global::AtomUI.Desktop.Controls.Form
        {
            FormLayout          = FormLayout.Horizontal,
            LabelColInfo        = new MediaBreakGridLength(new GridLength(120)),
            WrapperColInfo      = new MediaBreakGridLength(GridLength.Star),
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        var formItem = new FormItem
        {
            LabelText           = "Name",
            HorizontalAlignment = HorizontalAlignment.Stretch,
            Content = new LineEdit
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            }
        };
        form.Items.Add(formItem);
        var mediaHost = new MediaBreakAwarePanel(MediaBreakPoint.ExtraExtraExtraLarge)
        {
            Width  = 728,
            Height = 80
        };
        mediaHost.Children.Add(form);
        var window = CreateWindow(mediaHost);

        try
        {
            window.Show();
            RunLayoutJobs();
            mediaHost.RaiseMediaBreakPointChanged();
            RunLayoutJobs();

            var bodyLayout = formItem.GetVisualDescendants()
                                     .OfType<Avalonia.Controls.Grid>()
                                     .Single(item => item.Name == "PART_BodyLayout");

            bodyLayout.ColumnDefinitions[0].Width.ShouldBe(new GridLength(120));
            bodyLayout.ColumnDefinitions[1].Width.ShouldBe(GridLength.Star);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void FormItem_LabelWrap_Keeps_Label_Text_Inside_Label_Column()
    {
        var form = new global::AtomUI.Desktop.Controls.Form
        {
            FormLayout          = FormLayout.Horizontal,
            LabelColInfo        = new MediaBreakGridLength(new GridLength(120)),
            WrapperColInfo      = new MediaBreakGridLength(GridLength.Star),
            LabelAlign          = FormLabelAlign.Left,
            LabelWrapping       = TextWrapping.Wrap,
            IsShowColon         = false,
            Width               = 600,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        var formItem = new FormItem
        {
            LabelText  = "A super long label text",
            FieldName  = "password",
            IsRequired = true,
            Content = new LineEdit
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            }
        };
        form.Items.Add(formItem);
        var window = CreateWindow(form);

        try
        {
            window.Width  = 720;
            window.Height = 240;
            window.Show();
            RunLayoutJobs();

            var bodyLayout     = FindVisualByName<Avalonia.Controls.Grid>(formItem, "PART_BodyLayout");
            var labelLayout    = FindVisualByName<Panel>(formItem, "PART_LabelLayout");
            var labelContent   = FindVisualByName<DockPanel>(formItem, "PART_LabelContentLayout");
            var label          = FindVisualByName<Avalonia.Controls.TextBlock>(formItem, "PART_Label");
            var contentLayout  = FindVisualByName<Panel>(formItem, "PART_ContentLayout");
            var labelLayoutPos = labelLayout.TranslatePoint(default, bodyLayout).ShouldNotBeNull();
            var labelPos       = label.TranslatePoint(default, bodyLayout).ShouldNotBeNull();
            var contentPos     = contentLayout.TranslatePoint(default, bodyLayout).ShouldNotBeNull();

            labelContent.HorizontalAlignment.ShouldBe(HorizontalAlignment.Left);
            label.Bounds.Height.ShouldBeGreaterThan(24);
            labelPos.X.ShouldBeGreaterThanOrEqualTo(labelLayoutPos.X);
            (labelPos.X + label.Bounds.Width).ShouldBeLessThanOrEqualTo(
                labelLayoutPos.X + labelLayout.Bounds.Width + 0.5);
            contentPos.X.ShouldBeGreaterThanOrEqualTo(
                labelLayoutPos.X + labelLayout.Bounds.Width - 0.5);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void FormItem_Default_LabelAlign_Keeps_Label_Content_Right_Aligned()
    {
        var form = new global::AtomUI.Desktop.Controls.Form
        {
            FormLayout          = FormLayout.Horizontal,
            LabelColInfo        = new MediaBreakGridLength(new GridLength(120)),
            WrapperColInfo      = new MediaBreakGridLength(GridLength.Star),
            Width               = 600,
            HorizontalAlignment = HorizontalAlignment.Left
        };
        var formItem = new FormItem
        {
            LabelText = "Name",
            FieldName = "name",
            Content = new LineEdit
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            }
        };
        form.Items.Add(formItem);
        var window = CreateWindow(form);

        try
        {
            window.Width  = 720;
            window.Height = 200;
            window.Show();
            RunLayoutJobs();

            var labelLayout       = FindVisualByName<Panel>(formItem, "PART_LabelLayout");
            var labelContent      = FindVisualByName<DockPanel>(formItem, "PART_LabelContentLayout");
            var labelContentRight = labelContent.TranslatePoint(
                new Point(labelContent.Bounds.Width, 0),
                labelLayout).ShouldNotBeNull();

            labelContent.HorizontalAlignment.ShouldBe(HorizontalAlignment.Right);
            labelContentRight.X.ShouldBe(labelLayout.Bounds.Width, 0.5);
        }
        finally
        {
            window.Close();
        }
    }

    private static string ReadRepoFile(string relativePath)
    {
        return File.ReadAllText(Path.Combine(GetRepositoryRoot(), relativePath));
    }

    private static string GetRepositoryRoot()
    {
        var directory = AppContext.BaseDirectory;
        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory, "AtomUI.slnx")))
            {
                return directory;
            }

            directory = Directory.GetParent(directory)?.FullName;
        }

        throw new DirectoryNotFoundException("Unable to locate repository root.");
    }

    private static AvaloniaWindow CreateWindow(Control content)
    {
        return new AvaloniaWindow
        {
            Width   = 420,
            Height  = 320,
            Content = content
        };
    }

    private static void RunLayoutJobs()
    {
        Dispatcher.UIThread.RunJobs();
        Dispatcher.UIThread.RunJobs();
    }

    private static T FindVisualByName<T>(Control root, string name)
        where T : Control
    {
        return root.GetVisualDescendants()
                   .OfType<T>()
                   .Single(item => item.Name == name);
    }

    private sealed class FeedbackAwareFormControl : Control, IFormItemAware, IFormItemFeedbackAware
    {
        public FormValidateFeedback? Feedback { get; private set; }
        public FormValidateFeedback? LastReleasedFeedback { get; private set; }
        public FormValidateStatus? LastValidateStatus { get; private set; }
        public object? Value { get; set; }
        private EventHandler? _valueChanged;

        public event EventHandler? ValueChanged
        {
            add => _valueChanged += value;
            remove => _valueChanged -= value;
        }

        public void SetFormValue(object? value)
        {
            Value = value;
        }

        public object? GetFormValue() => Value;

        public void ClearFormValue()
        {
            Value = null;
        }

        public void NotifyValidateStatus(FormValidateStatus status)
        {
            LastValidateStatus = status;
        }

        public void SetFeedbackControl(FormValidateFeedback? value)
        {
            if (value is null)
            {
                LastReleasedFeedback = Feedback;
            }

            Feedback = value;
        }

        public void RaiseValueChanged()
        {
            _valueChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private sealed class MediaBreakAwarePanel(MediaBreakPoint mediaBreakPoint) : Panel, IMediaBreakAwareControl
    {
        public MediaBreakPoint MediaBreakPoint { get; } = mediaBreakPoint;

        public event EventHandler<MediaBreakPointChangedEventArgs>? MediaBreakPointChanged;

        public void RaiseMediaBreakPointChanged()
        {
            MediaBreakPointChanged?.Invoke(this, new MediaBreakPointChangedEventArgs(MediaBreakPoint));
        }
    }

    private sealed class StaticValidator : IFormValidator
    {
        private readonly FormValidateResult _result;

        public StaticValidator(FormValidateResult result, string? message = "message")
        {
            _result  = result;
            Message = message;
        }

        public string? Message { get; }
        public bool WarningOnly => _result == FormValidateResult.Warning;

        public Task<FormValidateResult> ValidateAsync(string fieldName, object? value, CancellationToken cancellationToken)
        {
            return Task.FromResult(_result);
        }
    }

    private sealed class QueuedValidator : IFormValidator
    {
        private readonly Queue<Task<FormValidateResult>> _results;

        public QueuedValidator(params Task<FormValidateResult>[] results)
        {
            _results = new Queue<Task<FormValidateResult>>(results);
        }

        public string? Message => "message";
        public bool WarningOnly => false;

        public Task<FormValidateResult> ValidateAsync(string fieldName, object? value, CancellationToken cancellationToken)
        {
            return _results.Dequeue();
        }
    }

    private sealed class SameTaskValidator : IFormValidator
    {
        private readonly Task<FormValidateResult> _result;

        public SameTaskValidator(Task<FormValidateResult> result)
        {
            _result = result;
        }

        public string? Message => "message";
        public bool WarningOnly => false;

        public Task<FormValidateResult> ValidateAsync(string fieldName, object? value, CancellationToken cancellationToken)
        {
            return _result;
        }
    }

}

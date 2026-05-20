using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Layout;
using AtomDatePicker = AtomUI.Desktop.Controls.DatePicker;
using AtomCheckBox = AtomUI.Desktop.Controls.CheckBox;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateFormScenarios()
    {
        return
        [
            new PerfScenario("Form.Basic3", _ => CreateBasicForm(3)),
            new PerfScenario("Form.Basic10", _ => CreateBasicForm(10)),
            new PerfScenario("Form.NoLabel10", _ => CreateNoLabelForm(10)),
            new PerfScenario("Form.WithTooltip", _ => CreateTooltipForm()),
            new PerfScenario("Form.WithFeedback", _ => CreateFeedbackForm()),
            new PerfScenario("Form.WithDeleteButton", _ => CreateDeleteButtonForm()),
            new PerfScenario("Form.MixedInputs", _ => CreateMixedInputForm()),
            new PerfScenario("Form.GalleryShape", _ => CreateFormGalleryShape())
        ];
    }

    private static Form CreateBasicForm(int itemCount)
    {
        var form = CreateDefaultForm();
        for (var i = 0; i < itemCount; i++)
        {
            form.Items.Add(CreateLineEditFormItem($"Field {i + 1}", $"field{i + 1}", required: i % 2 == 0));
        }

        form.Items.Add(CreateSubmitActionsItem());
        return form;
    }

    private static Form CreateNoLabelForm(int itemCount)
    {
        var form = CreateDefaultForm();
        form.IsHideItemLabel = true;
        for (var i = 0; i < itemCount; i++)
        {
            form.Items.Add(CreateLineEditFormItem(null, $"field{i + 1}", required: false));
        }

        form.Items.Add(CreateSubmitActionsItem());
        return form;
    }

    private static Form CreateTooltipForm()
    {
        var form = CreateDefaultForm();
        form.Items.Add(CreateLineEditFormItem("Username", "username", required: true, tooltip: "Required username"));
        form.Items.Add(CreateLineEditFormItem("Nickname", "nickname", required: false, tooltip: "Shown to other users"));
        form.Items.Add(new FormItem
        {
            LabelText   = "Custom icon",
            FieldName   = "customIcon",
            Tooltip     = "Custom tooltip icon",
            TooltipIcon = new InfoCircleOutlined(),
            Content     = new LineEdit()
        });
        form.Items.Add(CreateSubmitActionsItem());
        return form;
    }

    private static Form CreateFeedbackForm()
    {
        var form = CreateDefaultForm();
        form.IsValidateFeedbackEnabled = true;
        form.Items.Add(CreateLineEditFormItem("On blur", "fieldA", required: true));
        form.Items.Add(new FormItem
        {
            LabelText        = "Debounce",
            FieldName        = "fieldB",
            ValidateDebounce = TimeSpan.FromMilliseconds(200),
            Validators       = CreateRequiredValidators("Please input fieldB."),
            Content          = new LineEdit()
        });
        form.Items.Add(CreateSubmitActionsItem());
        return form;
    }

    private static Form CreateDeleteButtonForm()
    {
        var form = CreateDefaultForm();
        form.IsShowItemDeleteButton = true;
        form.MinItemCount           = 1;
        for (var i = 0; i < 4; i++)
        {
            form.Items.Add(CreateLineEditFormItem($"Passenger {i + 1}", $"passenger{i + 1}", required: true));
        }

        form.Items.Add(CreateSubmitActionsItem());
        return form;
    }

    private static Form CreateMixedInputForm()
    {
        var form = CreateDefaultForm();
        form.Items.Add(CreateLineEditFormItem("Input", "input", required: true));
        form.Items.Add(new FormItem
        {
            LabelText = "Password",
            FieldName = "password",
            Content = new LineEdit
            {
                RevealPassword       = false,
                PasswordChar         = '*',
                IsEnableRevealButton = true
            }
        });
        form.Items.Add(new FormItem
        {
            LabelText = "Select",
            FieldName = "select",
            Content = CreateSmallSelect()
        });
        form.Items.Add(new FormItem
        {
            LabelText = "Cascader",
            FieldName = "cascader",
            Content = CreateSmallCascader()
        });
        form.Items.Add(new FormItem
        {
            LabelText = "TreeSelect",
            FieldName = "treeSelect",
            Content = CreateSmallTreeSelect()
        });
        form.Items.Add(new FormItem
        {
            LabelText = "Date",
            FieldName = "date",
            Content = new AtomDatePicker
            {
                PickerPlacement = PlacementMode.Bottom
            }
        });
        form.Items.Add(new FormItem
        {
            LabelText = "CheckBox",
            FieldName = "check",
            Content = new AtomCheckBox
            {
                Content = "Remember me"
            }
        });
        form.Items.Add(CreateSubmitActionsItem());
        return form;
    }

    private static Control CreateFormGalleryShape()
    {
        var root = new StackPanel
        {
            Spacing = 18
        };

        root.Children.Add(CreateBasicForm(3));
        root.Children.Add(CreateBasicForm(6));
        root.Children.Add(CreateNoLabelForm(4));
        root.Children.Add(CreateTooltipForm());
        root.Children.Add(CreateFeedbackForm());
        root.Children.Add(CreateDeleteButtonForm());
        root.Children.Add(CreateMixedInputForm());
        root.Children.Add(CreateInlineLoginForm());
        return root;
    }

    private static Form CreateInlineLoginForm()
    {
        var form = CreateDefaultForm();
        form.FormLayout = FormLayout.Inline;
        form.Items.Add(new FormItem
        {
            LabelText = "Username",
            FieldName = "username",
            Content = new LineEdit
            {
                Width = 200,
                InnerLeftContent = new UserOutlined()
            }
        });
        form.Items.Add(new FormItem
        {
            LabelText = "Password",
            FieldName = "password",
            Content = new LineEdit
            {
                Width = 200,
                InnerLeftContent = new LockOutlined()
            }
        });
        form.Items.Add(new FormActionsItem
        {
            Content = new SubmitButton
            {
                Content = "Log in"
            }
        });
        return form;
    }

    private static Form CreateDefaultForm()
    {
        return new Form
        {
            LabelColInfo        = new MediaBreakGridLength(new GridLength(8, GridUnitType.Star)),
            WrapperColInfo      = new MediaBreakGridLength(new GridLength(16, GridUnitType.Star)),
            MinWidth            = 600,
            HorizontalAlignment = HorizontalAlignment.Left
        };
    }

    private static FormItem CreateLineEditFormItem(string? label,
                                                   string fieldName,
                                                   bool required,
                                                   string? tooltip = null)
    {
        return new FormItem
        {
            LabelText  = label,
            FieldName  = fieldName,
            IsRequired = required,
            Tooltip    = tooltip,
            Validators = required ? CreateRequiredValidators($"Please input {fieldName}.") : null,
            Content    = new LineEdit()
        };
    }

    private static IList<IFormValidator> CreateRequiredValidators(string message)
    {
        return
        [
            new FormStringNotEmptyValidator
            {
                Message = message
            }
        ];
    }

    private static FormActionsItem CreateSubmitActionsItem()
    {
        return new FormActionsItem
        {
            Content = new SubmitButton()
        };
    }

    private static Select CreateSmallSelect()
    {
        var select = new Select
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            IsAllowClear        = true,
            OptionsSource       =
            [
                new SelectOption { Header = "Apple", Content = "apple" },
                new SelectOption { Header = "Pear", Content = "pear" }
            ]
        };
        return select;
    }

    private static Cascader CreateSmallCascader()
    {
        var cascader = new Cascader
        {
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        var option = new CascaderOption
        {
            Header = "Zhejiang",
            Value  = "zhejiang",
            Children =
            [
                new CascaderOption
                {
                    Header = "Hangzhou",
                    Value  = "hangzhou"
                }
            ]
        };
        cascader.OptionsSource = new List<ICascaderOption> { option };
        return cascader;
    }

    private static TreeSelect CreateSmallTreeSelect()
    {
        var treeSelect = new TreeSelect
        {
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        var node = new TreeItemNode
        {
            Header = "Light",
            Value  = "light",
            Children =
            [
                new TreeItemNode
                {
                    Header = "Bamboo",
                    Value  = "bamboo"
                }
            ]
        };
        treeSelect.ItemsSource = [node];
        return treeSelect;
    }
}

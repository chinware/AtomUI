using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Shouldly;
using Xunit;
using AtomSizeType = AtomUI.SizeType;

namespace AtomUI.Desktop.Controls.Tests.Form;

public class FormCustomizableSizeTypeTests
{
    public FormCustomizableSizeTypeTests()
    {
        global::AtomUI.Desktop.Controls.Tests.AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void FormItem_Maps_Custom_Size_To_Middle_For_Legacy_Size_Aware_Content()
    {
        var content = new LegacySizeAwareFormControl();
        var formItem = new FormItem
        {
            SizeType = CustomizableSizeType.Large,
            Content  = content
        };

        content.SizeType.ShouldBe(AtomSizeType.Large);

        formItem.SizeType = CustomizableSizeType.Custom;

        content.SizeType.ShouldBe(AtomSizeType.Middle);
    }

    [Fact]
    public void FormItem_Relays_Customizable_Size_To_Customizable_Size_Aware_Content()
    {
        var content = new CustomizableSizeAwareFormControl();
        var formItem = new FormItem
        {
            SizeType = CustomizableSizeType.Small,
            Content  = content
        };

        content.SizeType.ShouldBe(CustomizableSizeType.Small);

        formItem.SizeType = CustomizableSizeType.Custom;

        content.SizeType.ShouldBe(CustomizableSizeType.Custom);
    }

    [Fact]
    public void FormItemDecorator_Maps_Custom_Size_To_Middle_For_Legacy_Size_Aware_Child()
    {
        var child = new LegacySizeAwareFormControl();
        var decorator = new FormItemDecorator
        {
            SizeType = CustomizableSizeType.Small,
            Child    = child
        };

        child.SizeType.ShouldBe(AtomSizeType.Small);

        decorator.SizeType = CustomizableSizeType.Custom;

        child.SizeType.ShouldBe(AtomSizeType.Middle);
    }

    [Fact]
    public void FormItemDecorator_Relays_Customizable_Size_To_Customizable_Size_Aware_Child()
    {
        var child = new CustomizableSizeAwareFormControl();
        var decorator = new FormItemDecorator
        {
            SizeType = CustomizableSizeType.Large,
            Child    = child
        };

        child.SizeType.ShouldBe(CustomizableSizeType.Large);

        decorator.SizeType = CustomizableSizeType.Custom;

        child.SizeType.ShouldBe(CustomizableSizeType.Custom);
    }

    private sealed class LegacySizeAwareFormControl : Control, IFormItemAware, ISizeTypeAware
    {
        public static readonly StyledProperty<AtomSizeType> SizeTypeProperty =
            SizeTypeControlProperty.SizeTypeProperty.AddOwner<LegacySizeAwareFormControl>();

        public AtomSizeType SizeType
        {
            get => GetValue(SizeTypeProperty);
            set => SetValue(SizeTypeProperty, value);
        }

        public event EventHandler? ValueChanged
        {
            add { }
            remove { }
        }

        public void SetFormValue(object? value)
        {
        }

        public object? GetFormValue() => null;

        public void ClearFormValue()
        {
        }

        public void NotifyValidateStatus(FormValidateStatus status)
        {
        }
    }

    private sealed class CustomizableSizeAwareFormControl : Control, IFormItemAware, ICustomizableSizeTypeAware
    {
        public static readonly StyledProperty<CustomizableSizeType> SizeTypeProperty =
            CustomizableSizeTypeControlProperty.SizeTypeProperty.AddOwner<CustomizableSizeAwareFormControl>();

        public CustomizableSizeType SizeType
        {
            get => GetValue(SizeTypeProperty);
            set => SetValue(SizeTypeProperty, value);
        }

        public event EventHandler? ValueChanged
        {
            add { }
            remove { }
        }

        public void SetFormValue(object? value)
        {
        }

        public object? GetFormValue() => null;

        public void ClearFormValue()
        {
        }

        public void NotifyValidateStatus(FormValidateStatus status)
        {
        }
    }
}

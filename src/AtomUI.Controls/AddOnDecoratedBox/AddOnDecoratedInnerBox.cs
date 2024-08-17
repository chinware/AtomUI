using AtomUI.Controls.Utils;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

[TemplatePart(AddOnDecoratedInnerBoxTheme.ContentPresenterPart, typeof(ContentPresenter), IsRequired = true)]
public abstract class AddOnDecoratedInnerBox : ContentControl
{
   #region 公共属性定义

   public static readonly StyledProperty<SizeType> SizeTypeProperty =
      AddOnDecoratedBox.SizeTypeProperty.AddOwner<AddOnDecoratedInnerBox>();   
   
   public static readonly StyledProperty<object?> LeftAddOnContentProperty =
      AvaloniaProperty.Register<AddOnDecoratedInnerBox, object?>(nameof(LeftAddOnContent));

   public static readonly StyledProperty<object?> RightAddOnContentProperty =
      AvaloniaProperty.Register<AddOnDecoratedInnerBox, object?>(nameof(RightAddOnContent));

   public static readonly StyledProperty<bool> IsClearButtonVisibleProperty =
      AvaloniaProperty.Register<AddOnDecoratedInnerBox, bool>(nameof(IsClearButtonVisible));
   
   public SizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }
   
   public object? LeftAddOnContent
   {
      get => GetValue(LeftAddOnContentProperty);
      set => SetValue(LeftAddOnContentProperty, value);
   }

   public object? RightAddOnContent
   {
      get => GetValue(RightAddOnContentProperty);
      set => SetValue(RightAddOnContentProperty, value);
   }

   public bool IsClearButtonVisible
   {
      get => GetValue(IsClearButtonVisibleProperty);
      set => SetValue(IsClearButtonVisibleProperty, value);
   }

   #endregion

   #region 内部属性定义
   
   internal static readonly DirectProperty<AddOnDecoratedInnerBox, Thickness> ContentPresenterMarginProperty =
      AvaloniaProperty.RegisterDirect<AddOnDecoratedInnerBox, Thickness>(nameof(ContentPresenterMargin),
                                                                         o => o.ContentPresenterMargin,
                                                                         (o, v) => o.ContentPresenterMargin = v);
   
   private static readonly DirectProperty<AddOnDecoratedInnerBox, double> MarginXSTokenProperty =
      AvaloniaProperty.RegisterDirect<AddOnDecoratedInnerBox, double>(nameof(MarginXSToken),
                                                                         o => o.MarginXSToken,
                                                                         (o, v) => o.MarginXSToken = v);
   
   private double _marginXSToken;

   private double MarginXSToken
   {
      get => _marginXSToken;
      set => SetAndRaise(MarginXSTokenProperty, ref _marginXSToken, value);
   }

   private Thickness _contentPresenterMargin;

   internal Thickness ContentPresenterMargin
   {
      get => _contentPresenterMargin;
      set => SetAndRaise(ContentPresenterMarginProperty, ref _contentPresenterMargin, value);
   }
   
   #endregion

   private StackPanel? _leftAddOnLayout;
   private StackPanel? _rightAddOnLayout;
   private IconButton? _clearButton;

   protected virtual void NotifyClearButtonClicked()
   {
   }
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);

      if (change.Property == LeftAddOnContentProperty || change.Property == RightAddOnContentProperty) {
         if (change.OldValue is Control oldControl) {
            UIStructureUtils.SetTemplateParent(oldControl, null);
         }

         if (change.NewValue is Control newControl) {
            UIStructureUtils.SetTemplateParent(newControl, this);
         }
      }
   }
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      TokenResourceBinder.CreateGlobalResourceBinding(this, MarginXSTokenProperty, GlobalResourceKey.MarginXS);
      _leftAddOnLayout = e.NameScope.Find<StackPanel>(AddOnDecoratedInnerBoxTheme.LeftAddOnLayoutPart);
      _rightAddOnLayout = e.NameScope.Find<StackPanel>(AddOnDecoratedInnerBoxTheme.RightAddOnLayoutPart);
      
      _clearButton = e.NameScope.Find<IconButton>(AddOnDecoratedInnerBoxTheme.ClearButtonPart);

      if (_leftAddOnLayout is not null) {
         _leftAddOnLayout.SizeChanged += HandleLayoutSizeChanged;
      }
      if (_rightAddOnLayout is not null) {
         _rightAddOnLayout.SizeChanged += HandleLayoutSizeChanged;
      }
      
      if (_clearButton is not null) {
         _clearButton.Click += (sender, args) =>
         {
            NotifyClearButtonClicked();
         };
      }
      
      SetupContentPresenterMargin();
   }

   private void HandleLayoutSizeChanged(object? sender, SizeChangedEventArgs args)
   {
      SetupContentPresenterMargin();
   }

   private void SetupContentPresenterMargin()
   {
      var marginLeft = 0d;
      var marginRight = 0d;
      if (_leftAddOnLayout is not null) {
         if (_leftAddOnLayout.DesiredSize.Width > 0 && _leftAddOnLayout.DesiredSize.Height > 0) {
            marginLeft = _marginXSToken;
         }
      }
      if (_rightAddOnLayout is not null) {
         if (_rightAddOnLayout.DesiredSize.Width > 0 && _rightAddOnLayout.DesiredSize.Height > 0) {
            marginRight = _marginXSToken;
         }
      }

      ContentPresenterMargin = new Thickness(marginLeft, 0, marginRight, 0);
   }
}
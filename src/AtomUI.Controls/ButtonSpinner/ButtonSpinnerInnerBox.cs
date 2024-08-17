using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Rendering;

namespace AtomUI.Controls;

[TemplatePart(ButtonSpinnerInnerBoxTheme.SpinnerHandlePart, typeof(ContentPresenter))]
internal class ButtonSpinnerInnerBox : AddOnDecoratedInnerBox, ICustomHitTest
{
   #region 公共属性定义

   public static readonly StyledProperty<object?> SpinnerContentProperty =
      AvaloniaProperty.Register<ButtonSpinnerInnerBox, object?>(nameof(SpinnerContent));

   public object? SpinnerContent
   {
      get => GetValue(SpinnerContentProperty);
      set => SetValue(SpinnerContentProperty, value);
   }

   #endregion

   #region 内部属性定义

   internal static readonly DirectProperty<ButtonSpinnerInnerBox, Location> ButtonSpinnerLocationProperty =
      AvaloniaProperty.RegisterDirect<ButtonSpinnerInnerBox, Location>(nameof(ButtonSpinnerLocation),
                                                                       o => o.ButtonSpinnerLocation,
                                                                       (o, v) => o.ButtonSpinnerLocation = v);

   internal static readonly DirectProperty<ButtonSpinnerInnerBox, bool> ShowButtonSpinnerProperty =
      AvaloniaProperty.RegisterDirect<ButtonSpinnerInnerBox, bool>(nameof(ShowButtonSpinner),
                                                                   o => o.ShowButtonSpinner,
                                                                   (o, v) => o.ShowButtonSpinner = v);
   
   internal static readonly DirectProperty<ButtonSpinnerInnerBox, Thickness> SpinnerBorderThicknessProperty =
      AvaloniaProperty.RegisterDirect<ButtonSpinnerInnerBox, Thickness>(nameof(SpinnerBorderThickness),
                                                                   o => o.SpinnerBorderThickness,
                                                                   (o, v) => o.SpinnerBorderThickness = v);
   
   internal static readonly DirectProperty<ButtonSpinnerInnerBox, IBrush?> SpinnerBorderBrushProperty =
      AvaloniaProperty.RegisterDirect<ButtonSpinnerInnerBox, IBrush?>(nameof(SpinnerBorderBrush),
                                                                      o => o.SpinnerBorderBrush,
                                                                      (o, v) => o.SpinnerBorderBrush = v);

   private bool _showButtonSpinner;

   internal bool ShowButtonSpinner
   {
      get => _showButtonSpinner;
      set => SetAndRaise(ShowButtonSpinnerProperty, ref _showButtonSpinner, value);
   }

   private Location _buttonSpinnerLocation;

   internal Location ButtonSpinnerLocation
   {
      get => _buttonSpinnerLocation;
      set => SetAndRaise(ButtonSpinnerLocationProperty, ref _buttonSpinnerLocation, value);
   }

   private Thickness _spinnerBorderThickness;

   internal Thickness SpinnerBorderThickness
   {
      get => _spinnerBorderThickness;
      set => SetAndRaise(SpinnerBorderThicknessProperty, ref _spinnerBorderThickness, value);
   }
   
   private IBrush? _spinnerBorderBrush;

   internal IBrush? SpinnerBorderBrush
   {
      get => _spinnerBorderBrush;
      set => SetAndRaise(SpinnerBorderBrushProperty, ref _spinnerBorderBrush, value);
   }
   
   #endregion

   private ContentPresenter? _handleContentPresenter;
   
   public bool HitTest(Point point)
   {
      return true;
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _handleContentPresenter = e.NameScope.Find<ContentPresenter>(ButtonSpinnerInnerBoxTheme.SpinnerHandlePart);
      TokenResourceBinder.CreateGlobalTokenBinding(this, SpinnerBorderThicknessProperty, GlobalResourceKey.BorderThickness, BindingPriority.Template,
         new RenderScaleAwareThicknessConfigure(this));
      TokenResourceBinder.CreateGlobalTokenBinding(this, SpinnerBorderBrushProperty, GlobalResourceKey.ColorBorder);
   }

   public override void Render(DrawingContext context)
   {
      if (_handleContentPresenter is not null) {
         Console.WriteLine(_handleContentPresenter.TranslatePoint(new Point(0, 0), this));
      }
   }
}
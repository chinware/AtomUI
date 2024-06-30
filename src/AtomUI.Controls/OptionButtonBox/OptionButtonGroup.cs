using System.Collections.Specialized;
using AtomUI.Data;
using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace AtomUI.Controls;

using ButtonSizeType = SizeType;
using OptionButtons = AvaloniaList<OptionButton>;

public partial class OptionButtonGroup : StyledControl, ISizeTypeAware
{
   public static readonly StyledProperty<ButtonSizeType> SizeTypeProperty =
      AvaloniaProperty.Register<OptionButtonGroup, ButtonSizeType>(nameof(SizeType), ButtonSizeType.Middle);

   public static readonly StyledProperty<OptionButtonStyle> ButtonStyleProperty =
      AvaloniaProperty.Register<OptionButtonGroup, OptionButtonStyle>(nameof(SizeType), OptionButtonStyle.Outline);
   
   /// <summary>
   /// Defines the <see cref="CornerRadius"/> property.
   /// </summary>
   public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
      AvaloniaProperty.Register<OptionButtonGroup, CornerRadius>(nameof(CornerRadius));
   
   /// <summary>
   /// Defines the <see cref="BorderThickness"/> property.
   /// </summary>
   public static readonly StyledProperty<Thickness> BorderThicknessProperty =
      AvaloniaProperty.Register<StyledControl, Thickness>(nameof(BorderThickness));
   
   
   public static readonly DirectProperty<OptionButtonGroup, OptionButton?> SelectedOptionProperty =
      AvaloniaProperty.RegisterDirect<OptionButtonGroup, OptionButton?>(nameof(SelectedOption),
         o => o.SelectedOption,
         (o, v) => o.SelectedOption = v);
   
   public ButtonSizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }
   
   public OptionButtonStyle ButtonStyle
   {
      get => GetValue(ButtonStyleProperty);
      set => SetValue(ButtonStyleProperty, value);
   }
   
   /// <summary>
   /// Gets or sets the radius of the border rounded corners.
   /// </summary>
   public CornerRadius CornerRadius
   {
      get => GetValue(CornerRadiusProperty);
      set => SetValue(CornerRadiusProperty, value);
   }
   
   /// <summary>
   /// Gets or sets the thickness of the border.
   /// </summary>
   public Thickness BorderThickness
   {
      get => GetValue(BorderThicknessProperty);
      set => SetValue(BorderThicknessProperty, value);
   }

   private OptionButton? _optionButton;
   public OptionButton? SelectedOption
   {
      get => _optionButton;
      set => SetAndRaise(SelectedOptionProperty, ref _optionButton, value);
   }

   [Content] public OptionButtons Options { get; } = new OptionButtons();

   static OptionButtonGroup()
   {
      AffectsMeasure<OptionButtonGroup>(SizeTypeProperty);
      AffectsRender<OptionButtonGroup>(SelectedOptionProperty, SelectedOptionBorderColorProperty,
         ButtonStyleProperty);
   }

   public OptionButtonGroup()
   {
      _controlTokenBinder = new ControlTokenBinder(this, OptionButtonToken.ID);
      _customStyle = this;
      _customStyle.InitOnConstruct();
      Options.CollectionChanged += OptionsChanged;
   }

   protected virtual void OptionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
   {
      switch (e.Action) {
         case NotifyCollectionChangedAction.Add:
            var newOptions = e.NewItems!.OfType<OptionButton>().ToList();
            ApplyInButtonGroupFlag(newOptions, true);
            _layout!.Children.AddRange(newOptions);
            break;

         case NotifyCollectionChangedAction.Move:
            _layout!.Children.MoveRange(e.OldStartingIndex, e.OldItems!.Count, e.NewStartingIndex);
            break;

         case NotifyCollectionChangedAction.Remove:
            var removedOptions = e.OldItems!.OfType<OptionButton>().ToList();
            ApplyInButtonGroupFlag(removedOptions, false);
            _layout!.Children.RemoveAll(removedOptions);
            break;

         case NotifyCollectionChangedAction.Replace:
            for (var i = 0; i < e.OldItems!.Count; ++i) {
               var index = i + e.OldStartingIndex;
               var oldChild = (OptionButton)e.OldItems![i]!;
               oldChild.InOptionGroup = false;
               var child = (OptionButton)e.NewItems![i]!;
               child.InOptionGroup = true;
               _layout!.Children[index] = child;
            }
            break;

         case NotifyCollectionChangedAction.Reset:
            throw new NotSupportedException();
      }

      UpdateOptionButtonsPosition();
      InvalidateMeasureOnOptionsChanged();
   }

   private void UpdateOptionButtonsPosition()
   {
      for (int i = 0; i < Options.Count; i++) {
         var button = Options[i];
         if (Options.Count > 1) {
            if (i == 0) {
               button.GroupPositionTrait = OptionButtonPositionTrait.First;
            } else if (i == Options.Count - 1) {
               button.GroupPositionTrait = OptionButtonPositionTrait.Last;
            } else {
               button.GroupPositionTrait = OptionButtonPositionTrait.Middle;
            }
         }
      }
   }

   private void ApplyInButtonGroupFlag(List<OptionButton> buttons, bool inGroup)
   {
      for (int i = 0; i < buttons.Count; i++) {
         var button = buttons[i];
         button.InOptionGroup = inGroup;
         if (inGroup) {
            button.IsCheckedChanged += HandleOptionSelected;
            button.OptionButtonPointerEvent += HandleOptionPointerEvent;
         } else {
            button.IsCheckedChanged -= HandleOptionSelected;
            button.OptionButtonPointerEvent -= HandleOptionPointerEvent;
            button.GroupPositionTrait = OptionButtonPositionTrait.OnlyOne;
         }
      }
   }

   private void HandleOptionSelected(object? sender, RoutedEventArgs args)
   {
      if (sender is OptionButton optionButton) {
         if (optionButton.IsChecked.HasValue && optionButton.IsChecked.Value) {
            SelectedOption = optionButton;
         }
      }
   }

   private protected virtual void InvalidateMeasureOnOptionsChanged()
   {
      InvalidateMeasure();
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      var targetWidth = size.Width;
      var targetHeight = Math.Max(size.Height, _controlHeight);
      return new Size(targetWidth, targetHeight);
   }
   
   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         _initialized = true;
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      _customStyle.HandlePropertyChangedForStyle(e);
   }

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      _customStyle.ApplyRenderScalingAwareStyleConfig();
   }
}
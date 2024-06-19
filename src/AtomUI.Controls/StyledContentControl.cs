using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public class StyledContentControl : BorderedStyleControl
{
   /// <summary>
   /// Defines the <see cref="TextAlignment"/> property
   /// </summary>
   public static readonly StyledProperty<TextAlignment> TextAlignmentProperty =
      TextBlock.TextAlignmentProperty.AddOwner<ContentPresenter>();

   /// <summary>
   /// Defines the <see cref="TextWrapping"/> property
   /// </summary>
   public static readonly StyledProperty<TextWrapping> TextWrappingProperty =
      TextBlock.TextWrappingProperty.AddOwner<ContentPresenter>();

   /// <summary>
   /// Defines the <see cref="TextTrimming"/> property
   /// </summary>
   public static readonly StyledProperty<TextTrimming> TextTrimmingProperty =
      TextBlock.TextTrimmingProperty.AddOwner<ContentPresenter>();

   /// <summary>
   /// Defines the <see cref="LineHeight"/> property
   /// </summary>
   public static readonly StyledProperty<double> LineHeightProperty =
      TextBlock.LineHeightProperty.AddOwner<ContentPresenter>();

   /// <summary>
   /// Defines the <see cref="MaxLines"/> property
   /// </summary>
   public static readonly StyledProperty<int> MaxLinesProperty =
      TextBlock.MaxLinesProperty.AddOwner<ContentPresenter>();
   
   /// <summary>
   /// Defines the <see cref="Content"/> property.
   /// </summary>
   public static readonly StyledProperty<object?> ContentProperty =
      AvaloniaProperty.Register<StyledContentControl, object?>(nameof(Content));

   /// <summary>
   /// Defines the <see cref="ContentTemplate"/> property.
   /// </summary>
   public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty =
      ContentControl.ContentTemplateProperty.AddOwner<ContentPresenter>();

   /// <summary>
   /// Defines the <see cref="HorizontalContentAlignment"/> property.
   /// </summary>
   public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
      AvaloniaProperty.Register<ContentControl, HorizontalAlignment>(nameof(HorizontalContentAlignment));

   /// <summary>
   /// Defines the <see cref="VerticalContentAlignment"/> property.
   /// </summary>
   public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
      AvaloniaProperty.Register<ContentControl, VerticalAlignment>(nameof(VerticalContentAlignment));

   /// <summary>
   /// Defines the <see cref="RecognizesAccessKey"/> property
   /// </summary>
   public static readonly StyledProperty<bool> RecognizesAccessKeyProperty =
      AvaloniaProperty.Register<ContentPresenter, bool>(nameof(RecognizesAccessKey));

   /// <summary>
   /// Gets or sets the content to display.
   /// </summary>
   [Content]
   public object? Content
   {
      get => GetValue(ContentProperty);
      set => SetValue(ContentProperty, value);
   }

   /// <summary>
   /// Gets or sets the data template used to display the content of the control.
   /// </summary>
   public IDataTemplate? ContentTemplate
   {
      get => GetValue(ContentTemplateProperty);
      set => SetValue(ContentTemplateProperty, value);
   }
   
   /// <summary>
   /// Gets or sets the horizontal alignment of the content within the control.
   /// </summary>
   public HorizontalAlignment HorizontalContentAlignment
   {
      get => GetValue(HorizontalContentAlignmentProperty);
      set => SetValue(HorizontalContentAlignmentProperty, value);
   }

   /// <summary>
   /// Gets or sets the vertical alignment of the content within the control.
   /// </summary>
   public VerticalAlignment VerticalContentAlignment
   {
      get => GetValue(VerticalContentAlignmentProperty);
      set => SetValue(VerticalContentAlignmentProperty, value);
   }


   /// <summary>
   /// Determine if <see cref="ContentPresenter"/> should use <see cref="AccessText"/> in its style
   /// </summary>
   public bool RecognizesAccessKey
   {
      get => GetValue(RecognizesAccessKeyProperty);
      set => SetValue(RecognizesAccessKeyProperty, value);
   }
   
   private Control? _child;
   private bool _createdChild;
   private IRecyclingDataTemplate? _recyclingDataTemplate;

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      _recyclingDataTemplate = null;
      _createdChild = false;
      InvalidateMeasure();
   }

   public void UpdateChild()
   {
      var content = Content;
      UpdateChild(content);
   }

   private void UpdateChild(object? content)
   {
      var contentTemplate = ContentTemplate;
      var oldChild = Child;
      var newChild = CreateChild(content, oldChild, contentTemplate);

      // Remove the old child if we're not recycling it.
      if (newChild != oldChild) {
         if (oldChild != null) {
            VisualChildren.Remove(oldChild);
            LogicalChildren.Remove(oldChild);
            ((ISetInheritanceParent)oldChild).SetParent(oldChild.Parent);
         }
      }

      // Set the DataContext if the data isn't a control.
      if (contentTemplate is { } || !(content is Control)) {
         DataContext = content;
      } else {
         ClearValue(DataContextProperty);
      }

      // Update the Child.
      if (newChild == null) {
         Child = null;
      } else if (newChild != oldChild) {
         ((ISetInheritanceParent)newChild).SetParent(this);
         Child = newChild;

         if (!LogicalChildren.Contains(newChild)) {
            LogicalChildren.Add(newChild);
         }

         VisualChildren.Add(newChild);
      }

      _createdChild = true;
   }

   private Control? CreateChild(object? content, Control? oldChild, IDataTemplate? template)
   {
      var newChild = content as Control;

      // We want to allow creating Child from the Template, if Content is null.
      // But it's important to not use DataTemplates, otherwise we will break content presenters in many places,
      // otherwise it will blow up every ContentPresenter without Content set.
      if ((newChild == null
           && (content != null || template != null)) || (newChild is { } && template is { })) {
         var dataTemplate = this.FindDataTemplate(content, template) ??
                            (
                               RecognizesAccessKey
                                  ? FuncDataTemplate.Access
                                  : FuncDataTemplate.Default
                            );

         if (dataTemplate is IRecyclingDataTemplate rdt) {
            var toRecycle = rdt == _recyclingDataTemplate ? oldChild : null;
            newChild = rdt.Build(content, toRecycle);
            _recyclingDataTemplate = rdt;
         } else {
            newChild = dataTemplate.Build(content);
            _recyclingDataTemplate = null;
         }
      } else {
         _recyclingDataTemplate = null;
      }

      return newChild;
   }
   
   /// <inheritdoc/>
   protected override Size ArrangeOverride(Size finalSize)
   {
      return ArrangeOverrideImpl(finalSize, new Vector());
   }

   internal Size ArrangeOverrideImpl(Size finalSize, Vector offset)
   {
      if (Child == null) {
         return finalSize;
      }
      var useLayoutRounding = UseLayoutRounding;
      var scale = LayoutHelper.GetLayoutScale(this);
      var padding = Padding;
      var borderThickness = BorderThickness;

      if (useLayoutRounding) {
         padding = LayoutHelper.RoundLayoutThickness(padding, scale, scale);
         borderThickness = LayoutHelper.RoundLayoutThickness(borderThickness, scale, scale);
      }

      padding += borderThickness;
      var horizontalContentAlignment = HorizontalContentAlignment;
      var verticalContentAlignment = VerticalContentAlignment;
      var availableSize = finalSize;
      var sizeForChild = availableSize;
      var originX = offset.X;
      var originY = offset.Y;

      if (horizontalContentAlignment != HorizontalAlignment.Stretch) {
         sizeForChild = sizeForChild.WithWidth(Math.Min(sizeForChild.Width, DesiredSize.Width));
      }

      if (verticalContentAlignment != VerticalAlignment.Stretch) {
         sizeForChild = sizeForChild.WithHeight(Math.Min(sizeForChild.Height, DesiredSize.Height));
      }

      if (useLayoutRounding) {
         sizeForChild = LayoutHelper.RoundLayoutSizeUp(sizeForChild, scale, scale);
         availableSize = LayoutHelper.RoundLayoutSizeUp(availableSize, scale, scale);
      }

      switch (horizontalContentAlignment) {
         case HorizontalAlignment.Center:
            originX += (availableSize.Width - sizeForChild.Width) / 2;
            break;
         case HorizontalAlignment.Right:
            originX += availableSize.Width - sizeForChild.Width;
            break;
      }

      switch (verticalContentAlignment) {
         case VerticalAlignment.Center:
            originY += (availableSize.Height - sizeForChild.Height) / 2;
            break;
         case VerticalAlignment.Bottom:
            originY += availableSize.Height - sizeForChild.Height;
            break;
      }

      if (useLayoutRounding) {
         originX = LayoutHelper.RoundLayoutValue(originX, scale);
         originY = LayoutHelper.RoundLayoutValue(originY, scale);
      }

      var boundsForChild = new Rect(originX, originY, sizeForChild.Width, sizeForChild.Height).Deflate(padding);
      Child.Arrange(boundsForChild);

      return finalSize;
   }

   private void ContentChanged(AvaloniaPropertyChangedEventArgs e)
   {
      _createdChild = false;

      if (((ILogical)this).IsAttachedToLogicalTree) {
         if (e.Property.Name == nameof(Content)) {
            UpdateChild(e.NewValue);
         } else {
            UpdateChild();
         }
      } else if (Child != null) {
         VisualChildren.Remove(Child);
         LogicalChildren.Remove(Child);
         ((ISetInheritanceParent)Child).SetParent(Child.Parent);
         Child = null;
         _recyclingDataTemplate = null;
      }

      UpdatePseudoClasses();
      InvalidateMeasure();
   }

   private void UpdatePseudoClasses()
   {
      PseudoClasses.Set(":empty", Content is null);
   }
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      switch (change.Property.Name)
      {
         case nameof(Content):
         case nameof(ContentTemplate):
            ContentChanged(change);
            break;
      }
   }
}
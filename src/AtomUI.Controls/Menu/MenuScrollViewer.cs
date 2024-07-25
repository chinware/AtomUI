using AtomUI.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;

namespace AtomUI.Controls;

using AvaloniaScrollViewer = Avalonia.Controls.ScrollViewer;

[TemplatePart(MenuScrollViewerTheme.ScrollDownButtonPart, typeof(IconButton))]
[TemplatePart(MenuScrollViewerTheme.ScrollUpButtonPart, typeof(IconButton))]
[TemplatePart(MenuScrollViewerTheme.ScrollViewContentPart, typeof(ScrollContentPresenter))]
public class MenuScrollViewer : AvaloniaScrollViewer, IControlCustomStyle
{
   private readonly IControlCustomStyle _customStyle;
   private IconButton? _scrollUpButton;
   private IconButton? _scrollDownButton;
   private ScrollContentPresenter? _scrollViewContent;
   
   public MenuScrollViewer()
   {
      _customStyle = this;
   }
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _customStyle.HandleTemplateApplied(e.NameScope);
   }
   
   #region IControlCustomStyle 实现
   
   void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
   {
      _scrollUpButton = scope.Find<IconButton>(MenuScrollViewerTheme.ScrollUpButtonPart);
      _scrollDownButton = scope.Find<IconButton>(MenuScrollViewerTheme.ScrollDownButtonPart);
      _scrollViewContent = scope.Find<ScrollContentPresenter>(MenuScrollViewerTheme.ScrollViewContentPart);
   }
   
   #endregion
}
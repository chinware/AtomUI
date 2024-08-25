using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Demo.Desktop.ShowCase;

public class ShowCaseItem : ContentControl
{
   private bool _initialized = false;
   public string Title { get; set; } = string.Empty;
   public string Description { get; set; } = string.Empty;

   private void SetupUi()
   {
      var mainLayout = new StackPanel();
      var showCaseTitle = new StackPanel
      {
         Orientation = Orientation.Horizontal,
         Margin = new Thickness(0, 30, 0, 0)
      };
      showCaseTitle.Children.Add(new Label()
      {
         VerticalAlignment = VerticalAlignment.Center,
         HorizontalAlignment = HorizontalAlignment.Left,
         Content = Title,
         FontWeight = FontWeight.Bold
      });
      showCaseTitle.Children.Add(new Separator()
      {
         VerticalAlignment = VerticalAlignment.Center,
         HorizontalAlignment = HorizontalAlignment.Stretch,
         Margin = new Thickness(10, 0),
         Width = 200,
         BorderBrush = new SolidColorBrush(Colors.Gray),
      });

      if (Content is Control contentControl) {
         LogicalChildren.Remove(contentControl);
         mainLayout.Children.Add(contentControl);
      }

      mainLayout.Children.Add(showCaseTitle);
      mainLayout.Children.Add(new TextBlock()
      {
         Text = Description,
         TextWrapping = TextWrapping.Wrap
      });
      
      var outerBorder = new Border()
      {
         BorderBrush = new SolidColorBrush(new Color(10, 5, 5, 5)),
         BorderThickness = new Thickness(1),
         Padding = new Thickness(20),
         Child = mainLayout
      };
      
      Content = outerBorder;
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      if (!_initialized) {
         SetupUi();
         _initialized = true;
      }
   }
}
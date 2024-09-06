using AtomUI.Theme;
using AtomUI.Theme.Styling;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Styling;
using AvaloniaButton = Avalonia.Controls.Button;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CalendarItemTheme : BaseControlTheme
{
   public const string ItemFramePart = "PART_ItemFrame";
   public const string ItemRootLayoutPart = "PART_ItemRootLayout";
   public const string MonthViewPart = "PART_MonthView";
   public const string YearViewPart = "PART_YearView";
   
   public const string PreviousButtonPart = "PART_PreviousButton";
   public const string HeaderButtonPart = "PART_HeaderButton";
   public const string NextButtonPart = "PART_NextButton";
   
   public CalendarItemTheme()
      : base(typeof(CalendarItem))
   {
   }
   
   protected override IControlTemplate BuildControlTemplate()
   {
      return new FuncControlTemplate<CalendarItem>((calendarItem, scope) =>
      {
         var frame = new Border()
         {
            Name = ItemFramePart
         };
         
         CreateTemplateParentBinding(frame, Border.BorderBrushProperty, CalendarItem.BorderBrushProperty);
         CreateTemplateParentBinding(frame, Border.BorderThicknessProperty, CalendarItem.BorderThicknessProperty);
         CreateTemplateParentBinding(frame, Border.CornerRadiusProperty, CalendarItem.CornerRadiusProperty);
         CreateTemplateParentBinding(frame, Border.BackgroundProperty, CalendarItem.BackgroundProperty);

         var rootLayout = new DockPanel()
         {
            Name = ItemRootLayoutPart,
            LastChildFill = true
         };
         BuildHeader(rootLayout, scope);
         BuildContentView(rootLayout, scope);

         BuildDayTitleTemplate(calendarItem);
         frame.Child = rootLayout;
         
         return frame;
      });
   }

   private void BuildDayTitleTemplate(CalendarItem calendarItem)
   {
      calendarItem.DayTitleTemplate = new DayTitleTemplate();
   }

   private void BuildHeader(DockPanel layout, INameScope scope)
   {
      var headerLayout = new Grid()
      {
         ColumnDefinitions = new ColumnDefinitions()
         {
            new ColumnDefinition(GridLength.Auto),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Auto),
         }
      };
      var previousButton = new AvaloniaButton()
      {
         Name = PreviousButtonPart
      };
      previousButton.RegisterInNameScope(scope);
      Grid.SetColumn(previousButton, 0);
      headerLayout.Children.Add(previousButton);
      
      var headerButton = new AvaloniaButton()
      {
         Name = HeaderButtonPart
      };
      Grid.SetColumn(headerButton, 1);
      headerButton.RegisterInNameScope(scope);
      headerLayout.Children.Add(headerButton);
      
      var nextButton = new AvaloniaButton()
      {
         Name = NextButtonPart
      };
      Grid.SetColumn(nextButton, 2);
      nextButton.RegisterInNameScope(scope);
      headerLayout.Children.Add(nextButton);
      DockPanel.SetDock(headerLayout, Dock.Top);
      layout.Children.Add(headerLayout);
   }

   private void BuildContentView(DockPanel layout, INameScope scope)
   {
      var monthView = new Grid()
      {
         Name = MonthViewPart,
         HorizontalAlignment = HorizontalAlignment.Stretch,
         IsVisible = false,
         RowDefinitions = new RowDefinitions()
         {
            new RowDefinition(30, GridUnitType.Pixel),
            new RowDefinition(GridLength.Star),
            new RowDefinition(GridLength.Star),
            new RowDefinition(GridLength.Star),
            new RowDefinition(GridLength.Star),
            new RowDefinition(GridLength.Star),
            new RowDefinition(GridLength.Star),
         },
         ColumnDefinitions = new ColumnDefinitions()
         {
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
         }
      };
      monthView.RegisterInNameScope(scope);
      layout.Children.Add(monthView);

      var yearView = new Grid()
      {
         Name = YearViewPart,
         IsVisible = false,
         RowDefinitions = new RowDefinitions()
         {
            new RowDefinition(GridLength.Star),
            new RowDefinition(GridLength.Star),
            new RowDefinition(GridLength.Star),
         },
         ColumnDefinitions = new ColumnDefinitions()
         {
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Star),
         }
      };
      yearView.RegisterInNameScope(scope);
      layout.Children.Add(yearView);
   }

   protected override void BuildStyles()
   {
      var commonStyle = new Style(selector => selector.Nesting());
      commonStyle.Add(CalendarItem.BorderBrushProperty, GlobalTokenResourceKey.ColorBorder);
      commonStyle.Add(CalendarItem.CornerRadiusProperty, GlobalTokenResourceKey.BorderRadius);
      commonStyle.Add(CalendarItem.BackgroundProperty, GlobalTokenResourceKey.ColorBgContainer);
      Add(commonStyle);
   }
}

internal class DayTitleTemplate : ITemplate<Control>
{
   public Control Build()
   {
      return new TextBlock();
   }

   object? ITemplate.Build()
   {
      return Build();
   }
}
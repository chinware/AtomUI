using AtomUI.Theme;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Styling;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class CalendarItemTheme : BaseControlTheme
{
   public const string ItemFramePart = "PART_ItemFrame";
   public const string ItemRootLayoutPart = "PART_ItemRootLayout";
   public const string MonthViewPart = "PART_MonthView";
   public const string YearViewPart = "PART_YearView";
   public const string HeaderLayoutPart = "PART_HeaderLayout";
   
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
         Name = HeaderLayoutPart,
         ColumnDefinitions = new ColumnDefinitions()
         {
            new ColumnDefinition(GridLength.Auto),
            new ColumnDefinition(GridLength.Star),
            new ColumnDefinition(GridLength.Auto),
         }
      };
      
      var previousButtonIcon = new PathIcon()
      {
         Kind = "LeftOutlined"
      };
      TokenResourceBinder.CreateGlobalTokenBinding(previousButtonIcon, PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorTextDescription);
      TokenResourceBinder.CreateGlobalTokenBinding(previousButtonIcon, PathIcon.ActiveFilledBrushProperty, GlobalTokenResourceKey.ColorText);
      TokenResourceBinder.CreateGlobalTokenBinding(previousButtonIcon, PathIcon.SelectedFilledBrushProperty, GlobalTokenResourceKey.ColorText);
      
      var previousButton = new IconButton()
      {
         Name = PreviousButtonPart,
         Icon = previousButtonIcon
      };

      TokenResourceBinder.CreateGlobalTokenBinding(previousButton, IconButton.IconWidthProperty, GlobalTokenResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(previousButton, IconButton.IconHeightProperty, GlobalTokenResourceKey.IconSize);
      
      previousButton.RegisterInNameScope(scope);
      Grid.SetColumn(previousButton, 0);
      headerLayout.Children.Add(previousButton);
      
      var headerButton = new HeadTextButton()
      {
         Name = HeaderButtonPart
      };
      Grid.SetColumn(headerButton, 1);
      headerButton.RegisterInNameScope(scope);
      headerLayout.Children.Add(headerButton);
      
      var nextButtonIcon = new PathIcon()
      {
         Kind = "RightOutlined"
      };
      TokenResourceBinder.CreateGlobalTokenBinding(nextButtonIcon, PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorTextDescription);
      TokenResourceBinder.CreateGlobalTokenBinding(nextButtonIcon, PathIcon.ActiveFilledBrushProperty, GlobalTokenResourceKey.ColorText);
      TokenResourceBinder.CreateGlobalTokenBinding(nextButtonIcon, PathIcon.SelectedFilledBrushProperty, GlobalTokenResourceKey.ColorText);
      
      var nextButton = new IconButton()
      {
         Name = NextButtonPart,
         Icon = nextButtonIcon
      };
      TokenResourceBinder.CreateGlobalTokenBinding(nextButton, IconButton.IconWidthProperty, GlobalTokenResourceKey.IconSize);
      TokenResourceBinder.CreateGlobalTokenBinding(nextButton, IconButton.IconHeightProperty, GlobalTokenResourceKey.IconSize);
      
      Grid.SetColumn(nextButton, 2);
      nextButton.RegisterInNameScope(scope);
      headerLayout.Children.Add(nextButton);
      DockPanel.SetDock(headerLayout, Dock.Top);
      layout.Children.Add(headerLayout);
   }

   private void BuildContentView(DockPanel layout, INameScope scope)
   {
      var dayTitleRowDef = new RowDefinition();
      TokenResourceBinder.CreateTokenBinding(dayTitleRowDef, RowDefinition.HeightProperty, CalendarTokenResourceKey.DayTitleHeight);
      
      var monthView = new Grid()
      {
         Name = MonthViewPart,
         HorizontalAlignment = HorizontalAlignment.Stretch,
         IsVisible = false,
         RowDefinitions = new RowDefinitions()
         {
            dayTitleRowDef,
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

      commonStyle.Add(CalendarItem.MinWidthProperty, CalendarTokenResourceKey.ItemPanelMinWidth);
      commonStyle.Add(CalendarItem.MinHeightProperty, CalendarTokenResourceKey.ItemPanelMinHeight);

      var headerLayoutStyle = new Style(selector => selector.Nesting().Template().Name(HeaderLayoutPart));
      headerLayoutStyle.Add(Grid.MarginProperty, CalendarTokenResourceKey.HeaderMargin);
      
      commonStyle.Add(headerLayoutStyle);
      
      Add(commonStyle);
   }
}

internal class DayTitleTemplate : ITemplate<Control>
{
   public Control Build()
   {
      var textBlock = new TextBlock()
      {
         HorizontalAlignment = HorizontalAlignment.Center,
         VerticalAlignment = VerticalAlignment.Center
      };
      textBlock.Bind(TextBlock.TextProperty, new Binding());
      return textBlock;
   }

   object? ITemplate.Build()
   {
      return Build();
   }
}
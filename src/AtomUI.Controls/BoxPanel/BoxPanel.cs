using System;
  using System.Collections.Generic;
  using System.Linq;
  using Avalonia;
  using Avalonia.Controls;
  using Avalonia.Layout;

  namespace AtomUI.Controls;

  /// <summary>
  /// A flexible box layout panel that implements CSS Flexbox-like behavior
  /// </summary>
  public class BoxPanel : Panel
  {
      #region Properties

      public static readonly StyledProperty<Orientation> OrientationProperty =
          AvaloniaProperty.Register<BoxPanel, Orientation>(nameof(Orientation), Orientation.Vertical);

      public static readonly StyledProperty<double> SpacingProperty =
          AvaloniaProperty.Register<BoxPanel, double>(nameof(Spacing), 0.0);

      public static readonly StyledProperty<JustifyContent> JustifyContentProperty =
          AvaloniaProperty.Register<BoxPanel, JustifyContent>(nameof(JustifyContent),
              JustifyContent.FlexStart);

      public static readonly StyledProperty<AlignItems> AlignItemsProperty =
          AvaloniaProperty.Register<BoxPanel, AlignItems>(nameof(AlignItems), AlignItems.Stretch);

      public static readonly StyledProperty<AlignContent> AlignContentProperty =
          AvaloniaProperty.Register<BoxPanel, AlignContent>(nameof(AlignContent), AlignContent.FlexStart);

      public static readonly StyledProperty<FlexWrap> WrapProperty =
          AvaloniaProperty.Register<BoxPanel, FlexWrap>(nameof(Wrap), FlexWrap.NoWrap);

      public static readonly StyledProperty<double> ColumnSpacingProperty =
          AvaloniaProperty.Register<BoxPanel, double>(nameof(ColumnSpacing), double.NaN);

      public static readonly StyledProperty<double> RowSpacingProperty =
          AvaloniaProperty.Register<BoxPanel, double>(nameof(RowSpacing), double.NaN);

      public static readonly AttachedProperty<int> FlexProperty =
          AvaloniaProperty.RegisterAttached<BoxPanel, Control, int>("Flex", 0);

      public static readonly AttachedProperty<int> OrderProperty =
          AvaloniaProperty.RegisterAttached<BoxPanel, Control, int>("Order", 0);

      public static readonly AttachedProperty<AlignItems?> AlignSelfProperty =
          AvaloniaProperty.RegisterAttached<BoxPanel, Control, AlignItems?>("AlignSelf", null);

      public Orientation Orientation
      {
          get => GetValue(OrientationProperty);
          set => SetValue(OrientationProperty, value);
      }

      public double Spacing
      {
          get => GetValue(SpacingProperty);
          set => SetValue(SpacingProperty, value);
      }

      public JustifyContent JustifyContent
      {
          get => GetValue(JustifyContentProperty);
          set => SetValue(JustifyContentProperty, value);
      }

      public AlignItems AlignItems
      {
          get => GetValue(AlignItemsProperty);
          set => SetValue(AlignItemsProperty, value);
      }

      public AlignContent AlignContent
      {
          get => GetValue(AlignContentProperty);
          set => SetValue(AlignContentProperty, value);
      }

      public FlexWrap Wrap
      {
          get => GetValue(WrapProperty);
          set => SetValue(WrapProperty, value);
      }

      public double ColumnSpacing
      {
          get => GetValue(ColumnSpacingProperty);
          set => SetValue(ColumnSpacingProperty, value);
      }

      public double RowSpacing
      {
          get => GetValue(RowSpacingProperty);
          set => SetValue(RowSpacingProperty, value);
      }

      public static void SetFlex(Control element, int value) => element.SetValue(FlexProperty, value);
      public static int GetFlex(Control element) => element.GetValue(FlexProperty);

      public static void SetOrder(Control element, int value) => element.SetValue(OrderProperty, value);
      public static int GetOrder(Control element) => element.GetValue(OrderProperty);

      public static void SetAlignSelf(Control element, AlignItems? value) =>
          element.SetValue(AlignSelfProperty, value);

      public static AlignItems? GetAlignSelf(Control element) => element.GetValue(AlignSelfProperty);

      #endregion

      static BoxPanel()
      {
          AffectsMeasure<BoxPanel>(OrientationProperty, SpacingProperty, JustifyContentProperty,
              WrapProperty, ColumnSpacingProperty, RowSpacingProperty);
          AffectsArrange<BoxPanel>(AlignItemsProperty, AlignContentProperty);
          AffectsParentMeasure<BoxPanel>(OrderProperty, FlexProperty);
          AffectsParentArrange<BoxPanel>(AlignSelfProperty);
      }

      private List<Section> _sections = new();
      private Control[] _visibleChildren = Array.Empty<Control>();

      #region Helper Methods

      /// <summary>
      /// Adds a fixed spacing element to the panel
      /// </summary>
      public void AddSpacing(double size)
      {
          var spacer = new Border
          {
              Width  = Orientation == Orientation.Horizontal ? size : 0,
              Height = Orientation == Orientation.Vertical ? size : 0
          };
          Children.Add(spacer);
      }

      /// <summary>
      /// Adds a flexible spacing element that will grow to fill available space
      /// </summary>
      public void AddFlex(int flex)
      {
          var spacer = new Border();
          SetFlex(spacer, flex);
          Children.Add(spacer);
      }

      private double GetEffectiveColumnSpacing()
      {
          return double.IsNaN(ColumnSpacing) ? Spacing : ColumnSpacing;
      }

      private double GetEffectiveRowSpacing()
      {
          return double.IsNaN(RowSpacing) ? Spacing : RowSpacing;
      }

      #endregion

      protected override Size MeasureOverride(Size availableSize)
      {
          var isColumn = Orientation == Orientation.Vertical;
          var even     = JustifyContent == JustifyContent.SpaceEvenly ? 2 : 0;

          var maxAxisCoord     = AxisCoordinate.FromSize(availableSize, isColumn);
          var spacingAxisCoord = AxisCoordinate.FromSize(GetEffectiveColumnSpacing(), GetEffectiveRowSpacing(), isColumn);

          var mainAxisPosition = 0.0;
          var itemCount = 0;

          var crossAxisPosition    = 0.0;
          var maxCrossAxis = 0.0;
          var sectionCount    = 0;

          _sections = new List<Section>();
          var first = 0;
          var i     = 0;

          // Get visible children sorted by Order
          _visibleChildren = Children
                             .Where(c => c.IsVisible)
                             .OrderBy(GetOrder)
                             .ToArray();

          foreach (var element in _visibleChildren)
          {
              element.Measure(availableSize);

              var elementAxisCoord = AxisCoordinate.FromSize(element.DesiredSize, isColumn);

              // Check if we need to wrap
              if (Wrap != FlexWrap.NoWrap && itemCount > 0 && mainAxisPosition + elementAxisCoord.MainAxis + (itemCount + even) * spacingAxisCoord.MainAxis > maxAxisCoord.MainAxis)
              {
                  _sections.Add(new Section(first, i - 1, mainAxisPosition, maxCrossAxis));

                  mainAxisPosition = 0.0;
                  itemCount = 0;

                  crossAxisPosition    += maxCrossAxis;
                  maxCrossAxis =  0.0;
                  sectionCount++;

                  first = i;
              }

              if (elementAxisCoord.CrossAxis > maxCrossAxis)
              {
                  maxCrossAxis = elementAxisCoord.CrossAxis;
              }

              mainAxisPosition += elementAxisCoord.MainAxis;
              itemCount++;
              i++;
          }

          // Add final section
          if (itemCount != 0)
          {
              _sections.Add(new Section(first, first + itemCount - 1, mainAxisPosition, maxCrossAxis));
          }

          // Handle WrapReverse
          if (Wrap == FlexWrap.WrapReverse)
          {
              _sections.Reverse();
          }

          if (_sections.Count == 0)
          {
              return new Size(0, 0);
          }

          // Calculate final size
          var maxMainAxis   = _sections.Max(s => s.U + (s.Last - s.First + even) * spacingAxisCoord.MainAxis);
          var totalCrossAxis = crossAxisPosition + maxCrossAxis + (_sections.Count - 1) * spacingAxisCoord.CrossAxis;

          return AxisCoordinate.ToSize(new AxisCoordinate(
              double.IsInfinity(maxAxisCoord.MainAxis) ? maxMainAxis : maxAxisCoord.MainAxis,
              double.IsInfinity(maxAxisCoord.CrossAxis) ? totalCrossAxis : maxAxisCoord.CrossAxis
          ), isColumn);
      }

      protected override Size ArrangeOverride(Size finalSize)
      {
          if (_visibleChildren.Length == 0 || _sections.Count == 0)
          {
              return finalSize;
          }

          var isColumn  = Orientation == Orientation.Vertical;
          var isReverse = false;

          var sectionCount       = _sections.Count;
          var containerAxisCoord    = AxisCoordinate.FromSize(finalSize, isColumn);
          var spacingAxisCoord = AxisCoordinate.FromSize(GetEffectiveColumnSpacing(), GetEffectiveRowSpacing(), isColumn);

          // Calculate total section cross-axis and spacing cross-axis
          double totalSectionCrossAxis = 0.0;
          foreach (var section in _sections)
          {
              totalSectionCrossAxis += section.V;
          }

          double totalSpacingCrossAxis = (sectionCount - 1) * spacingAxisCoord.CrossAxis;
          double totalCrossAxis        = totalSectionCrossAxis + totalSpacingCrossAxis;

          // Calculate cross-axis spacing (AlignContent)
          var crossAxisSpacing = AlignContent switch
          {
              AlignContent.FlexStart => spacingAxisCoord.CrossAxis,
              AlignContent.FlexEnd => spacingAxisCoord.CrossAxis,
              AlignContent.Center => spacingAxisCoord.CrossAxis,
              AlignContent.Stretch => spacingAxisCoord.CrossAxis,
              AlignContent.SpaceBetween => sectionCount > 1 ? spacingAxisCoord.CrossAxis + (containerAxisCoord.CrossAxis - totalCrossAxis) / (sectionCount - 1) : spacingAxisCoord.CrossAxis,
              AlignContent.SpaceAround => (containerAxisCoord.CrossAxis - totalSectionCrossAxis) / sectionCount,
              AlignContent.SpaceEvenly => (containerAxisCoord.CrossAxis - totalSectionCrossAxis) / (sectionCount + 1),
              _ => spacingAxisCoord.CrossAxis
          };

          var crossAxisScale = AlignContent == AlignContent.Stretch && totalSectionCrossAxis > 0
              ? ((containerAxisCoord.CrossAxis - totalSpacingCrossAxis) / totalSectionCrossAxis)
              : 1.0;

          var crossAxisPosition = AlignContent switch
          {
              AlignContent.FlexStart => 0.0,
              AlignContent.FlexEnd => containerAxisCoord.CrossAxis - totalCrossAxis,
              AlignContent.Center => (containerAxisCoord.CrossAxis - totalCrossAxis) / 2,
              AlignContent.Stretch => 0.0,
              AlignContent.SpaceBetween => 0.0,
              AlignContent.SpaceAround => crossAxisSpacing / 2,
              AlignContent.SpaceEvenly => crossAxisSpacing,
              _ => 0.0
          };

          foreach (var section in _sections)
          {
              // Calculate section's cross-axis size
              double sectionCrossAxis;
              if (sectionCount == 1 && AlignItems == AlignItems.Stretch)
              {
                  sectionCrossAxis = containerAxisCoord.CrossAxis; // Single section with stretch: use full container size
              }
              else
              {
                  sectionCrossAxis = crossAxisScale * section.V;
              }

              // Calculate gap count (number of gaps = items - 1)
              var gapCount = section.Last - section.First;

              // First pass: calculate total flex and check if any flex items exist
              double totalFlex    = 0;
              var    hasFlexItems = false;

              for (int i = section.First; i <= section.Last; i++)
              {
                  int flex = GetFlex(_visibleChildren[i]);
                  if (flex > 0)
                  {
                      totalFlex    += flex;
                      hasFlexItems =  true;
                  }
              }

              // Calculate spacing and offset based on JustifyContent
              double mainAxisSpacing;
              double mainAxisOffset;

              if (hasFlexItems)
              {
                  // With flex items: use base spacing, start at 0 (flex items consume remaining space)
                  mainAxisSpacing = spacingAxisCoord.MainAxis;
                  mainAxisOffset  = 0.0;
              }
              else
              {
                  // No flex items: apply JustifyContent normally
                  (mainAxisSpacing, mainAxisOffset) = JustifyContent switch
                  {
                      JustifyContent.FlexStart => (spacingAxisCoord.MainAxis, 0.0),
                      JustifyContent.FlexEnd => (spacingAxisCoord.MainAxis, containerAxisCoord.MainAxis - section.U - gapCount * spacingAxisCoord.MainAxis),
                      JustifyContent.Center => (spacingAxisCoord.MainAxis, (containerAxisCoord.MainAxis - section.U - gapCount * spacingAxisCoord.MainAxis) / 2),
                      JustifyContent.SpaceBetween => gapCount > 0
                          ? ((containerAxisCoord.MainAxis - section.U) / gapCount, 0.0)
                          : (spacingAxisCoord.MainAxis, 0.0),
                      JustifyContent.SpaceAround => (spacingAxisCoord.MainAxis, (containerAxisCoord.MainAxis - section.U - gapCount * spacingAxisCoord.MainAxis) /
                                                                2),
                      JustifyContent.SpaceEvenly => ((containerAxisCoord.MainAxis - section.U) / (gapCount + 2), (containerAxisCoord.MainAxis -
                          section.U) / (gapCount + 2)),
                      _ => (spacingAxisCoord.MainAxis, 0.0)
                  };
              }

              // Calculate available space for flex items
              double totalMainAxisSpacing = gapCount * mainAxisSpacing;
              double fixedSize     = 0;

              // Calculate total size of non-flex items
              for (int i = section.First; i <= section.Last; i++)
              {
                  int flex = GetFlex(_visibleChildren[i]);
                  if (flex == 0)
                  {
                      var elementAxisCoord = AxisCoordinate.FromSize(_visibleChildren[i].DesiredSize, isColumn);
                      fixedSize += elementAxisCoord.MainAxis;
                  }
              }

              double flexSpace = totalFlex > 0 ? Math.Max(0, containerAxisCoord.MainAxis - fixedSize - totalMainAxisSpacing) : 0;

              var mainAxisPosition = mainAxisOffset;

              for (int i = section.First; i <= section.Last; i++)
              {
                  var element     = _visibleChildren[i];
                  var elementAxisCoord = AxisCoordinate.FromSize(element.DesiredSize, isColumn);

                  // Calculate main-axis size
                  int    flex = GetFlex(element);
                  double finalMainAxis;

                  if (flex > 0 && totalFlex > 0)
                  {
                      // Flex item: allocate proportional space
                      finalMainAxis = flexSpace * (flex / totalFlex);
                  }
                  else
                  {
                      // Fixed item: use desired size
                      finalMainAxis = elementAxisCoord.MainAxis;
                  }

                  // Calculate cross-axis alignment
                  var align = GetAlignSelf(element) ?? AlignItems;

                  double finalCrossAxis = align switch
                  {
                      AlignItems.FlexStart => crossAxisPosition,
                      AlignItems.FlexEnd => crossAxisPosition + sectionCrossAxis - elementAxisCoord.CrossAxis,
                      AlignItems.Center => crossAxisPosition + (sectionCrossAxis - elementAxisCoord.CrossAxis) / 2,
                      AlignItems.Stretch => crossAxisPosition,
                      _ => crossAxisPosition
                  };

                  // 🔑 CSS-compliant Stretch logic: only stretch if element doesn't have explicit cross-axis size
                  double actualCrossAxis;
                  if (align == AlignItems.Stretch)
                  {
                      // Check if element has explicit cross-axis size
                      bool hasExplicitSize = isColumn
                          ? (!double.IsNaN(element.Width) && element.Width > 0)
                          : (!double.IsNaN(element.Height) && element.Height > 0);

                      if (hasExplicitSize)
                      {
                          // Element has explicit size, don't stretch
                          actualCrossAxis = elementAxisCoord.CrossAxis;
                      }
                      else
                      {
                          // Element has auto size, stretch it
                          actualCrossAxis = sectionCrossAxis;
                      }
                  }
                  else
                  {
                      actualCrossAxis = elementAxisCoord.CrossAxis;
                  }

                  var positionAxisCoord    = new AxisCoordinate(isReverse ? (containerAxisCoord.MainAxis - finalMainAxis - mainAxisPosition) : mainAxisPosition, finalCrossAxis);
                  var arrangeSizeAxisCoord = new AxisCoordinate(finalMainAxis, actualCrossAxis);

                  element.Arrange(new Rect(
                      AxisCoordinate.ToPoint(positionAxisCoord, isColumn),
                      AxisCoordinate.ToSize(arrangeSizeAxisCoord, isColumn)
                  ));

                  mainAxisPosition += finalMainAxis + mainAxisSpacing;
              }

              crossAxisPosition += sectionCrossAxis + crossAxisSpacing;
          }

          return finalSize;
      }
  }
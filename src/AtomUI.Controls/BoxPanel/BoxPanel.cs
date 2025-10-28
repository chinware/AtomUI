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

          var max     = Uv.FromSize(availableSize, isColumn);
          var spacing = Uv.FromSize(GetEffectiveColumnSpacing(), GetEffectiveRowSpacing(), isColumn);

          var u = 0.0;
          var m = 0;

          var v    = 0.0;
          var maxV = 0.0;
          var n    = 0;

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

              var size = Uv.FromSize(element.DesiredSize, isColumn);

              // Check if we need to wrap
              if (Wrap != FlexWrap.NoWrap && m > 0 && u + size.U + (m + even) * spacing.U > max.U)
              {
                  _sections.Add(new Section(first, i - 1, u, maxV));

                  u = 0.0;
                  m = 0;

                  v    += maxV;
                  maxV =  0.0;
                  n++;

                  first = i;
              }

              if (size.V > maxV)
              {
                  maxV = size.V;
              }

              u += size.U;
              m++;
              i++;
          }

          // Add final section
          if (m != 0)
          {
              _sections.Add(new Section(first, first + m - 1, u, maxV));
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
          var maxU   = _sections.Max(s => s.U + (s.Last - s.First + even) * spacing.U);
          var totalV = v + maxV + (_sections.Count - 1) * spacing.V;

          return Uv.ToSize(new Uv(
              double.IsInfinity(max.U) ? maxU : max.U,
              double.IsInfinity(max.V) ? totalV : max.V
          ), isColumn);
      }

      protected override Size ArrangeOverride(Size finalSize)
      {
          if (_visibleChildren.Length == 0 || _sections.Count == 0)
              return finalSize;

          var isColumn  = Orientation == Orientation.Vertical;
          var isReverse = false;

          var n       = _sections.Count;
          var size    = Uv.FromSize(finalSize, isColumn);
          var spacing = Uv.FromSize(GetEffectiveColumnSpacing(), GetEffectiveRowSpacing(), isColumn);

          // Calculate total section V and spacing V
          double totalSectionV = 0.0;
          foreach (var section in _sections)
          {
              totalSectionV += section.V;
          }

          double totalSpacingV = (n - 1) * spacing.V;
          double totalV        = totalSectionV + totalSpacingV;

          // Calculate cross-axis spacing (AlignContent)
          var spacingV = AlignContent switch
          {
              AlignContent.FlexStart => spacing.V,
              AlignContent.FlexEnd => spacing.V,
              AlignContent.Center => spacing.V,
              AlignContent.Stretch => spacing.V,
              AlignContent.SpaceBetween => n > 1 ? spacing.V + (size.V - totalV) / (n - 1) : spacing.V,
              AlignContent.SpaceAround => (size.V - totalSectionV) / n,
              AlignContent.SpaceEvenly => (size.V - totalSectionV) / (n + 1),
              _ => spacing.V
          };

          var scaleV = AlignContent == AlignContent.Stretch && totalSectionV > 0
              ? ((size.V - totalSpacingV) / totalSectionV)
              : 1.0;

          var v = AlignContent switch
          {
              AlignContent.FlexStart => 0.0,
              AlignContent.FlexEnd => size.V - totalV,
              AlignContent.Center => (size.V - totalV) / 2,
              AlignContent.Stretch => 0.0,
              AlignContent.SpaceBetween => 0.0,
              AlignContent.SpaceAround => spacingV / 2,
              AlignContent.SpaceEvenly => spacingV,
              _ => 0.0
          };

          foreach (var section in _sections)
          {
              // Calculate section's cross-axis size
              double sectionV;
              if (n == 1 && AlignItems == AlignItems.Stretch)
              {
                  sectionV = size.V; // Single section with stretch: use full container size
              }
              else
              {
                  sectionV = scaleV * section.V;
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
              double spacingU;
              double offsetU;

              if (hasFlexItems)
              {
                  // With flex items: use base spacing, start at 0 (flex items consume remaining space)
                  spacingU = spacing.U;
                  offsetU  = 0.0;
              }
              else
              {
                  // No flex items: apply JustifyContent normally
                  (spacingU, offsetU) = JustifyContent switch
                  {
                      JustifyContent.FlexStart => (spacing.U, 0.0),
                      JustifyContent.FlexEnd => (spacing.U, size.U - section.U - gapCount * spacing.U),
                      JustifyContent.Center => (spacing.U, (size.U - section.U - gapCount * spacing.U) / 2),
                      JustifyContent.SpaceBetween => gapCount > 0
                          ? ((size.U - section.U) / gapCount, 0.0)
                          : (spacing.U, 0.0),
                      JustifyContent.SpaceAround => (spacing.U, (size.U - section.U - gapCount * spacing.U) /
                                                                2),
                      JustifyContent.SpaceEvenly => ((size.U - section.U) / (gapCount + 2), (size.U -
                          section.U) / (gapCount + 2)),
                      _ => (spacing.U, 0.0)
                  };
              }

              // Calculate available space for flex items
              double totalSpacingU = gapCount * spacingU;
              double fixedSize     = 0;

              // Calculate total size of non-flex items
              for (int i = section.First; i <= section.Last; i++)
              {
                  int flex = GetFlex(_visibleChildren[i]);
                  if (flex == 0)
                  {
                      var elementSize = Uv.FromSize(_visibleChildren[i].DesiredSize, isColumn);
                      fixedSize += elementSize.U;
                  }
              }

              double flexSpace = totalFlex > 0 ? Math.Max(0, size.U - fixedSize - totalSpacingU) : 0;

              var u = offsetU;

              for (int i = section.First; i <= section.Last; i++)
              {
                  var element     = _visibleChildren[i];
                  var elementSize = Uv.FromSize(element.DesiredSize, isColumn);

                  // Calculate main-axis size
                  int    flex = GetFlex(element);
                  double finalU;

                  if (flex > 0 && totalFlex > 0)
                  {
                      // Flex item: allocate proportional space
                      finalU = flexSpace * (flex / totalFlex);
                  }
                  else
                  {
                      // Fixed item: use desired size
                      finalU = elementSize.U;
                  }

                  // Calculate cross-axis alignment
                  var align = GetAlignSelf(element) ?? AlignItems;

                  double finalV = align switch
                  {
                      AlignItems.FlexStart => v,
                      AlignItems.FlexEnd => v + sectionV - elementSize.V,
                      AlignItems.Center => v + (sectionV - elementSize.V) / 2,
                      AlignItems.Stretch => v,
                      _ => v
                  };

                  // 🔑 CSS-compliant Stretch logic: only stretch if element doesn't have explicit cross-axis size
                  double actualV;
                  if (align == AlignItems.Stretch)
                  {
                      // Check if element has explicit cross-axis size
                      bool hasExplicitSize = isColumn
                          ? (!double.IsNaN(element.Width) && element.Width > 0)
                          : (!double.IsNaN(element.Height) && element.Height > 0);

                      if (hasExplicitSize)
                      {
                          // Element has explicit size, don't stretch
                          actualV = elementSize.V;
                      }
                      else
                      {
                          // Element has auto size, stretch it
                          actualV = sectionV;
                      }
                  }
                  else
                  {
                      actualV = elementSize.V;
                  }

                  var position    = new Uv(isReverse ? (size.U - finalU - u) : u, finalV);
                  var arrangeSize = new Uv(finalU, actualV);

                  element.Arrange(new Rect(
                      Uv.ToPoint(position, isColumn),
                      Uv.ToSize(arrangeSize, isColumn)
                  ));

                  u += finalU + spacingU;
              }

              v += sectionV + spacingV;
          }

          return finalSize;
      }
  }
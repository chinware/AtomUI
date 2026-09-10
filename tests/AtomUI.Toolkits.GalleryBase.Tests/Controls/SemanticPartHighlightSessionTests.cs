using AtomUI.Theme;
using AtomUI.Theme.Schema;
using AtomUI.Toolkits.GalleryBase.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUIButton = AtomUI.Desktop.Controls.Button;
using AtomUIWindow = AtomUI.Desktop.Controls.Window;
using AtomUIMasonry = AtomUI.Desktop.Controls.Masonry;

namespace AtomUI.Toolkits.GalleryBase.Tests.Controls;

public class SemanticPartHighlightSessionTests
{
    public SemanticPartHighlightSessionTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Adorner_Draws_An_Outline_Without_Covering_The_Target()
    {
        var primary = RenderAdorner(true);
        primary.Length.ShouldBe(2);
        primary.ShouldAllBe(static drawing => drawing.Brush == null);
        AssertPen(primary[0], Colors.White, 1);
        AssertPen(primary[1], Color.FromArgb(0xFF, 0xFA, 0xAD, 0x14), 2);

        var secondary = RenderAdorner(false);
        secondary.Length.ShouldBe(1);
        secondary[0].Brush.ShouldBeNull();
        AssertPen(secondary[0], Color.FromArgb(0xD9, 0xFA, 0xAD, 0x14), 1);
    }

    [Fact]
    public void Adorner_Disables_Ancestor_Clipping_By_Default()
    {
        var target = new Border();
        var adorner = SemanticPartAdorner.Create(target, isPrimary: true);

        AdornerLayer.GetIsClipEnabled(adorner).ShouldBeFalse();
        adorner.Clip.ShouldBeNull();
        AdornerLayer.GetAdornedElement(adorner).ShouldBe(target);
    }

    [Fact]
    public void Start_Creates_Native_Adorners_And_Dispose_Releases_Them()
    {
        var registry = Application.Current.ShouldNotBeNull()
                                  .GetThemeManager().ShouldNotBeNull()
                                  .SemanticParts;
        registry.TryGetControl(typeof(AtomUIButton), out var descriptor).ShouldBeTrue();
        var part = descriptor.Parts.Single(static candidate => candidate.Path == "content");
        var button = new AtomUIButton
        {
            Content = "Semantic Button"
        };

        using var context = ShowInAdornerHost(button);
        var layer = AdornerLayer.GetAdornerLayer(button).ShouldNotBeNull();
        var session = SemanticPartHighlightSession.Start(button, part, registry);

        session.TotalMatchCount.ShouldBe(1);
        session.HighlightedTargetCount.ShouldBe(1);
        var adorner = layer.Children.OfType<SemanticPartAdorner>().Single();
        AdornerLayer.GetAdornedElement(adorner).ShouldNotBeNull();

        session.Dispose();

        AdornerLayer.GetAdornedElement(adorner).ShouldBeNull();
        layer.Children.ShouldNotContain(adorner);
        session.HighlightedTargetCount.ShouldBe(0);
    }

    [Fact]
    public void Start_Disables_Target_Ancestor_Clipping_For_Outward_Markers()
    {
        var registry = Application.Current.ShouldNotBeNull()
                                  .GetThemeManager().ShouldNotBeNull()
                                  .SemanticParts;
        registry.TryGetControl(typeof(AtomUIMasonry), out var descriptor).ShouldBeTrue();
        var part = descriptor.Parts.Single(static candidate => candidate.Path == "item");
        var masonry = new AtomUIMasonry
        {
            Width = 200,
            Height = 100,
            ColumnCount = 1
        };
        masonry.Items.Add(new Border { Width = 100, Height = 40 });
        using var context = ShowInAdornerHost(masonry);
        using var session = SemanticPartHighlightSession.Start(masonry, part, registry);
        Dispatcher.UIThread.RunJobs();
        var target = masonry.GetVisualDescendants().First(static visual => visual.Classes.Contains("semantic-item"));
        var adorner = context.Layer.Children.OfType<SemanticPartAdorner>().Single();
        AdornerLayer.GetIsClipEnabled(adorner).ShouldBeFalse();
        adorner.Clip.ShouldBeNull();
        adorner.Bounds.ShouldBe(new Rect(-3, -3, target.Bounds.Width + 6, target.Bounds.Height + 6));

        session.Dispose();
        var rootPart = descriptor.Parts.Single(static candidate => candidate.Path == "root");
        using var rootSession = SemanticPartHighlightSession.Start(masonry, rootPart, registry);
        Dispatcher.UIThread.RunJobs();
        var rootAdorner = context.Layer.Children.OfType<SemanticPartAdorner>().Single();
        AdornerLayer.GetIsClipEnabled(rootAdorner).ShouldBeFalse();
        rootAdorner.Clip.ShouldBeNull();
    }

    [Fact]
    public void Start_Gives_Outward_Markers_Their_Own_Layout_Bounds()
    {
        var primaryTarget = new Border
        {
            Width  = 20,
            Height = 4
        };
        primaryTarget.Classes.Add("semantic-item");
        var secondaryTarget = new Border
        {
            Width  = 20,
            Height = 20
        };
        secondaryTarget.Classes.Add("semantic-item");
        Canvas.SetLeft(secondaryTarget, 40);
        Canvas.SetTop(secondaryTarget, 20);

        var owner = new Canvas
        {
            Width  = 100,
            Height = 100,
            Children =
            {
                primaryTarget,
                secondaryTarget
            }
        };
        var descriptor = RuntimeDescriptor(typeof(Canvas), "MarkerBoundsCanvas");
        var registry = new SemanticPartRegistry([descriptor]);
        var part = descriptor.Parts.Single(static candidate => candidate.Path == "item");

        using var context = ShowInAdornerHost(owner);
        using var session = SemanticPartHighlightSession.Start(owner, part, registry);
        Dispatcher.UIThread.RunJobs();

        var adorners = context.Layer.Children.OfType<SemanticPartAdorner>().ToArray();
        var primary = adorners.Single(adorner =>
            AdornerLayer.GetAdornedElement(adorner) == primaryTarget);
        var secondary = adorners.Single(adorner =>
            AdornerLayer.GetAdornedElement(adorner) == secondaryTarget);

        AssertOutwardMarkerBounds(primaryTarget, primary, context.Layer, 3);
        AssertOutwardMarkerBounds(secondaryTarget, secondary, context.Layer, 1);
    }

    [Fact]
    public void Start_Uses_The_Thirty_Two_Visible_Target_Budget()
    {
        var owner = new Grid
        {
            Width  = 400,
            Height = 400
        };
        for (var index = 0; index < 40; index++)
        {
            var target = new Border
            {
                Width  = 20,
                Height = 20
            };
            target.Classes.Add("semantic-item");
            owner.Children.Add(target);
        }

        var descriptor = RuntimeDescriptor(typeof(Grid), "BudgetGrid");
        var registry = new SemanticPartRegistry([descriptor]);
        var part = descriptor.Parts.Single(static candidate => candidate.Path == "item");

        using var context = ShowInAdornerHost(owner);
        using var session = SemanticPartHighlightSession.Start(owner, part, registry);

        session.TotalMatchCount.ShouldBe(40);
        session.HighlightedTargetCount.ShouldBe(32);
        context.Layer.Children.OfType<SemanticPartAdorner>().Count().ShouldBe(32);
    }

    [Fact]
    public void Start_Resolves_Targets_Across_Multiple_Owners()
    {
        var ownerA = new Grid
        {
            Width  = 200,
            Height = 200
        };
        var targetA = new Border
        {
            Width  = 20,
            Height = 20
        };
        targetA.Classes.Add("semantic-item");
        ownerA.Children.Add(targetA);

        var ownerB = new Grid
        {
            Width  = 200,
            Height = 200
        };
        var targetB = new Border
        {
            Width  = 20,
            Height = 20
        };
        targetB.Classes.Add("semantic-item");
        ownerB.Children.Add(targetB);

        var host = new StackPanel
        {
            Children =
            {
                ownerA,
                ownerB
            }
        };
        var descriptor = RuntimeDescriptor(typeof(Grid), "BudgetGrid");
        var registry = new SemanticPartRegistry([descriptor]);
        var part = descriptor.Parts.Single(static candidate => candidate.Path == "item");

        using var context = ShowInAdornerHost(host);
        using var session = SemanticPartHighlightSession.Start([ownerA, ownerB], part, registry);

        session.TotalMatchCount.ShouldBe(2);
        session.HighlightedTargetCount.ShouldBe(2);
        context.Layer.Children.OfType<SemanticPartAdorner>().Count().ShouldBe(2);
    }

    [Fact]
    public void Start_Enforces_A_Single_Shared_Budget_Across_Owners()
    {
        var owners = new List<Control>();
        var host = new StackPanel();
        for (var ownerIndex = 0; ownerIndex < 2; ownerIndex++)
        {
            var owner = new Grid
            {
                Width  = 400,
                Height = 400
            };
            for (var index = 0; index < 20; index++)
            {
                var target = new Border
                {
                    Width  = 20,
                    Height = 20
                };
                target.Classes.Add("semantic-item");
                owner.Children.Add(target);
            }

            owners.Add(owner);
            host.Children.Add(owner);
        }

        var descriptor = RuntimeDescriptor(typeof(Grid), "BudgetGrid");
        var registry = new SemanticPartRegistry([descriptor]);
        var part = descriptor.Parts.Single(static candidate => candidate.Path == "item");

        using var context = ShowInAdornerHost(host);
        using var session = SemanticPartHighlightSession.Start(owners, part, registry);

        session.TotalMatchCount.ShouldBe(40);
        session.HighlightedTargetCount.ShouldBe(32);
        context.Layer.Children.OfType<SemanticPartAdorner>().Count().ShouldBe(32);
    }

    [Fact]
    public void Cross_Root_Session_ReResolves_An_Owner_Template_Popup()
    {
        var popupTarget = new Border
        {
            Width  = 80,
            Height = 40
        };
        popupTarget.Classes.Add("semantic-popup-content");
        var owner = new PopupSemanticOwner(popupTarget);
        var descriptor = PopupDescriptor();
        var registry = new SemanticPartRegistry([descriptor]);
        var part = descriptor.Parts.Single(static candidate => candidate.Path == "popup");

        using var context = ShowInNativeAdornerHost(owner);
        using var session = SemanticPartHighlightSession.Start(owner, part, registry);
        session.HighlightedTargetCount.ShouldBe(0);

        owner.Popup.IsOpen = true;
        Dispatcher.UIThread.RunJobs();

        var popupLayer = AdornerLayer.GetAdornerLayer(popupTarget).ShouldNotBeNull();
        session.HighlightedTargetCount.ShouldBe(1);
        popupLayer.Children.OfType<SemanticPartAdorner>().Count().ShouldBe(1);

        owner.Popup.Close();
        Dispatcher.UIThread.RunJobs();

        session.HighlightedTargetCount.ShouldBe(0);
        popupLayer.Children.OfType<SemanticPartAdorner>().ShouldBeEmpty();
    }

    private static ControlSemanticDescriptor RuntimeDescriptor(Type controlType, string id)
    {
        return new ControlSemanticDescriptor(
            controlType,
            new ControlTokenIdentity("GalleryTests", id),
            [
                Root(controlType),
                new SemanticPartDescriptor(
                    "item",
                    "item",
                    "semantic-item",
                    typeof(Control),
                    SemanticPartCardinality.Multiple,
                    SemanticPartCustomization.Selector,
                    null,
                    false,
                    null,
                    true,
                    "> .semantic-item")
            ]);
    }

    private static ControlSemanticDescriptor PopupDescriptor()
    {
        return new ControlSemanticDescriptor(
            typeof(PopupSemanticOwner),
            new ControlTokenIdentity("GalleryTests", "PopupSemanticOwner"),
            [
                Root(typeof(PopupSemanticOwner)),
                new SemanticPartDescriptor(
                    "popup",
                    "popup",
                    "semantic-popup-content",
                    typeof(Border),
                    SemanticPartCardinality.Single,
                    SemanticPartCustomization.Selector,
                    null,
                    true,
                    null,
                    false)
            ]);
    }

    private static SemanticPartDescriptor Root(Type controlType)
    {
        return new SemanticPartDescriptor(
            "root",
            "root",
            null,
            controlType,
            SemanticPartCardinality.Single,
            SemanticPartCustomization.Root,
            null,
            false,
            null,
            false);
    }

    private static GeometryDrawing[] RenderAdorner(bool isPrimary)
    {
        var adorner = SemanticPartAdorner.Create(new Border(), isPrimary);
        adorner.Measure(new Size(20, 20));
        adorner.Arrange(new Rect(0, 0, 20, 20));

        var drawingGroup = new DrawingGroup();
        using (var context = drawingGroup.Open())
        {
            adorner.Render(context);
        }

        var drawings = drawingGroup.Children.ToArray();
        drawings.ShouldAllBe(static child => child is GeometryDrawing);
        return drawings.Cast<GeometryDrawing>().ToArray();
    }

    private static void AssertPen(GeometryDrawing drawing, Color expectedColor, double expectedThickness)
    {
        var pen = drawing.Pen.ShouldNotBeNull();
        pen.Thickness.ShouldBe(expectedThickness);
        var brush = pen.Brush.ShouldNotBeNull().ShouldBeAssignableTo<ISolidColorBrush>();
        brush.Color.ShouldBe(expectedColor);
    }

    private static void AssertOutwardMarkerBounds(
        Control target,
        SemanticPartAdorner adorner,
        AdornerLayer layer,
        double expectedOutset)
    {
        adorner.Bounds.ShouldBe(new Rect(
            -expectedOutset,
            -expectedOutset,
            target.Bounds.Width + expectedOutset * 2,
            target.Bounds.Height + expectedOutset * 2));

        var targetOrigin = target.TranslatePoint(default, layer).ShouldNotBeNull();
        var adornerOrigin = adorner.TranslatePoint(default, layer).ShouldNotBeNull();
        adornerOrigin.ShouldBe(new Point(
            targetOrigin.X - expectedOutset,
            targetOrigin.Y - expectedOutset));

        var localBounds = new Rect(adorner.Bounds.Size);
        foreach (var drawing in RenderAdorner(adorner))
        {
            var pen = drawing.Pen.ShouldNotBeNull();
            var drawingBounds = drawing.Geometry.ShouldNotBeNull()
                                       .Bounds
                                       .Inflate(pen.Thickness / 2);
            drawingBounds.Left.ShouldBeGreaterThanOrEqualTo(localBounds.Left);
            drawingBounds.Top.ShouldBeGreaterThanOrEqualTo(localBounds.Top);
            drawingBounds.Right.ShouldBeLessThanOrEqualTo(localBounds.Right);
            drawingBounds.Bottom.ShouldBeLessThanOrEqualTo(localBounds.Bottom);
        }
    }

    private static GeometryDrawing[] RenderAdorner(SemanticPartAdorner adorner)
    {
        var drawingGroup = new DrawingGroup();
        using (var context = drawingGroup.Open())
        {
            adorner.Render(context);
        }

        var drawings = drawingGroup.Children.ToArray();
        drawings.ShouldAllBe(static child => child is GeometryDrawing);
        return drawings.Cast<GeometryDrawing>().ToArray();
    }

    private static AdornerHostContext ShowInAdornerHost(Control control)
    {
        var visualLayerManager = new VisualLayerManager
        {
            EnableAdornerLayer = true,
            Child              = control
        };
        var window = new AtomUIWindow
        {
            Width   = 500,
            Height  = 500,
            Content = visualLayerManager
        };

        window.Show();
        control.ApplyTemplate();
        Dispatcher.UIThread.RunJobs();
        return new AdornerHostContext(
            window,
            AdornerLayer.GetAdornerLayer(control).ShouldNotBeNull());
    }

    private static AdornerHostContext ShowInNativeAdornerHost(Control control)
    {
        var window = new AtomUIWindow
        {
            Width   = 500,
            Height  = 500,
            Content = control
        };

        window.Show();
        control.ApplyTemplate();
        Dispatcher.UIThread.RunJobs();
        return new AdornerHostContext(
            window,
            AdornerLayer.GetAdornerLayer(control).ShouldNotBeNull());
    }

    private sealed class AdornerHostContext : IDisposable
    {
        private readonly Window _window;

        public AdornerHostContext(Window window, AdornerLayer layer)
        {
            _window = window;
            Layer   = layer;
        }

        public AdornerLayer Layer { get; }

        public void Dispose()
        {
            _window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private sealed class PopupSemanticOwner : TemplatedControl
    {
        public PopupSemanticOwner(Control popupChild)
        {
            Popup = new Popup
            {
                Child                 = popupChild,
                PlacementTarget       = this,
                ShouldUseOverlayLayer = true
            };
            Template = new FuncControlTemplate<PopupSemanticOwner>((owner, _) =>
                new Grid
                {
                    Children =
                    {
                        new Border
                        {
                            Width  = 120,
                            Height = 80
                        },
                        owner.Popup
                    }
                });
        }

        public Popup Popup { get; }
    }

    [Fact]
    public void Adorner_Marker_Rect_Stays_Inside_The_Expanded_Adorner_Bounds()
    {
        // 50x40 目标的主 adorner 四周各扩 3px，因此 Bounds 为 56x46。
        // 白色 halo 和金框都完全位于这个 Bounds 内，视觉上仍落在目标外侧。
        SemanticPartAdorner.GetMarkerRect(new Size(56, 46), 3, 2.5)
                           .ShouldBe(new Rect(0.5, 0.5, 55, 45));
        SemanticPartAdorner.GetMarkerRect(new Size(56, 46), 3, 1)
                           .ShouldBe(new Rect(2, 2, 52, 42));

        // 副 adorner 四周各扩 1px，1px 描边也保留完整的四条边。
        SemanticPartAdorner.GetMarkerRect(new Size(52, 42), 1, 0.5)
                           .ShouldBe(new Rect(0.5, 0.5, 51, 41));

        // 201x4 的细窄目标扩展后为 207x10，金框不会退化或越出 adorner。
        SemanticPartAdorner.GetMarkerRect(new Size(207, 10), 3, 1)
                           .ShouldBe(new Rect(2, 2, 203, 6));
    }

    [Fact]
    public void Marker_Rect_Is_Clamped_Inside_The_Host_Window_For_Edge_To_Edge_Targets()
    {
        // native 预览对话框的 popup.root/body 是贴边满区目标：adorner 经负 Margin 外扩 3px
        // 并平移到标题栏下方（层坐标 (-3, 37)）后，未钳制的描边矩形左、右、下三条边越出
        // 1210x691 的窗口表面被 OS 裁剪，视觉上只剩顶部一条线。
        var markerRect = SemanticPartAdorner.GetMarkerRect(new Size(1216, 657), 3, 1);

        var clamped = SemanticPartAdorner.ClampMarkerRect(
            markerRect,
            new Rect(0, 0, 1210, 691),
            new Point(-3, 37),
            2);

        // 钳制只内收半个笔宽（对齐上游 Marker：描边沿目标边缘、不内收留白）。
        // 换算回层坐标断言：被钳制的左/右/下边缘距层边缘正好 1px（2px 主笔宽的一半），
        // 描边整条完整落在窗口表面内；顶部本就在层内（标题栏下方 39px），保持原位。
        var layerRect = clamped.Translate(new Vector(-3, 37));
        layerRect.ShouldBe(new Rect(1, 39, 1208, 651));
    }

    [Fact]
    public void Marker_Rect_Clamp_Keeps_Interior_Targets_Untouched()
    {
        // 有余量的常规目标（cover 演示位）钳制不应改变描边。
        var markerRect = SemanticPartAdorner.GetMarkerRect(new Size(246, 242), 3, 1);

        var clamped = SemanticPartAdorner.ClampMarkerRect(
            markerRect,
            new Rect(0, 0, 1300, 900),
            new Point(321, 250.5),
            3);

        clamped.ShouldBe(markerRect);
    }
}

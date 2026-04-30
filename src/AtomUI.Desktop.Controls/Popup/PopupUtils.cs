using AtomUI.Controls;
using AtomUI.MotionScene;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;

namespace AtomUI.Desktop.Controls;

internal static class PopupUtils
{
    internal static ArrowPosition? CalculateArrowPosition(
        PlacementMode placement,
        PopupAnchor? anchor,
        PopupGravity? gravity)
    {
        if (!CanEnabledArrow(placement, anchor, gravity))
        {
            return null;
        }

        if (placement != PlacementMode.AnchorAndGravity)
        {
            var ret = GetAnchorAndGravity(placement);
            anchor = ret.Item1;
            gravity = ret.Item2;
        }

        ArrowPosition? arrowPosition;
        switch (anchor, gravity)
        {
            case (PopupAnchor.Bottom, PopupGravity.Bottom):
                arrowPosition = ArrowPosition.Top;
                break;
            case (PopupAnchor.Right, PopupGravity.Right):
                arrowPosition = ArrowPosition.Left;
                break;
            case (PopupAnchor.Left, PopupGravity.Left):
                arrowPosition = ArrowPosition.Right;
                break;
            case (PopupAnchor.Top, PopupGravity.Top):
                arrowPosition = ArrowPosition.Bottom;
                break;
            case (PopupAnchor.TopRight, PopupGravity.TopLeft):
                arrowPosition = ArrowPosition.BottomEdgeAlignedRight;
                break;
            case (PopupAnchor.TopLeft, PopupGravity.TopRight):
                arrowPosition = ArrowPosition.BottomEdgeAlignedLeft;
                break;
            case (PopupAnchor.BottomLeft, PopupGravity.BottomRight):
                arrowPosition = ArrowPosition.TopEdgeAlignedLeft;
                break;
            case (PopupAnchor.BottomRight, PopupGravity.BottomLeft):
                arrowPosition = ArrowPosition.TopEdgeAlignedRight;
                break;
            case (PopupAnchor.TopLeft, PopupGravity.BottomLeft):
                arrowPosition = ArrowPosition.RightEdgeAlignedTop;
                break;
            case (PopupAnchor.BottomLeft, PopupGravity.TopLeft):
                arrowPosition = ArrowPosition.RightEdgeAlignedBottom;
                break;
            case (PopupAnchor.TopRight, PopupGravity.BottomRight):
                arrowPosition = ArrowPosition.LeftEdgeAlignedTop;
                break;
            case (PopupAnchor.BottomRight, PopupGravity.TopRight):
                arrowPosition = ArrowPosition.LeftEdgeAlignedBottom;
                break;
            default:
                arrowPosition = null;
                break;
        }

        return arrowPosition;
    }

    /// <summary>
    /// 判断是否可以启用箭头，有些组合是不能启用箭头绘制的，因为没有意义
    /// </summary>
    /// <param name="placement"></param>
    /// <param name="anchor"></param>
    /// <param name="gravity"></param>
    /// <returns></returns>
    internal static bool CanEnabledArrow(PlacementMode placement, PopupAnchor? anchor = null,
        PopupGravity? gravity = null)
    {
        if (placement == PlacementMode.Center || placement == PlacementMode.Pointer)
        {
            return false;
        }

        return IsCanonicalAnchorType(placement, anchor, gravity);
    }

    /// <summary>
    /// 是否为标准的 anchor 类型
    /// </summary>
    /// <param name="placement"></param>
    /// <param name="anchor"></param>
    /// <param name="gravity"></param>
    /// <returns></returns>
    internal static bool IsCanonicalAnchorType(PlacementMode placement, PopupAnchor? anchor, PopupGravity? gravity)
    {
        if (placement == PlacementMode.AnchorAndGravity)
        {
            switch (anchor, gravity)
            {
                case (PopupAnchor.Bottom, PopupGravity.Bottom):
                case (PopupAnchor.Right, PopupGravity.Right):
                case (PopupAnchor.Left, PopupGravity.Left):
                case (PopupAnchor.Top, PopupGravity.Top):
                case (PopupAnchor.TopRight, PopupGravity.TopLeft):
                case (PopupAnchor.TopLeft, PopupGravity.TopRight):
                case (PopupAnchor.BottomLeft, PopupGravity.BottomRight):
                case (PopupAnchor.BottomRight, PopupGravity.BottomLeft):
                case (PopupAnchor.TopLeft, PopupGravity.BottomLeft):
                case (PopupAnchor.BottomLeft, PopupGravity.TopLeft):
                case (PopupAnchor.TopRight, PopupGravity.BottomRight):
                case (PopupAnchor.BottomRight, PopupGravity.TopRight):
                    break;
                default:
                    return false;
            }
        }

        return true;
    }

    internal static bool IsCanonicalAnchorPlacementMode(PlacementMode placement)
    {
        return placement switch
        {
            PlacementMode.Bottom => true,
            PlacementMode.Right => true,
            PlacementMode.Left => true,
            PlacementMode.Top => true,
            PlacementMode.TopEdgeAlignedRight => true,
            PlacementMode.TopEdgeAlignedLeft => true,
            PlacementMode.BottomEdgeAlignedLeft => true,
            PlacementMode.BottomEdgeAlignedRight => true,
            PlacementMode.LeftEdgeAlignedTop => true,
            PlacementMode.LeftEdgeAlignedBottom => true,
            PlacementMode.RightEdgeAlignedTop => true,
            PlacementMode.RightEdgeAlignedBottom => true,
            _ => false
        };
    }

    internal static (AbstractMotion? open, AbstractMotion? close) CreateMotionForPlacement(PlacementMode placement)
    {
        if (!IsCanonicalAnchorPlacementMode(placement))
        {
            return (null, null);
        }

        return (new ZoomBigInMotion(), new ZoomBigOutMotion());
    }
    
    public static void ValidateEdge(this PopupAnchor edge)
    {
        if (edge.HasAllFlags(PopupAnchor.Left | PopupAnchor.Right) ||
            edge.HasAllFlags(PopupAnchor.Top | PopupAnchor.Bottom))
        {
            throw new ArgumentException("Opposite edges specified");
        }
    }

    public static void ValidateGravity(this PopupGravity gravity)
    {
        ValidateEdge((PopupAnchor)gravity);
    }

    public static PopupAnchor Flip(this PopupAnchor edge)
    {
        if (edge.HasAnyFlag(PopupAnchor.HorizontalMask))
        {
            edge ^= PopupAnchor.HorizontalMask;
        }

        if (edge.HasAnyFlag(PopupAnchor.VerticalMask))
        {
            edge ^= PopupAnchor.VerticalMask;
        }

        return edge;
    }

    public static PopupAnchor FlipX(this PopupAnchor edge)
    {
        if (edge.HasAnyFlag(PopupAnchor.HorizontalMask))
        {
            edge ^= PopupAnchor.HorizontalMask;
        }

        return edge;
    }

    public static PopupAnchor FlipY(this PopupAnchor edge)
    {
        if (edge.HasAnyFlag(PopupAnchor.VerticalMask))
        {
            edge ^= PopupAnchor.VerticalMask;
        }

        return edge;
    }

    public static PopupGravity FlipX(this PopupGravity gravity)
    {
        return (PopupGravity)FlipX((PopupAnchor)gravity);
    }

    public static PopupGravity FlipY(this PopupGravity gravity)
    {
        return (PopupGravity)FlipY((PopupAnchor)gravity);
    }

    internal static Point GetAnchorPoint(Rect anchorRect, PopupAnchor edge)
    {
        double x, y;
        if (edge.HasAllFlags(PopupAnchor.Left))
        {
            x = anchorRect.X;
        }
        else if (edge.HasAllFlags(PopupAnchor.Right))
        {
            x = anchorRect.Right;
        }
        else
        {
            x = anchorRect.X + anchorRect.Width / 2;
        }

        if (edge.HasAllFlags(PopupAnchor.Top))
        {
            y = anchorRect.Y;
        }
        else if (edge.HasAllFlags(PopupAnchor.Bottom))
        {
            y = anchorRect.Bottom;
        }
        else
        {
            y = anchorRect.Y + anchorRect.Height / 2;
        }

        return new Point(x, y);
    }

    internal static Point Gravitate(Point anchorPoint, Size size, PopupGravity gravity)
    {
        double x, y;
        if (gravity.HasAllFlags(PopupGravity.Left))
        {
            x = -size.Width;
        }
        else if (gravity.HasAllFlags(PopupGravity.Right))
        {
            x = 0;
        }
        else
        {
            x = -size.Width / 2;
        }

        if (gravity.HasAllFlags(PopupGravity.Top))
        {
            y = -size.Height;
        }
        else if (gravity.HasAllFlags(PopupGravity.Bottom))
        {
            y = 0;
        }
        else
        {
            y = -size.Height / 2;
        }

        return anchorPoint + new Point(x, y);
    }

    internal static Direction GetDirection(PlacementMode placement)
    {
        return placement switch
        {
            PlacementMode.Left => Direction.Left,
            PlacementMode.LeftEdgeAlignedBottom => Direction.Left,
            PlacementMode.LeftEdgeAlignedTop => Direction.Left,

            PlacementMode.Top => Direction.Top,
            PlacementMode.TopEdgeAlignedLeft => Direction.Top,
            PlacementMode.TopEdgeAlignedRight => Direction.Top,

            PlacementMode.Right => Direction.Right,
            PlacementMode.RightEdgeAlignedBottom => Direction.Right,
            PlacementMode.RightEdgeAlignedTop => Direction.Right,

            PlacementMode.Bottom => Direction.Bottom,
            PlacementMode.BottomEdgeAlignedLeft => Direction.Bottom,
            PlacementMode.BottomEdgeAlignedRight => Direction.Bottom,
            _ => throw new ArgumentOutOfRangeException(nameof(placement), placement, "Invalid value for PlacementMode")
        };
    }

    internal static Direction FlipDirection(Direction direction)
    {
        if (direction == Direction.Left)
        {
            direction = Direction.Right;
        }
        else if (direction == Direction.Right)
        {
            direction = Direction.Left;
        }
        else if (direction == Direction.Top)
        {
            direction = Direction.Bottom;
        }
        else if (direction == Direction.Bottom)
        {
            direction = Direction.Top;
        }
        return direction;
    }

    /// <summary>
    /// 判断在给定的自定义放置参数下，<c>PopupRoot</c>（独立窗口弹窗）最终会在 X / Y 轴上发生翻转。
    /// 实现与 Avalonia 的 <c>ManagedPopupPositioner.Calculate</c> 完全一致。
    /// </summary>
    /// <param name="placement">自定义放置参数（来源于 <c>Popup.CustomPopupPlacementCallback</c>）。</param>
    /// <returns>X 轴是否翻转、Y 轴是否翻转。</returns>
    internal static (bool flipX, bool flipY) CalculatePopupRootFlipInfo(CustomPopupPlacement placement)
    {
        var target = placement.Target;
        if (placement.PopupSize.Width <= 0 || placement.PopupSize.Height <= 0)
        {
            return (false, false);
        }

        var topLevel = TopLevel.GetTopLevel(target);
        var screens  = topLevel?.Screens;
        if (topLevel == null || screens == null || screens.ScreenCount == 0)
        {
            return (false, false);
        }

        // —— 与 Avalonia ManagedPopupPositioner 保持单位一致 ——
        // 关键：使用 DesktopScaling（不是 RenderScaling），原因：
        //   * Win32:  DesktopScaling == RenderScaling，整套换算到物理像素；
        //   * macOS:  DesktopScaling == 1（原生硬编码），PointToScreen 与 NSScreen.frame 都是 Cocoa 逻辑点，
        //             ManagedPopupPositionerPopupImplHelper.Scaling 用的也是 DesktopScaling，整套在逻辑点系。
        // 这样保证 anchorRect、clientScreenPos、screen.Bounds 三者单位完全一致。
        var scaling         = topLevel.PlatformImpl?.DesktopScaling ?? 1.0;
        var clientScreenPos = topLevel.PointToScreen(default);
        var anchorRectPx    = new Rect(
            placement.AnchorRectangle.X * scaling + clientScreenPos.X,
            placement.AnchorRectangle.Y * scaling + clientScreenPos.Y,
            placement.AnchorRectangle.Width  * scaling,
            placement.AnchorRectangle.Height * scaling);
        var popupSizePx = placement.PopupSize * scaling;
        var offsetPx    = placement.Offset    * scaling;

        // —— 选屏：与 ManagedPopupPositioner.GetBounds 优先级一致：先 anchor 所在屏，再 TopLevel 所在屏，再主屏 ——
        var anchorPixelRect = new PixelRect(
            (int)anchorRectPx.X, (int)anchorRectPx.Y,
            (int)anchorRectPx.Width, (int)anchorRectPx.Height);
        var screen = screens.ScreenFromBounds(anchorPixelRect)
                  ?? screens.ScreenFromTopLevel(topLevel)
                  ?? screens.Primary
                  ?? screens.All.FirstOrDefault();
        if (screen == null)
        {
            return (false, false);
        }

        var workingAreaPx = screen.WorkingArea;
        var bounds = (workingAreaPx.Width == 0 && workingAreaPx.Height == 0)
            ? screen.Bounds.ToRect(1.0)
            : workingAreaPx.ToRect(1.0);

        return CalculateFlipInfoCore(placement, anchorRectPx, bounds, popupSizePx, offsetPx);
    }

    /// <summary>
    /// 判断在给定的自定义放置参数下，<c>OverlayPopupHost</c>（Overlay 层弹窗）最终会在 X / Y 轴上发生翻转。
    /// 实现与 Avalonia 的 <c>OverlayPopupHost + ManagedPopupPositioner.Calculate</c> 完全一致：
    /// 使用 TopLevel 客户区逻辑坐标，并按 SafeAreaPadding 收缩可用区域。
    /// </summary>
    /// <param name="placement">自定义放置参数（来源于 <c>Popup.CustomPopupPlacementCallback</c>）。</param>
    /// <returns>X 轴是否翻转、Y 轴是否翻转。</returns>
    internal static (bool flipX, bool flipY) CalculateOverlayPopupHostFlipInfo(CustomPopupPlacement placement)
    {
        var target = placement.Target;
        if (placement.PopupSize.Width <= 0 || placement.PopupSize.Height <= 0)
        {
            return (false, false);
        }

        var topLevel = TopLevel.GetTopLevel(target);
        if (topLevel == null)
        {
            return (false, false);
        }

        var bounds = new Rect(default, topLevel.ClientSize);
        var padding = topLevel.InsetsManager?.SafeAreaPadding ?? default;
        if (padding != default)
        {
            bounds = bounds.Deflate(padding);
        }

        return CalculateFlipInfoCore(
            placement,
            placement.AnchorRectangle,
            bounds,
            placement.PopupSize,
            placement.Offset);
    }

    private static (bool flipX, bool flipY) CalculateFlipInfoCore(
        CustomPopupPlacement placement,
        Rect anchorRect,
        Rect bounds,
        Size popupSize,
        Point offset)
    {
        var anchor               = placement.Anchor;
        var gravity              = placement.Gravity;
        var constraintAdjustment = placement.ConstraintAdjustment;

        Rect GetUnconstrained(PopupAnchor a, PopupGravity g) =>
            new Rect(Gravitate(GetAnchorPoint(anchorRect, a), popupSize, g) + offset, popupSize);

        bool FitsInBounds(Rect rc, PopupAnchor edge)
        {
            if (edge.HasAllFlags(PopupAnchor.Left)   && rc.X      < bounds.X      ||
                edge.HasAllFlags(PopupAnchor.Top)    && rc.Y      < bounds.Y      ||
                edge.HasAllFlags(PopupAnchor.Right)  && rc.Right  > bounds.Right  ||
                edge.HasAllFlags(PopupAnchor.Bottom) && rc.Bottom > bounds.Bottom)
            {
                return false;
            }

            return true;
        }

        var geo   = GetUnconstrained(anchor, gravity);
        var flipX = false;
        var flipY = false;

        if (!FitsInBounds(geo, PopupAnchor.HorizontalMask)
            && constraintAdjustment.HasAllFlags(PopupPositionerConstraintAdjustment.FlipX))
        {
            var flipped = GetUnconstrained(anchor.FlipX(), gravity.FlipX());
            if (FitsInBounds(flipped, PopupAnchor.HorizontalMask))
            {
                flipX = true;
                geo   = geo.WithX(flipped.X);
            }
        }

        if (!FitsInBounds(geo, PopupAnchor.VerticalMask)
            && constraintAdjustment.HasAllFlags(PopupPositionerConstraintAdjustment.FlipY))
        {
            var flipped = GetUnconstrained(anchor.FlipY(), gravity.FlipY());
            if (FitsInBounds(flipped, PopupAnchor.VerticalMask))
            {
                flipY = true;
            }
        }

        return (flipX, flipY);
    }

    internal static (PopupAnchor, PopupGravity) GetAnchorAndGravity(PlacementMode placement)
    {
        return placement switch
        {
            PlacementMode.Bottom => (PopupAnchor.Bottom, PopupGravity.Bottom),
            PlacementMode.Right => (PopupAnchor.Right, PopupGravity.Right),
            PlacementMode.Left => (PopupAnchor.Left, PopupGravity.Left),
            PlacementMode.Top => (PopupAnchor.Top, PopupGravity.Top),
            PlacementMode.TopEdgeAlignedRight => (PopupAnchor.TopRight, PopupGravity.TopLeft),
            PlacementMode.TopEdgeAlignedLeft => (PopupAnchor.TopLeft, PopupGravity.TopRight),
            PlacementMode.BottomEdgeAlignedLeft => (PopupAnchor.BottomLeft, PopupGravity.BottomRight),
            PlacementMode.BottomEdgeAlignedRight => (PopupAnchor.BottomRight, PopupGravity.BottomLeft),
            PlacementMode.LeftEdgeAlignedTop => (PopupAnchor.TopLeft, PopupGravity.BottomLeft),
            PlacementMode.LeftEdgeAlignedBottom => (PopupAnchor.BottomLeft, PopupGravity.TopLeft),
            PlacementMode.RightEdgeAlignedTop => (PopupAnchor.TopRight, PopupGravity.BottomRight),
            PlacementMode.RightEdgeAlignedBottom => (PopupAnchor.BottomRight, PopupGravity.TopRight),
            _ => (PopupAnchor.None, PopupGravity.None)
        };
    }
    
    internal static (double offsetX, double offsetY) CalculateShadowOffset(ArrowPosition arrowPosition, Thickness shadowThickness, Rect arrowBounds)
    {
        var shadowOffsetX = 0.0d;
        var shadowOffsetY = 0.0d;

        var arrowWidth              = arrowBounds.Width;
        var arrowHeight             = arrowBounds.Height;
        var leftResidualShadow      = Math.Max(shadowThickness.Left   - arrowWidth, 0);
        var rightResidualShadow     = Math.Max(shadowThickness.Right  - arrowWidth, 0);
        var topResidualShadow       = Math.Max(shadowThickness.Top    - arrowHeight, 0);
        var bottomResidualShadow    = Math.Max(shadowThickness.Bottom - arrowHeight, 0);
        var centeredVerticalDelta   = (shadowThickness.Bottom - shadowThickness.Top) / 2;
        var centeredHorizontalDelta = (shadowThickness.Right  - shadowThickness.Left) / 2;

        switch (arrowPosition)
        {
            case ArrowPosition.Left:
                shadowOffsetX = leftResidualShadow;
                shadowOffsetY = centeredVerticalDelta;
                break;
            case ArrowPosition.LeftEdgeAlignedTop:
                shadowOffsetX = leftResidualShadow;
                shadowOffsetY = -shadowThickness.Top;
                break;
            case ArrowPosition.LeftEdgeAlignedBottom:
                shadowOffsetX = leftResidualShadow;
                shadowOffsetY = shadowThickness.Bottom;
                break;
            case ArrowPosition.Right:
                shadowOffsetX = -rightResidualShadow;
                shadowOffsetY = centeredVerticalDelta;
                break;
            case ArrowPosition.RightEdgeAlignedTop:
                shadowOffsetX = -rightResidualShadow;
                shadowOffsetY = -shadowThickness.Top;
                break;
            case ArrowPosition.RightEdgeAlignedBottom:
                shadowOffsetX = -rightResidualShadow;
                shadowOffsetY = shadowThickness.Bottom;
                break;
            case ArrowPosition.Top:
                shadowOffsetX = centeredHorizontalDelta;
                shadowOffsetY = topResidualShadow;
                break;
            case ArrowPosition.TopEdgeAlignedLeft:
                shadowOffsetX = -shadowThickness.Left;
                shadowOffsetY = topResidualShadow;
                break;
            case ArrowPosition.TopEdgeAlignedRight:
                shadowOffsetX = shadowThickness.Right;
                shadowOffsetY = topResidualShadow;
                break;
            case ArrowPosition.Bottom:
                shadowOffsetX = centeredHorizontalDelta;
                shadowOffsetY = -bottomResidualShadow;
                break;
            case ArrowPosition.BottomEdgeAlignedLeft:
                shadowOffsetX = -shadowThickness.Left;
                shadowOffsetY = -bottomResidualShadow;
                break;
            case ArrowPosition.BottomEdgeAlignedRight:
                shadowOffsetX = shadowThickness.Right;
                shadowOffsetY = -bottomResidualShadow;
                break;
        }

        return (shadowOffsetX, shadowOffsetY);
    }

    internal static bool ApplyCustomPlacement(
        CustomPopupPlacement placement,
        PlacementMode requestedPlacement,
        bool isUseOverlayHost,
        double hOffset,
        double vOffset,
        double marginToAnchor,
        Thickness shadowThickness,
        PopupAnchor anchor,
        PopupGravity gravity,
        Rect arrowIndicatorLayoutBounds = default)
    {
        placement.Anchor      = anchor;
        placement.Gravity     = gravity;

        var (flipX, flipY) = isUseOverlayHost
            ? CalculateOverlayPopupHostFlipInfo(placement)
            : CalculatePopupRootFlipInfo(placement);

        if (requestedPlacement == PlacementMode.Pointer)
        {
            hOffset += shadowThickness.Left + marginToAnchor;
            vOffset += shadowThickness.Top + marginToAnchor;
        }
        else
        {
            if (!isUseOverlayHost)
            {
                var arrowPosition = CalculateArrowPosition(requestedPlacement, anchor, gravity);
                if (arrowPosition != null)
                {
                    if (flipX || flipY)
                    {
                        arrowPosition = ArrowPositionUtils.FlipArrowPosition(arrowPosition.Value);
                    }

                    var (shadowOffsetX, shadowOffsetY) = CalculateShadowOffset(arrowPosition.Value,
                        shadowThickness, arrowIndicatorLayoutBounds);
                    hOffset += shadowOffsetX;
                    vOffset += shadowOffsetY;
                }
            }

            var direction = GetDirection(requestedPlacement);
            if (flipX || flipY)
            {
                direction = FlipDirection(direction);
            }

            if (direction == Direction.Left)
            {
                hOffset -= marginToAnchor;
            }
            else if (direction == Direction.Right)
            {
                hOffset += marginToAnchor;
            }
            else if (direction == Direction.Top)
            {
                vOffset -= marginToAnchor;
            }
            else
            {
                vOffset += marginToAnchor;
            }
        }

        placement.Offset = new Point(hOffset, vOffset);
        return flipX || flipY;
    }
}

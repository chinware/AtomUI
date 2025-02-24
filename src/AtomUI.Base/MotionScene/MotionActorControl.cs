using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Transformation;

namespace AtomUI.MotionScene;

internal class MotionActorControl : Decorator
{
    #region 公共属性定义

    public static readonly StyledProperty<TransformOperations?> MotionTransformProperty =
        AvaloniaProperty.Register<MotionActorControl, TransformOperations?>(nameof(MotionTransform));

    public static readonly StyledProperty<bool> UseRenderTransformProperty =
        AvaloniaProperty.Register<MotionActorControl, bool>(nameof(UseRenderTransform));

    public TransformOperations? MotionTransform
    {
        get => GetValue(MotionTransformProperty);
        set => SetValue(MotionTransformProperty, value);
    }

    public bool UseRenderTransform
    {
        get => GetValue(UseRenderTransformProperty);
        set => SetValue(UseRenderTransformProperty, value);
    }

    public Control? MotionTransformRoot => Child;

    #endregion

    #region 公共事件定义

    public event EventHandler? PreStart;
    public event EventHandler? Completed;

    #endregion

    /// <summary>
    /// RenderTransform/MatrixTransform applied to MotionTransformRoot.
    /// </summary>
    private readonly MatrixTransform _matrixTransform = new();

    /// <summary>
    /// Transformation matrix corresponding to _matrixTransform.
    /// </summary>
    private Matrix _transformation = Matrix.Identity;

    private IDisposable? _transformChangedEvent;

    /// <summary>
    /// Acceptable difference between two doubles.
    /// </summary>
    private const double AcceptableDelta = 0.0001;

    /// <summary>
    /// Number of decimals to round the Matrix to.
    /// </summary>
    private const int DecimalsAfterRound = 4;

    /// <summary>
    /// Actual DesiredSize of Child element (the value it returned from its MeasureOverride method).
    /// </summary>
    private Size _childActualSize;

    /// <summary>
    /// 动画是否在
    /// </summary>
    private bool _animating = false;

    static MotionActorControl()
    {
        ClipToBoundsProperty.OverrideDefaultValue<MotionActorControl>(true);

        MotionTransformProperty.Changed
                               .AddClassHandler<MotionActorControl>((x, e) => x.HandleLayoutTransformChanged(e));

        ChildProperty.Changed
                     .AddClassHandler<MotionActorControl>((x, _) => x.HandleChildChanged());
        AffectsRender<MotionActorControl>(MotionTransformProperty);
    }

    private void HandleLayoutTransformChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var newTransform = e.NewValue as Transform;

        _transformChangedEvent?.Dispose();
        _transformChangedEvent = null;

        if (newTransform != null)
        {
            _transformChangedEvent = Observable.FromEventPattern(
                                                   v => newTransform.Changed += v, v => newTransform.Changed -= v)
                                               .Subscribe(_ => ApplyMotionTransform());
        }

        ApplyMotionTransform();
    }

    private void HandleChildChanged()
    {
        if (null != MotionTransformRoot)
        {
            // 这里我们会过滤掉 Scale 缩放
            MotionTransformRoot.RenderTransform       = _matrixTransform;
            MotionTransformRoot.RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Absolute);
        }

        ApplyMotionTransform();
    }

    /// <summary>
    /// Applies the layout transform on the LayoutTransformerControl content.
    /// </summary>
    /// <remarks>
    /// Only used in advanced scenarios (like animating the LayoutTransform).
    /// Should be used to notify the LayoutTransformer control that some aspect
    /// of its Transform property has changed.
    /// </remarks>
    private void ApplyMotionTransform()
    {
        // Get the transform matrix and apply it
        var matrix = MotionTransform is null ? Matrix.Identity : RoundMatrix(MotionTransform.Value, DecimalsAfterRound);

        if (_transformation == matrix)
        {
            return;
        }

        _transformation         = matrix;
        _matrixTransform.Matrix = UseRenderTransform ? matrix : FilterScaleTransform(matrix);
        RenderTransform         = _matrixTransform;
        // New transform means re-layout is necessary
        InvalidateMeasure();
    }

    /// <summary>
    /// Rounds the non-offset elements of a Matrix to avoid issues due to floating point imprecision.
    /// </summary>
    /// <param name="matrix">Matrix to round.</param>
    /// <param name="decimals">Number of decimal places to round to.</param>
    /// <returns>Rounded Matrix.</returns>
    private static Matrix RoundMatrix(Matrix matrix, int decimals)
    {
        return new Matrix(
            Math.Round(matrix.M11, decimals),
            Math.Round(matrix.M12, decimals),
            Math.Round(matrix.M21, decimals),
            Math.Round(matrix.M22, decimals),
            matrix.M31,
            matrix.M32);
    }

    private static Matrix FilterScaleTransform(Matrix matrix)
    {
        return new Matrix(
            1.0,
            matrix.M12,
            matrix.M21,
            1.0,
            matrix.M31,
            matrix.M32);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (MotionTransformRoot == null || MotionTransform == null || UseRenderTransform)
        {
            // TODO 这里可能会引起混淆，因为我们不会对 Target 实施 Scale 转换
            return base.ArrangeOverride(finalSize);
        }

        // Determine the largest available size after the transformation
        Size finalSizeTransformed = ComputeLargestTransformedSize(finalSize);

        if (IsSizeSmaller(finalSizeTransformed, MotionTransformRoot.DesiredSize))
        {
            // Some elements do not like being given less space than they asked for (ex: TextBlock)
            // Bump the working size up to do the right thing by them
            finalSizeTransformed = MotionTransformRoot.DesiredSize;
        }

        // Transform the working size to find its width/height
        Rect transformedRect =
            new Rect(0, 0, finalSizeTransformed.Width, finalSizeTransformed.Height).TransformToAABB(_transformation);
        // Create the Arrange rect to center the transformed content
        Rect finalRect = new Rect(
            -transformedRect.X + ((finalSize.Width - transformedRect.Width) / 2),
            -transformedRect.Y + ((finalSize.Height - transformedRect.Height) / 2),
            finalSizeTransformed.Width,
            finalSizeTransformed.Height);

        // Perform an Arrange on MotionTransformRoot (containing Child)
        Size arrangedsize;
        MotionTransformRoot.Arrange(finalRect);
        arrangedsize = MotionTransformRoot.Bounds.Size;

        // This is the first opportunity under Silverlight to find out the Child's true DesiredSize
        if (IsSizeSmaller(finalSizeTransformed, arrangedsize) && _childActualSize == default)
        {
            //// Unfortunately, all the work so far is invalid because the wrong DesiredSize was used
            //// Make a note of the actual DesiredSize
            // _childActualSize = arrangedsize;
            // //// Force a new measure/arrange pass
            // InvalidateMeasure();
        }
        else
        {
            // Clear the "need to measure/arrange again" flag
            _childActualSize = default;
        }

        // Return result to perform the transformation
        return finalSize;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (MotionTransformRoot == null || MotionTransform == null || UseRenderTransform)
        {
            return base.MeasureOverride(availableSize);
        }

        Size measureSize;
        if (_childActualSize == default)
        {
            // Determine the largest size after the transformation
            measureSize = ComputeLargestTransformedSize(availableSize);
        }
        else
        {
            // Previous measure/arrange pass determined that Child.DesiredSize was larger than believed
            measureSize = _childActualSize;
        }

        // Perform a measure on the MotionTransformRoot (containing Child)
        if (MotionTransformRoot.DesiredSize == default || _animating == false)
        {
            MotionTransformRoot.Measure(measureSize);
        }

        var desiredSize = MotionTransformRoot.DesiredSize;

        // Transform DesiredSize to find its width/height
        Rect transformedDesiredRect =
            new Rect(0, 0, desiredSize.Width, desiredSize.Height).TransformToAABB(_transformation);
        Size transformedDesiredSize = new Size(transformedDesiredRect.Width, transformedDesiredRect.Height);

        // Return result to allocate enough space for the transformation
        return transformedDesiredSize;
    }

    /// <summary>
    /// Compute the largest usable size (greatest area) after applying the transformation to the specified bounds.
    /// </summary>
    /// <param name="arrangeBounds">Arrange bounds.</param>
    /// <returns>Largest Size possible.</returns>
    private Size ComputeLargestTransformedSize(Size arrangeBounds)
    {
        // Computed largest transformed size
        Size computedSize = default;

        // Detect infinite bounds and constrain the scenario
        bool infiniteWidth = double.IsInfinity(arrangeBounds.Width);
        if (infiniteWidth)
        {
            // arrangeBounds.Width = arrangeBounds.Height;
            arrangeBounds = arrangeBounds.WithWidth(arrangeBounds.Height);
        }

        bool infiniteHeight = double.IsInfinity(arrangeBounds.Height);
        if (infiniteHeight)
        {
            //arrangeBounds.Height = arrangeBounds.Width;
            arrangeBounds = arrangeBounds.WithHeight(arrangeBounds.Width);
        }

        // Capture the matrix parameters
        double a = _transformation.M11;
        double b = _transformation.M12;
        double c = _transformation.M21;
        double d = _transformation.M22;

        // Compute maximum possible transformed width/height based on starting width/height
        // These constraints define two lines in the positive x/y quadrant
        double maxWidthFromWidth   = Math.Abs(arrangeBounds.Width / a);
        double maxHeightFromWidth  = Math.Abs(arrangeBounds.Width / c);
        double maxWidthFromHeight  = Math.Abs(arrangeBounds.Height / b);
        double maxHeightFromHeight = Math.Abs(arrangeBounds.Height / d);

        // The transformed width/height that maximize the area under each segment is its midpoint
        // At most one of the two midpoints will satisfy both constraints
        double idealWidthFromWidth   = maxWidthFromWidth / 2;
        double idealHeightFromWidth  = maxHeightFromWidth / 2;
        double idealWidthFromHeight  = maxWidthFromHeight / 2;
        double idealHeightFromHeight = maxHeightFromHeight / 2;

        // Compute slope of both constraint lines
        double slopeFromWidth  = -(maxHeightFromWidth / maxWidthFromWidth);
        double slopeFromHeight = -(maxHeightFromHeight / maxWidthFromHeight);

        if ((0 == arrangeBounds.Width) || (0 == arrangeBounds.Height))
        {
            // Check for empty bounds
            computedSize = new Size(arrangeBounds.Width, arrangeBounds.Height);
        }
        else if (infiniteWidth && infiniteHeight)
        {
            // Check for completely unbound scenario
            computedSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
        }
        else if (!_transformation.HasInverse)
        {
            // Check for singular matrix
            computedSize = new Size(0, 0);
        }
        else if ((0 == b) || (0 == c))
        {
            // Check for 0/180 degree special cases
            double maxHeight = (infiniteHeight ? double.PositiveInfinity : maxHeightFromHeight);
            double maxWidth  = (infiniteWidth ? double.PositiveInfinity : maxWidthFromWidth);
            if ((0 == b) && (0 == c))
            {
                // No constraints
                computedSize = new Size(maxWidth, maxHeight);
            }
            else if (0 == b)
            {
                // Constrained by width
                double computedHeight = Math.Min(idealHeightFromWidth, maxHeight);
                computedSize = new Size(
                    maxWidth - Math.Abs((c * computedHeight) / a),
                    computedHeight);
            }
            else if (0 == c)
            {
                // Constrained by height
                double computedWidth = Math.Min(idealWidthFromHeight, maxWidth);
                computedSize = new Size(
                    computedWidth,
                    maxHeight - Math.Abs((b * computedWidth) / d));
            }
        }
        else if ((0 == a) || (0 == d))
        {
            // Check for 90/270 degree special cases
            double maxWidth  = (infiniteHeight ? double.PositiveInfinity : maxWidthFromHeight);
            double maxHeight = (infiniteWidth ? double.PositiveInfinity : maxHeightFromWidth);
            if ((0 == a) && (0 == d))
            {
                // No constraints
                computedSize = new Size(maxWidth, maxHeight);
            }
            else if (0 == a)
            {
                // Constrained by width
                double computedHeight = Math.Min(idealHeightFromHeight, maxHeight);
                computedSize = new Size(
                    maxWidth - Math.Abs((d * computedHeight) / b),
                    computedHeight);
            }
            else if (0 == d)
            {
                // Constrained by height
                double computedWidth = Math.Min(idealWidthFromWidth, maxWidth);
                computedSize = new Size(
                    computedWidth,
                    maxHeight - Math.Abs((a * computedWidth) / c));
            }
        }
        else if (idealHeightFromWidth <= ((slopeFromHeight * idealWidthFromWidth) + maxHeightFromHeight))
        {
            // Check the width midpoint for viability (by being below the height constraint line)
            computedSize = new Size(idealWidthFromWidth, idealHeightFromWidth);
        }
        else if (idealHeightFromHeight <= ((slopeFromWidth * idealWidthFromHeight) + maxHeightFromWidth))
        {
            // Check the height midpoint for viability (by being below the width constraint line)
            computedSize = new Size(idealWidthFromHeight, idealHeightFromHeight);
        }
        else
        {
            // Neither midpoint is viable; use the intersection of the two constraint lines instead
            // Compute width by setting heights equal (m1*x+c1=m2*x+c2)
            double computedWidth = (maxHeightFromHeight - maxHeightFromWidth) / (slopeFromWidth - slopeFromHeight);
            // Compute height from width constraint line (y=m*x+c; using height would give same result)
            computedSize = new Size(
                computedWidth,
                (slopeFromWidth * computedWidth) + maxHeightFromWidth);
        }

        // Return result
        return computedSize;
    }

    /// <summary>
    /// Returns true if Size a is smaller than Size b in either dimension.
    /// </summary>
    /// <param name="a">Second Size.</param>
    /// <param name="b">First Size.</param>
    /// <returns>True if Size a is smaller than Size b in either dimension.</returns>
    private static bool IsSizeSmaller(Size a, Size b)
    {
        return (a.Width + AcceptableDelta < b.Width) || (a.Height + AcceptableDelta < b.Height);
    }

    internal virtual void NotifyMotionPreStart()
    {
        PreStart?.Invoke(this, EventArgs.Empty);
        _animating = true;
    }

    internal virtual void NotifyMotionCompleted()
    {
        _animating = false;
        InvalidateMeasure();
        Completed?.Invoke(this, EventArgs.Empty);
    }
}
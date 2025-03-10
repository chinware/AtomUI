using System.Diagnostics;
using System.Reflection;
using AtomUI.Reflection;
using Avalonia;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Input;

internal class TransformTrackingHelper : IDisposable
{
    #region 反射信息定义

    private static readonly Lazy<FieldInfo> AfterRenderFieldInfo = new Lazy<FieldInfo>(() =>
        typeof(DispatcherPriority).GetFieldInfoOrThrow("AfterRender",
            BindingFlags.Static | BindingFlags.NonPublic));

    #endregion
    
    private Visual? _visual;
    private bool _queuedForUpdate;
    private readonly EventHandler<AvaloniaPropertyChangedEventArgs> _propertyChangedHandler;
    private readonly List<Visual> _propertyChangedSubscriptions = new();

    public TransformTrackingHelper()
    {
        _propertyChangedHandler = PropertyChangedHandler;
    }

    public void SetVisual(Visual? visual)
    {
        Dispose();
        _visual = visual;
        if (visual != null)
        {
            visual.AttachedToVisualTree   += OnAttachedToVisualTree;
            visual.DetachedFromVisualTree -= OnDetachedFromVisualTree;
            if (visual.GetVisualRoot() is not null)
            {
                SubscribeToParents();
            }

            UpdateMatrix();
        }
    }

    public Matrix? Matrix { get; private set; }
    public event Action? MatrixChanged;

    public void Dispose()
    {
        if (_visual == null)
        {
            return;
        }

        UnsubscribeFromParents();
        _visual.AttachedToVisualTree   -= OnAttachedToVisualTree;
        _visual.DetachedFromVisualTree -= OnDetachedFromVisualTree;
        _visual                        =  null;
    }

    private void SubscribeToParents()
    {
        var visual = _visual;
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        // false positive
        while (visual != null)
        {
            if (visual is Visual v)
            {
                v.PropertyChanged += _propertyChangedHandler;
                _propertyChangedSubscriptions.Add(v);
            }

            visual = visual.GetVisualParent();
        }
    }

    private void UnsubscribeFromParents()
    {
        foreach (var v in _propertyChangedSubscriptions)
        {
            v.PropertyChanged -= _propertyChangedHandler;
        }

        _propertyChangedSubscriptions.Clear();
    }

    private void UpdateMatrix()
    {
        Matrix? matrix = null;
        if (_visual != null && _visual.GetVisualRoot() != null)
        {
            matrix = _visual.TransformToVisual((Visual)_visual.GetVisualRoot()!);
        }

        if (Matrix != matrix)
        {
            Matrix = matrix;
            MatrixChanged?.Invoke();
        }
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs visualTreeAttachmentEventArgs)
    {
        SubscribeToParents();
        UpdateMatrix();
    }

    private void EnqueueForUpdate()
    {
        if (_queuedForUpdate)
        {
            return;
        }

        _queuedForUpdate = true;
        var priority = AfterRenderFieldInfo.Value.GetValue(null) as DispatcherPriority?;
        Debug.Assert(priority != null);
        Dispatcher.UIThread.Post(UpdateMatrix, priority.Value);
    }

    private void PropertyChangedHandler(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        e.TryGetProperty<bool>("IsEffectiveValueChange", out var isEffectiveValueChange);
        if (isEffectiveValueChange && e.Property == Visual.BoundsProperty)
        {
            EnqueueForUpdate();
        }
    }

    private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs visualTreeAttachmentEventArgs)
    {
        UnsubscribeFromParents();
        UpdateMatrix();
    }

    public static IDisposable Track(Visual visual, Action<Visual, Matrix?> cb)
    {
        var rv = new TransformTrackingHelper();
        rv.MatrixChanged += () => cb(visual, rv.Matrix);
        rv.SetVisual(visual);
        return rv;
    }
}
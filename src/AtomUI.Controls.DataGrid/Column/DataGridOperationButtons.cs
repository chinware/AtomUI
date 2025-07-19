using System.Diagnostics;
using AtomUI.Controls.Data;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

internal class DataGridOperationButtons : TemplatedControl
{
    #region 公共接口

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<DataGridOperationButtons>();
    
    public static readonly StyledProperty<bool> IsEditEnabledProperty =
        AvaloniaProperty.Register<DataGridOperationButtons, bool>(nameof(IsEditEnabled));
    
    public static readonly StyledProperty<bool> IsDeleteEnabledProperty =
        AvaloniaProperty.Register<DataGridOperationButtons, bool>(nameof(IsDeleteEnabled));
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public bool IsEditEnabled
    {
        get => GetValue(IsEditEnabledProperty);
        set => SetValue(IsEditEnabledProperty, value);
    }
    
    public bool IsDeleteEnabled
    {
        get => GetValue(IsDeleteEnabledProperty);
        set => SetValue(IsDeleteEnabledProperty, value);
    }
    #endregion
    
    internal static readonly DirectProperty<DataGridOperationButtons, bool> IsEditingProperty =
        AvaloniaProperty.RegisterDirect<DataGridOperationButtons, bool>(
            nameof(IsEditing),
            o => o.IsEditing,
            (o, v) => o.IsEditing = v);
    
    internal bool IsEditing
    {
        get => _isEditing;
        set => SetAndRaise(IsEditingProperty, ref _isEditing, value);
    }

    private bool _isEditing;

    private HyperLinkTextBlock? _editAction;
    private PopupConfirm? _deleteAction;
    private HyperLinkTextBlock? _saveAction;
    private PopupConfirm? _cancelAction;
    private IDisposable? _disposable;
    
    internal DataGrid? OwningGrid { get; set; }
    internal DataGridRow? OwningRow { get; set; }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _editAction   = e.NameScope.Find<HyperLinkTextBlock>(DataGridOperationButtonsThemeConstants.EditActionPart);
        _deleteAction = e.NameScope.Find<PopupConfirm>(DataGridOperationButtonsThemeConstants.DeleteActionPart);
        _saveAction   = e.NameScope.Find<HyperLinkTextBlock>(DataGridOperationButtonsThemeConstants.SaveActionPart);
        _cancelAction = e.NameScope.Find<PopupConfirm>(DataGridOperationButtonsThemeConstants.CancelActionPart);

        if (_editAction != null)
        {
            _editAction.Click += HandleEditingRow;
        }

        if (_deleteAction != null)
        {
            _deleteAction.Confirmed += HandleDeleteRow;
        }

        if (_cancelAction != null)
        {
            _cancelAction.Confirmed += HandleCancelEditingRow;
        }

        if (_saveAction != null)
        {
            _saveAction.Click += HandleSaveRow;
        }
    }

    private void HandleDeleteRow(object? sender, RoutedEventArgs e)
    {
        if (OwningGrid != null)
        {
            var dataCollectionView =  OwningGrid.DataConnection.CollectionView;
            Debug.Assert(dataCollectionView != null);
            var index              = OwningRow?.Index;
            if (index.HasValue)
            {
                dataCollectionView.RemoveAt(index.Value);
            }
        }
    }

    private void HandleEditingRow(object? sender, RoutedEventArgs e)
    {
        if (OwningRow != null && OwningGrid != null)
        {
            SetValue(IsEditingProperty, true, BindingPriority.Template);
            OwningGrid?.BeginRowEdit(OwningRow);
        }
    }
    
    private void HandleSaveRow(object? sender, RoutedEventArgs e)
    {
        if (OwningRow != null && OwningGrid != null)
        {
            SetValue(IsEditingProperty, false, BindingPriority.Template);
            OwningGrid?.EndRowEdit(DataGridEditAction.Commit, true, true);
        }
    }
    
    
    private void HandleCancelEditingRow(object? sender, RoutedEventArgs e)
    {
        if (OwningRow != null && OwningGrid != null)
        {
            SetValue(IsEditingProperty, false, BindingPriority.Template);
            OwningGrid?.EndRowEdit(DataGridEditAction.Cancel, true, true);
        }
    }

    internal void NotifyLoadingRow(DataGridRow row)
    {
        OwningRow                                         = row;
        _disposable = BindUtils.RelayBind(row, DataGridRow.IsEditingModeProperty, this, IsEditingProperty);
    }
    
    internal void NotifyUnLoadingRow(DataGridRow row)
    {
        _disposable?.Dispose();
        OwningRow = null;
    }
}
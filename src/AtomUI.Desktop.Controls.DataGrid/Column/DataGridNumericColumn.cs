using System.Globalization;
using System.Reactive.Disposables;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUI.Controls;

public class DataGridNumericColumn : DataGridAbstractTextColumn
{
    #region 公共属性定义

    public static readonly StyledProperty<NumberFormatInfo?> NumberFormatProperty = 
        NumericUpDown.NumberFormatProperty.AddOwner<DataGridNumericColumn>();
    
    public static readonly StyledProperty<string> FormatStringProperty = 
        NumericUpDown.FormatStringProperty.AddOwner<DataGridNumericColumn>();
    
    public static readonly StyledProperty<Decimal> MaximumProperty =
        NumericUpDown.MaximumProperty.AddOwner<DataGridNumericColumn>();
    
    public static readonly StyledProperty<Decimal> MinimumProperty =
        NumericUpDown.MinimumProperty.AddOwner<DataGridNumericColumn>();
    
    public NumberFormatInfo? NumberFormat
    {
        get => GetValue(NumberFormatProperty);
        set => SetValue(NumberFormatProperty, value);
    }
    
    public string FormatString
    {
        get => GetValue(FormatStringProperty);
        set => SetValue(FormatStringProperty, value);
    }

    public Decimal Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public Decimal Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }
    
    #endregion
    
    public DataGridNumericColumn()
    {
        BindingTarget = NumericUpDown.ValueProperty;
    }

    protected override void CancelCellEdit(Control editingElement, object uneditedValue)
    {
        if (editingElement is NumericUpDown numericUpDown)
        {
            string? uneditedString = uneditedValue as string;
            numericUpDown.Text = uneditedString ?? string.Empty;
        }
    }

    protected override Control GenerateEditingElementDirect(DataGridCell cell, object dataItem)
    {
        var numericUpDown = new NumericUpDown
        {                
            Name  = "CellNumberUpDown",
        };
        
        BindUtils.RelayBind(this, NumberFormatProperty, numericUpDown, NumericUpDown.NumberFormatProperty);
        BindUtils.RelayBind(this, FormatStringProperty, numericUpDown, NumericUpDown.FormatStringProperty);
        BindUtils.RelayBind(this, MaximumProperty, numericUpDown, NumericUpDown.MaximumProperty);
        BindUtils.RelayBind(this, MinimumProperty, numericUpDown, NumericUpDown.MinimumProperty);
        SyncProperties(numericUpDown);
        return numericUpDown;
    }

    protected override object PrepareCellForEdit(Control editingElement, RoutedEventArgs editingEventArgs)
    {
        if (editingElement is NumericUpDown numericUpDown)
        {
            string uneditedText = numericUpDown.Text ?? string.Empty;
            return uneditedText;
        }
        return string.Empty;
    }
}
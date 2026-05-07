// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using AtomUI.Desktop.Controls.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

public abstract class DataGridBoundColumn : DataGridColumn
{
    private BindingBase? _binding;

    /// <summary>
    /// Gets or sets the binding that associates the column with a property in the data source.
    /// </summary>
    //TODO Binding
    [AssignBinding]
    [InheritDataTypeFromItems(nameof(DataGrid.ItemsSource), AncestorType = typeof(DataGrid))]
    public virtual BindingBase? Binding
    {
        get => _binding;
        set
        {
            if (_binding != value)
            {
                if (OwningGrid != null && !OwningGrid.CommitEdit(DataGridEditingUnit.Row, exitEditingMode: true))
                {
                    // Edited value couldn't be committed, so we force a CancelEdit
                    OwningGrid.CancelEdit(DataGridEditingUnit.Row, raiseEvents: false);
                }

                _binding = value; 

                if (_binding != null)
                {
                    if(_binding is { } binding)
                    {
                        if (GetBindingMode(binding) == BindingMode.OneWayToSource)
                        {
                            throw new InvalidOperationException("DataGridColumn doesn't support BindingMode.OneWayToSource. Use BindingMode.TwoWay instead.");
                        }

                        var path = GetBindingPath(binding);
                        if (!string.IsNullOrEmpty(path) && GetBindingMode(binding) == BindingMode.Default)
                        {
                            SetBindingMode(binding, BindingMode.TwoWay);
                        } 

                        if (GetBindingConverter(binding) == null && string.IsNullOrEmpty(GetBindingStringFormat(binding)))
                        {
                            SetBindingConverter(binding, DataGridValueConverter.Instance);
                        }
                    }

                    // Apply the new Binding to existing rows in the DataGrid
                    if (OwningGrid != null)
                    {
                        OwningGrid.HandleColumnBindingChanged(this);
                    }
                } 

                RemoveEditingElement();
            }
        }
    } 

    /// <summary>
    /// The binding that will be used to get or set cell content for the clipboard.
    /// If the base ClipboardContentBinding is not explicitly set, this will return the value of Binding.
    /// </summary>
    public override BindingBase? ClipboardContentBinding
    {
        get => base.ClipboardContentBinding ?? Binding;
        set => base.ClipboardContentBinding = value;
    }

    //TODO Rename
    //TODO Validation
    protected sealed override Control GenerateEditingElement(DataGridCell cell, object dataItem, out BindingExpressionBase? editBinding)
    {
        Control element = GenerateEditingElementDirect(cell, dataItem);
        editBinding = null;

        if (Binding != null)
        {
            Debug.Assert(BindingTarget != null);
            editBinding = element.Bind(BindingTarget, Binding);
        }

        return element;
    } 

    protected abstract Control GenerateEditingElementDirect(DataGridCell cell, object dataItem);

    protected AvaloniaProperty? BindingTarget { get; set; }

    internal void SetHeaderFromBinding()
    {
        if (OwningGrid != null && OwningGrid.DataConnection.DataType != null
                               && Header == null && Binding is { } binding)
        {
            var path = GetBindingPath(binding);
            if (!string.IsNullOrWhiteSpace(path))
            {
                var header = OwningGrid.DataConnection.DataType.GetDisplayName(path);
                if (header != null)
                {
                    Header = header;
                }
            }
        }
    }

    private static BindingMode GetBindingMode(BindingBase binding)
    {
        return binding switch
        {
            ReflectionBinding reflectionBinding => reflectionBinding.Mode,
            CompiledBinding compiledBinding => compiledBinding.Mode,
            _ => BindingMode.Default
        };
    }

    private static void SetBindingMode(BindingBase binding, BindingMode mode)
    {
        switch (binding)
        {
            case ReflectionBinding reflectionBinding:
                reflectionBinding.Mode = mode;
                break;
            case CompiledBinding compiledBinding:
                compiledBinding.Mode = mode;
                break;
        }
    }

    private static string? GetBindingPath(BindingBase binding)
    {
        return binding switch
        {
            ReflectionBinding reflectionBinding => reflectionBinding.Path,
            CompiledBinding compiledBinding => compiledBinding.Path?.ToString(),
            _ => null
        };
    }

    private static object? GetBindingConverter(BindingBase binding)
    {
        return binding switch
        {
            ReflectionBinding reflectionBinding => reflectionBinding.Converter,
            CompiledBinding compiledBinding => compiledBinding.Converter,
            _ => null
        };
    }

    private static void SetBindingConverter(BindingBase binding, DataGridValueConverter converter)
    {
        switch (binding)
        {
            case ReflectionBinding reflectionBinding:
                reflectionBinding.Converter = converter;
                break;
            case CompiledBinding compiledBinding:
                compiledBinding.Converter = converter;
                break;
        }
    }

    private static string? GetBindingStringFormat(BindingBase binding)
    {
        return binding switch
        {
            ReflectionBinding reflectionBinding => reflectionBinding.StringFormat,
            CompiledBinding compiledBinding => compiledBinding.StringFormat,
            _ => null
        };
    }
}

using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using AvaloniaControlList = Avalonia.Controls.Controls;

namespace AtomUIGallery.Controls;

public class ShowCasePanel : TemplatedControl
{
    internal const string MainPanelPart = "PART_MainPanel";

    private bool _initialized;
    private Grid? _layoutPanel;

    [Content]
    public AvaloniaControlList Children { get; } = new();

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        var effectCount = 0;
        foreach (var child in Children)
        {
            if (child is ShowCaseItem showCaseItem)
            {
                effectCount++;
                if (showCaseItem.IsOccupyEntireRow)
                {
                    effectCount++;
                }
            }
        }
        if (effectCount % 2 != 0)
        {
            var extra = new ShowCaseItem()
            {
                IsFake = true
            };
            Children.Add(extra);
        }
        base.OnApplyTemplate(e);
        _layoutPanel = e.NameScope.Get<Grid>(MainPanelPart);
        if (_layoutPanel != null && !_initialized)
        {
            var row = 0;
            var column = 0;
            
            for (var i = 0; i < Children.Count; ++i)
            {
                if (Children[i] is ShowCaseItem item)
                {
                    if (item.IsOccupyEntireRow)
                    {
                        if (column != 0)
                        {
                            row++;
                        }
                        Grid.SetRow(item, row++);
                        
                        Grid.SetColumn(item, 0);
                        Grid.SetColumnSpan(item, 2);
                    }
                    else
                    {
                        Grid.SetRow(item, row);
                        Grid.SetColumn(item, column++);
                        if (column == 2)
                        {
                            row++;
                            column = 0;
                        }
                    }
                    _layoutPanel.Children.Add(item);
                    LogicalChildren.Add(item);
                }
            }
            
            var rowDefinitions = new RowDefinitions();
            for (var i = 0; i < row; ++i)
            {
                rowDefinitions.Add(new RowDefinition(GridLength.Auto));
            }
            _layoutPanel.RowDefinitions = rowDefinitions;
            _initialized                = true;
        }
    }

    internal virtual void NotifyAboutToActive()
    {
    }

    internal virtual void NotifyActivated()
    {
    }

    internal virtual void NotifyAboutToDeactivated()
    {
    }

    internal virtual void NotifyDeactivated()
    {
    }
}
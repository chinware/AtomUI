using AtomUI.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

public interface IAddOnDecoratedBox
{
    [DependsOn(nameof(LeftAddOnTemplate))]
    public object? LeftAddOn { get; set; }
    
    public IDataTemplate? LeftAddOnTemplate { get; set; }

    [DependsOn(nameof(RightAddOnTemplate))]
    public object? RightAddOn{ get; set; }
    
    public IDataTemplate? RightAddOnTemplate { get; set; }
    
    [DependsOn(nameof(ContentLeftAddOnTemplate))]
    public object? ContentLeftAddOn{ get; set; }
    
    public IDataTemplate? ContentLeftAddOnTemplate { get; set; }

    [DependsOn(nameof(ContentRightAddOnTemplate))]
    public object? ContentRightAddOn{ get; set; }
    
    public IDataTemplate? ContentRightAddOnTemplate { get; set; }

    public SizeType SizeType { get; set; }

    public InputControlStyleVariant StyleVariant { get; set; }

    public InputControlStatus Status { get; set; }
    
    [DependsOn(nameof(ExtraTemplate))]
    public object? Extra { get; set; }
    
    public IDataTemplate? ExtraTemplate { get; set; }
}
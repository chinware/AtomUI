namespace AtomUI.Theme.TokenSystem;

[GlobalDesignToken]
public partial class DesignToken
{
    /// <summary>
    /// 更小的组件高度
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double ControlHeightXS { get; set; }

    /// <summary>
    /// 较小的组件高度
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double ControlHeightSM { get; set; }

    /// <summary>
    /// 较高的组件高度
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public double ControlHeightLG { get; set; }
}
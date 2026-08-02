namespace AtomUI.Theme.DesignTokens;

/// <summary>
/// 所有的控件 Token 定义是除了全局 Token 之外专属于当前控件的 Token 值。
/// </summary>
public abstract class AbstractControlDesignToken : AbstractDesignToken
{
    protected DesignToken EffectiveGlobalToken { get; private set; } = default!;

    internal void AssignEffectiveGlobalToken(DesignToken effectiveGlobalToken)
    {
        EffectiveGlobalToken = effectiveGlobalToken;
    }

    public virtual void CalculateTokenValues(bool isDarkMode)
    {
    }

}

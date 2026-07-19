namespace AtomUI.Theme.DesignTokens;

/// <summary>
/// 所有的组件 Token 定义是除了全局的 Token 的之外的专属于当前的组件的 Token 值
/// </summary>
public abstract class AbstractControlDesignToken : AbstractDesignToken
{
    public string Id => _id;
    protected DesignToken SharedToken;

    private readonly string _id;

    protected AbstractControlDesignToken(string id)
    {
        _id         = id;
        SharedToken = default!;
    }

    public void AssignSharedToken(DesignToken sharedToken)
    {
        SharedToken = sharedToken;
    }

    public virtual void CalculateTokenValues(bool isDarkMode)
    {
    }

}

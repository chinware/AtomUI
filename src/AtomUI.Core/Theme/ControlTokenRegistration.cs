using System.Diagnostics.CodeAnalysis;
using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme;

public readonly struct ControlTokenRegistration
{
    public ControlTokenRegistration(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                                    DynamicallyAccessedMemberTypes.PublicProperties |
                                    DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type tokenType)
    {
        TokenType = tokenType;
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor |
                                DynamicallyAccessedMemberTypes.PublicProperties |
                                DynamicallyAccessedMemberTypes.NonPublicProperties)]
    public Type TokenType { get; }

    internal AbstractControlDesignToken? Activate()
    {
        return Activator.CreateInstance(TokenType) as AbstractControlDesignToken;
    }
}

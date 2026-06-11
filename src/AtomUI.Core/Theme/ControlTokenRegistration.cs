using System.Diagnostics.CodeAnalysis;

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
}

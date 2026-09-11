namespace AtomUI.Registration;

/// <summary>
/// Marks a public <see cref="IAtomUIBuilder"/> extension method as a Control Package
/// registration entry for compile-time linked registration metadata generation.
/// </summary>
/// <remarks>
/// AtomUI reads this declaration only during compilation. Runtime registration still occurs
/// when the application explicitly calls the marked extension method.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class ControlPackageRegistrationEntryAttribute : Attribute
{
}

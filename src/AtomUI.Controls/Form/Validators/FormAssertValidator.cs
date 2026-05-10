namespace AtomUI.Controls;

public class FormAssertValidator : AbstractFormValidator
{
    public bool AssertResult { get; set; }
    public TimeSpan Delay { get; set; } = TimeSpan.Zero;

    protected override async Task<bool> ValidateCoreAsync(string fieldName, object? value, CancellationToken cancellationToken)
    {
        if (Delay != TimeSpan.Zero)
        {
            await Task.Delay(Delay, cancellationToken);
        }
        return AssertResult;
    }
}
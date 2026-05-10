namespace AtomUI.Controls;

public class FormNotNullValidator : AbstractFormValidator
{
    protected override async Task<bool> ValidateCoreAsync(string fieldName, object? value, CancellationToken cancellationToken)
    {
        return await Task.FromResult(value != null);
    }
}
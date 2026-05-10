namespace AtomUI.Controls;

public class FormStringNotEmptyValidator : AbstractFormValidator
{
    protected override async Task<bool> ValidateCoreAsync(string fieldName, object? value, CancellationToken cancellationToken)
    {
        var strValue = value as string;
        return await Task.FromResult(!string.IsNullOrWhiteSpace(strValue));
    }
}
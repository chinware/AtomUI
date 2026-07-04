using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal sealed class FormDataValidationError : Exception
{
    public FormDataValidationError(string message)
        : base(message)
    {
    }
}

internal static class FormDataValidationErrors
{
    public static void SetFormErrors(Control control, IEnumerable<string>? messages)
    {
        var errors = GetNonFormErrors(control);
        if (messages is not null)
        {
            foreach (var message in messages)
            {
                if (!string.IsNullOrWhiteSpace(message))
                {
                    errors.Add(new FormDataValidationError(message));
                }
            }
        }

        SetErrors(control, errors);
    }

    public static void ClearFormErrors(Control control)
    {
        SetErrors(control, GetNonFormErrors(control));
    }

    public static IList<string> GetErrorMessages(Control control)
    {
        var errors = DataValidationErrors.GetErrors(control);
        if (errors is null)
        {
            return [];
        }

        var messages = new List<string>();
        foreach (var error in errors)
        {
            var message = error switch
            {
                Exception exception => exception.Message,
                _                   => error?.ToString()
            };
            if (!string.IsNullOrWhiteSpace(message))
            {
                messages.Add(message);
            }
        }
        return messages;
    }

    private static List<object> GetNonFormErrors(Control control)
    {
        var errors = DataValidationErrors.GetErrors(control);
        if (errors is null)
        {
            return [];
        }

        var nonFormErrors = new List<object>();
        foreach (var error in errors)
        {
            if (error is not FormDataValidationError)
            {
                nonFormErrors.Add(error);
            }
        }
        return nonFormErrors;
    }

    private static void SetErrors(Control control, List<object> errors)
    {
        if (errors.Count == 0)
        {
            DataValidationErrors.ClearErrors(control);
        }
        else
        {
            DataValidationErrors.SetErrors(control, errors);
        }
    }
}

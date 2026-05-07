// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using System.Reactive.Subjects;
using AtomUI.Desktop.Controls.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace AtomUI.Desktop.Controls;

internal class CellEditBinding : ICellEditBinding
{
    private readonly BindingExpressionBase _bindingExpression;
    private readonly Control? _validationTarget;
    private readonly Subject<bool> _changedSubject = new();
    private readonly List<Exception> _validationErrors = new ();
    private readonly IDisposable? _validationSubscription;
    
    public bool IsValid => _validationErrors.Count <= 0;
    public IEnumerable<Exception> ValidationErrors => _validationErrors;
    public IObservable<bool> ValidationChanged => _changedSubject;

    public CellEditBinding(BindingExpressionBase bindingExpression, Control? validationTarget = null)
    {
        _bindingExpression = bindingExpression;
        _validationTarget  = validationTarget;
        _validationSubscription = validationTarget?.GetObservable(DataValidationErrors.ErrorsProperty)
            .Subscribe(UpdateValidationErrors);
        UpdateValidationErrors(validationTarget != null ? DataValidationErrors.GetErrors(validationTarget) : null);
    }

    public bool CommitEdit()
    {
        _bindingExpression.UpdateSource();
        UpdateValidationErrors(_validationTarget != null ? DataValidationErrors.GetErrors(_validationTarget) : null);
        return IsValid;
    }

    private void UpdateValidationErrors(IEnumerable<object>? errors)
    {
        var wasValid = IsValid;
        _validationErrors.Clear();

        if (errors != null)
        {
            foreach (var error in errors)
            {
                if (error is Exception exception)
                {
                    _validationErrors.AddRange(ValidationUtils.UnpackException(exception));
                }
                else
                {
                    _validationErrors.Add(new DataValidationException(error));
                }
            }
        }

        var isValid = IsValid;
        if (isValid != wasValid)
        {
            _changedSubject.OnNext(isValid);
        }
    }

    public void Dispose()
    {
        _validationSubscription?.Dispose();
        _bindingExpression.Dispose();
        _changedSubject.Dispose();
    }
}

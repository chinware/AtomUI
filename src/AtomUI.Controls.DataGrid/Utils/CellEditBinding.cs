using AtomUI.Reactive;
using Avalonia.Data;

namespace AtomUI.Controls.Utils;

internal class CellEditBinding : ICellEditBinding
{
    private readonly LightweightSubject<bool> _changedSubject = new();
    private readonly List<Exception> _validationErrors = new();
    private readonly SubjectWrapper _inner;
    
    public bool IsValid => _validationErrors.Count <= 0;
    public IEnumerable<Exception> ValidationErrors => _validationErrors;
    public IObservable<bool> ValidationChanged => _changedSubject;
    public IAtomUISubject<object?> InternalSubject => _inner;

    public CellEditBinding(IAtomUISubject<object?> bindingSourceSubject)
    {
        _inner = new SubjectWrapper(bindingSourceSubject, this);
    }
    
    private void AlterValidationErrors(Action<List<Exception>> action)
    {
        var wasValid = IsValid;
        action(_validationErrors);
        var isValid = IsValid;

        if (!isValid || !wasValid)
        {
            _changedSubject.OnNext(isValid);
        }
    }

    public bool CommitEdit()
    {
        _inner.CommitEdit();
        return IsValid;
    }

    class SubjectWrapper : LightweightObservableBase<object?>, IAtomUISubject<object?>, IDisposable
    {
        private readonly IAtomUISubject<object?> _sourceSubject;
        private readonly CellEditBinding _editBinding;
        private IDisposable? _subscription;
        private object? _controlValue;
        private bool _isControlValueSet = false;
        private bool _settingSourceValue = false;

        public SubjectWrapper(IAtomUISubject<object?> bindingSourceSubject, CellEditBinding editBinding)
        {
            _sourceSubject = bindingSourceSubject;
            _editBinding   = editBinding;
        }

        private void SetSourceValue(object? value)
        {
            if (!_settingSourceValue)
            {
                _settingSourceValue = true;
                _sourceSubject.OnNext(value);
                _settingSourceValue = false;
            }
        }

        private void SetControlValue(object? value)
        {
            PublishNext(value);
        }

        private void OnValidationError(BindingNotification notification)
        {
            if (notification.Error != null)
            {
                _editBinding.AlterValidationErrors(errors =>
                {
                    errors.Clear();
                    var unpackedErrors = ValidationUtil.UnpackException(notification.Error);
                    errors.AddRange(unpackedErrors);
                });
            }
        }

        private void OnControlValueUpdated(object? value)
        {
            _controlValue      = value;
            _isControlValueSet = true;

            if (!_editBinding.IsValid)
            {
                SetSourceValue(value);
            }
        }

        private void OnSourceValueUpdated(object? value)
        {
            void OnValidValue(object? val)
            {
                SetControlValue(val);
                _editBinding.AlterValidationErrors(errors => errors.Clear());
            }

            if (value is BindingNotification notification)
            {
                if (notification.ErrorType != BindingErrorType.None)
                    OnValidationError(notification);
                else
                    OnValidValue(value);
            }
            else
            {
                OnValidValue(value);
            }
        }

        protected override void Deinitialize()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        protected override void Initialize()
        {
            _subscription = _sourceSubject.Subscribe(OnSourceValueUpdated);
        }

        void IObserver<object?>.OnCompleted()
        {
            throw new NotImplementedException();
        }

        void IObserver<object?>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<object?>.OnNext(object? value)
        {
            OnControlValueUpdated(value);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
            _subscription = null;
        }

        public void CommitEdit()
        {
            if (_isControlValueSet)
            {
                SetSourceValue(_controlValue);
            }
        }
    }
}
namespace AtomUI.Exceptions;

public class InvalidPropertyValueException : ArgumentException
{
    public string PropertyName { get; }
    public object InvalidValue { get; }
    
    public InvalidPropertyValueException(string propertyName, object invalidValue)
        : base($"The value '{invalidValue}' is not valid for property '{propertyName}'.")
    {
        PropertyName = propertyName;
        InvalidValue = invalidValue;
    }
    
    public InvalidPropertyValueException(string propertyName, object invalidValue, string message)
        : base(message)
    {
        PropertyName = propertyName;
        InvalidValue = invalidValue;
    }
    
    public InvalidPropertyValueException(string propertyName, object invalidValue, string message, Exception innerException)
        : base(message, innerException)
    {
        PropertyName = propertyName;
        InvalidValue = invalidValue;
    }
}
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.ComponentModel;
using System.Globalization;
using AtomUI.Utils;
using Avalonia.Utilities;

namespace AtomUI.Controls;

public enum DataGridLengthUnitType
{
    Auto = 0,
    Pixel = 1,
    SizeToCells = 2,
    SizeToHeader = 3,
    Star = 4
}

public struct DataGridLength : IEquatable<DataGridLength>
{
    // WPF uses 1.0 as the default value as well
    private const double DEFAULT_VALUE = 1.0;
    
    private readonly double _desiredValue;   //  desired value storage
    private readonly double _displayValue;   //  display value storage
    private readonly double _unitValue;      //  unit value storage
    private readonly DataGridLengthUnitType _unitType; //  unit type storage

    #region 公共属性定义

    /// <summary>
    /// Gets a <see cref="T:AtomUI.Controls.DataGridLength" /> structure that represents the standard automatic sizing mode.
    /// </summary>
    /// <returns>
    /// A <see cref="T:AtomUI.Controls.DataGridLength" /> structure that represents the standard automatic sizing mode.
    /// </returns>
    public static DataGridLength Auto { get; } = new DataGridLength(DEFAULT_VALUE, DataGridLengthUnitType.Auto);

    /// <summary>
    ///     Returns the desired value of this instance.
    /// </summary>
    public double DesiredValue => _desiredValue;

    /// <summary>
    ///     Returns the display value of this instance.
    /// </summary>
    public double DisplayValue => _displayValue;

    /// <summary>
    ///     Returns <c>true</c> if this DataGridLength instance holds 
    ///     an absolute (pixel) value.
    /// </summary>
    public bool IsAbsolute => _unitType == DataGridLengthUnitType.Pixel;

    /// <summary>
    ///     Returns <c>true</c> if this DataGridLength instance is 
    ///     automatic (not specified).
    /// </summary>
    public bool IsAuto => _unitType == DataGridLengthUnitType.Auto;

    /// <summary>
    ///     Returns <c>true</c> if this instance is to size to the cells of a column or row.
    /// </summary>
    public bool IsSizeToCells => _unitType == DataGridLengthUnitType.SizeToCells;

    /// <summary>
    ///     Returns <c>true</c> if this instance is to size to the header of a column or row.
    /// </summary>
    public bool IsSizeToHeader => _unitType == DataGridLengthUnitType.SizeToHeader;

    /// <summary>
    ///     Returns <c>true</c> if this DataGridLength instance holds a weighted proportion
    ///     of available space.
    /// </summary>
    public bool IsStar => _unitType == DataGridLengthUnitType.Star;

    /// <summary>
    /// Gets a <see cref="T:AtomUI.Controls.DataGridLength" /> structure that represents the cell-based automatic sizing mode.
    /// </summary>
    /// <returns>
    /// A <see cref="T:AtomUI.Controls.DataGridLength" /> structure that represents the cell-based automatic sizing mode.
    /// </returns>
    public static DataGridLength SizeToCells { get; } =
        new DataGridLength(DEFAULT_VALUE, DataGridLengthUnitType.SizeToCells);

    /// <summary>
    /// Gets a <see cref="T:AtomUI.Controls.DataGridLength" /> structure that represents the header-based automatic sizing mode.
    /// </summary>
    /// <returns>
    /// A <see cref="T:AtomUI.Controls.DataGridLength" /> structure that represents the header-based automatic sizing mode.
    /// </returns>
    public static DataGridLength SizeToHeader { get; } =
        new DataGridLength(DEFAULT_VALUE, DataGridLengthUnitType.SizeToHeader);

    /// <summary>
    /// Gets the <see cref="T:AtomUI.Controls.DataGridLengthUnitType" /> that represents the current sizing mode.
    /// </summary>
    public DataGridLengthUnitType UnitType => _unitType;

    /// <summary>
    /// Gets the absolute value of the <see cref="T:AtomUI.Controls.DataGridLength" /> in pixels.
    /// </summary>
    /// <returns>
    /// The absolute value of the <see cref="T:AtomUI.Controls.DataGridLength" /> in pixels.
    /// </returns>
    public double Value => _unitValue;

    #endregion
    
    
    /// <summary>
    /// Initializes a new instance of the <see cref="T:AtomUI.Controls.DataGridLength" /> class. 
    /// </summary>
    /// <param name="value"></param>
    public DataGridLength(double value)
        : this(value, DataGridLengthUnitType.Pixel)
    {
    }
    
    /// <summary>
    ///     Initializes to a specified value and unit.
    /// </summary>
    /// <param name="value">The value to hold.</param>
    /// <param name="type">The unit of <c>value</c>.</param>
    /// <remarks> 
    ///     <c>value</c> is ignored unless <c>type</c> is
    ///     <c>DataGridLengthUnitType.Pixel</c> or
    ///     <c>DataGridLengthUnitType.Star</c>
    /// </remarks>
    /// <exception cref="ArgumentException">
    ///     If <c>value</c> parameter is <c>double.NaN</c>
    ///     or <c>value</c> parameter is <c>double.NegativeInfinity</c>
    ///     or <c>value</c> parameter is <c>double.PositiveInfinity</c>.
    /// </exception>
    public DataGridLength(double value, DataGridLengthUnitType type)
        : this(value, type, (type == DataGridLengthUnitType.Pixel ? value : Double.NaN), (type == DataGridLengthUnitType.Pixel ? value : Double.NaN))
    {
    }
    
    /// <summary>
    ///     Initializes to a specified value and unit.
    /// </summary>
    /// <param name="value">The value to hold.</param>
    /// <param name="type">The unit of <c>value</c>.</param>
    /// <param name="desiredValue"></param>
    /// <param name="displayValue"></param>
    /// <remarks> 
    ///     <c>value</c> is ignored unless <c>type</c> is
    ///     <c>DataGridLengthUnitType.Pixel</c> or
    ///     <c>DataGridLengthUnitType.Star</c>
    /// </remarks>
    /// <exception cref="ArgumentException">
    ///     If <c>value</c> parameter is <c>double.NaN</c>
    ///     or <c>value</c> parameter is <c>double.NegativeInfinity</c>
    ///     or <c>value</c> parameter is <c>double.PositiveInfinity</c>.
    /// </exception>
    public DataGridLength(double value, DataGridLengthUnitType type, double desiredValue, double displayValue)
    {
        if (double.IsNaN(value))
        {
            throw DataGridError.DataGrid.ValueCannotBeSetToNAN("value");
        }
        if (double.IsInfinity(value))
        {
            throw DataGridError.DataGrid.ValueCannotBeSetToInfinity("value");
        }
        if (double.IsInfinity(desiredValue))
        {
            throw DataGridError.DataGrid.ValueCannotBeSetToInfinity("desiredValue");
        }
        if (double.IsInfinity(displayValue))
        {
            throw DataGridError.DataGrid.ValueCannotBeSetToInfinity("displayValue");
        }
        if (value < 0)
        {
            throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("value", "value", 0);
        }
        if (desiredValue < 0)
        {
            throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("desiredValue", "desiredValue", 0);
        }
        if (displayValue < 0)
        {
            throw DataGridError.DataGrid.ValueMustBeGreaterThanOrEqualTo("displayValue", "displayValue", 0);
        }

        if (type != DataGridLengthUnitType.Auto &&
            type != DataGridLengthUnitType.SizeToCells &&
            type != DataGridLengthUnitType.SizeToHeader &&
            type != DataGridLengthUnitType.Star &&
            type != DataGridLengthUnitType.Pixel)
        {
            throw DataGridError.DataGridLength.InvalidUnitType("type");
        }

        _desiredValue = desiredValue;
        _displayValue = displayValue;
        _unitValue    = (type == DataGridLengthUnitType.Auto) ? DEFAULT_VALUE : value;
        _unitType     = type;
    }

    /// <summary>
    /// Overloaded operator, compares 2 DataGridLength's.
    /// </summary>
    /// <param name="gl1">first DataGridLength to compare.</param>
    /// <param name="gl2">second DataGridLength to compare.</param>
    /// <returns>true if specified DataGridLength have same value, 
    /// unit type, desired value, and display value.</returns>
    public static bool operator ==(DataGridLength gl1, DataGridLength gl2)
    {
        return (gl1.UnitType == gl2.UnitType
                && MathUtils.AreClose(gl1.Value, gl2.Value)
                && MathUtils.AreClose(gl1.DesiredValue, gl2.DesiredValue)
                && MathUtils.AreClose(gl1.DisplayValue, gl2.DisplayValue));
    }

    /// <summary>
    /// Overloaded operator, compares 2 DataGridLength's.
    /// </summary>
    /// <param name="gl1">first DataGridLength to compare.</param>
    /// <param name="gl2">second DataGridLength to compare.</param>
    /// <returns>true if specified DataGridLength have either different value, 
    /// unit type, desired value, or display value.</returns>
    public static bool operator !=(DataGridLength gl1, DataGridLength gl2)
    {
        return (gl1.UnitType != gl2.UnitType
                || !MathUtils.AreClose(gl1.Value, gl2.Value)
                || !MathUtils.AreClose(gl1.DesiredValue, gl2.DesiredValue)
                || !MathUtils.AreClose(gl1.DisplayValue, gl2.DisplayValue));
    }

    /// <summary>
    /// Compares this instance of DataGridLength with another instance.
    /// </summary>
    /// <param name="other">DataGridLength length instance to compare.</param>
    /// <returns><c>true</c> if this DataGridLength instance has the same value 
    /// and unit type as gridLength.</returns>
    public bool Equals(DataGridLength other)
    {
        return (this == other);
    }

    /// <summary>
    /// Compares this instance of DataGridLength with another object.
    /// </summary>
    /// <param name="obj">Reference to an object for comparison.</param>
    /// <returns><c>true</c> if this DataGridLength instance has the same value 
    /// and unit type as oCompare.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is DataGridLength dataGridLength)
        {
            return this == dataGridLength;
        }
        return false;
    }

    /// <summary>
    /// Returns a unique hash code for this DataGridLength
    /// </summary>
    /// <returns>hash code</returns>
    public override int GetHashCode()
    {
        return ((int)_unitValue + (int)_unitType) + (int)_desiredValue + (int)_displayValue;
    }

}

/// <summary>
/// DataGridLengthConverter - Converter class for converting instances of other types to and from DataGridLength instances.
/// </summary> 
public class DataGridLengthConverter : TypeConverter
{
    private static readonly string StarSuffix = "*";
    private static readonly string[] ValueInvariantUnitStrings = ["auto", "sizetocells", "sizetoheader"];
    private static readonly DataGridLength[] ValueInvariantDataGridLengths = [DataGridLength.Auto, DataGridLength.SizeToCells, DataGridLength.SizeToHeader];

    /// <summary>
    /// Checks whether or not this class can convert from a given type.
    /// </summary>
    /// <param name="context">
    /// An ITypeDescriptorContext that provides a format context. 
    /// </param>
    /// <param name="sourceType">The Type being queried for support.</param>
    /// <returns>
    /// <c>true</c> if this converter can convert from the provided type, 
    /// <c>false</c> otherwise.
    /// </returns>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        // We can only handle strings, integral and floating types
        TypeCode tc = Type.GetTypeCode(sourceType);
        switch (tc)
        {
            case TypeCode.String:
            case TypeCode.Decimal:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Checks whether or not this class can convert to a given type.
    /// </summary>
    /// <param name="context">
    /// An ITypeDescriptorContext that provides a format context. 
    /// </param>
    /// <param name="destinationType">The Type being queried for support.</param>
    /// <returns>
    /// <c>true</c> if this converter can convert to the provided type, 
    /// <c>false</c> otherwise.
    /// </returns>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        return destinationType == typeof(string);
    }

    /// <summary>
    /// Attempts to convert to a DataGridLength from the given object.
    /// </summary>
    /// <param name="context">
    /// An ITypeDescriptorContext that provides a format context. 
    /// </param>
    /// <param name="culture">
    /// The CultureInfo to use for the conversion. 
    /// </param>
    /// <param name="value">The object to convert to a GridLength.</param>
    /// <returns>
    /// The GridLength instance which was constructed.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// A NotSupportedException is thrown if the example object is null.
    /// </exception>
    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        // GridLengthConverter in WPF throws a NotSupportedException on a null value as well.
        if (value == null)
        {
            throw DataGridError.DataGridLengthConverter.CannotConvertFrom("(null)");
        }

        if (value is string stringValue)
        {
            stringValue = stringValue.Trim();

            if (stringValue.EndsWith(StarSuffix, StringComparison.Ordinal))
            {
                string stringValueWithoutSuffix = stringValue.Substring(0, stringValue.Length - StarSuffix.Length);

                double starWeight;
                if (string.IsNullOrEmpty(stringValueWithoutSuffix))
                {
                    starWeight = 1;
                }
                else
                {
                    starWeight = Convert.ToDouble(stringValueWithoutSuffix, culture ?? CultureInfo.CurrentCulture);
                }

                return new DataGridLength(starWeight, DataGridLengthUnitType.Star);
            }

            for (int index = 0; index < ValueInvariantUnitStrings.Length; index++)
            {
                if (stringValue.Equals(ValueInvariantUnitStrings[index], StringComparison.OrdinalIgnoreCase))
                {
                    return ValueInvariantDataGridLengths[index];
                }
            }
        }

        // Conversion from numeric type, WPF lets Convert exceptions bubble out here as well
        double doubleValue = Convert.ToDouble(value, culture ?? CultureInfo.CurrentCulture);
        if (double.IsNaN(doubleValue))
        {
            // WPF returns Auto in this case as well
            return DataGridLength.Auto;
        }
        else
        {
            return new DataGridLength(doubleValue);
        }
    }

    /// <summary>
    /// Attempts to convert a DataGridLength instance to the given type.
    /// </summary>
    /// <param name="context">
    /// An ITypeDescriptorContext that provides a format context. 
    /// </param>
    /// <param name="culture">
    /// The CultureInfo to use for the conversion. 
    /// </param>
    /// <param name="value">The DataGridLength to convert.</param>
    /// <param name="destinationType">The type to which to convert the DataGridLength instance.</param>
    /// <returns>
    /// The object which was constructed.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// An ArgumentNullException is thrown if the example object is null.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// A NotSupportedException is thrown if the object is not null and is not a DataGridLength,
    /// or if the destinationType isn't one of the valid destination types.
    /// </exception>
    ///<SecurityNote>
    ///     Critical: calls InstanceDescriptor ctor which LinkDemands
    ///     PublicOK: can only make an InstanceDescriptor for DataGridLength, not an arbitrary class
    ///</SecurityNote> 
    public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == null)
        {
            throw new ArgumentNullException(nameof(destinationType));
        }
        if (destinationType != typeof(string))
        {
            throw DataGridError.DataGridLengthConverter.CannotConvertTo(destinationType.ToString());
        }
        DataGridLength? dataGridLength = value as DataGridLength?;
        if (!dataGridLength.HasValue)
        {
            throw DataGridError.DataGridLengthConverter.InvalidDataGridLength("value");
        }
        else
        {
            // Convert dataGridLength to a string
            switch (dataGridLength.Value.UnitType)
            {
                //  for Auto print out "Auto". value is always "1.0"
                case DataGridLengthUnitType.Auto:
                    return "Auto";

                case DataGridLengthUnitType.SizeToHeader:
                    return "SizeToHeader";

                case DataGridLengthUnitType.SizeToCells:
                    return "SizeToCells";

                //  Star has one special case when value is "1.0".
                //  in this case drop value part and print only "Star"
                case DataGridLengthUnitType.Star:
                    return (
                        MathUtilities.AreClose(1.0, dataGridLength.Value.Value)
                            ? StarSuffix
                            : Convert.ToString(dataGridLength.Value.Value, culture ?? CultureInfo.CurrentCulture) + DataGridLengthConverter.StarSuffix);

                default:
                    return (Convert.ToString(dataGridLength.Value.Value, culture ?? CultureInfo.CurrentCulture));
            }
        }
    }
}
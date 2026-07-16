using System.ComponentModel;
using AtomUI.Utils;

namespace AtomUI.Theme;

[TypeConverter(typeof (ThemeAlgorithmTypeConverter))]
public enum ThemeAlgorithm
{
    Default,
    Dark,
    Compact
}
using System.ComponentModel;
using AtomUI.Theme.Utils;

namespace AtomUI.Theme;

[TypeConverter(typeof (ThemeAlgorithmTypeConverter))]
public enum ThemeAlgorithm
{
    Default,
    Dark,
    Compact
}
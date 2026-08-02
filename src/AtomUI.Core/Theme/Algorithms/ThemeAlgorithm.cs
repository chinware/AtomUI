using System.ComponentModel;
using AtomUI.Utils;

namespace AtomUI.Theme.Algorithms;

[TypeConverter(typeof (ThemeAlgorithmTypeConverter))]
public enum ThemeAlgorithm
{
    Default = 0,
    Dark = 1,
    Compact = 2
}

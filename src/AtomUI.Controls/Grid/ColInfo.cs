using System.ComponentModel;

namespace AtomUI.Controls;

[TypeConverter(typeof(GridColSizeConverter))]
public record ColInfo : GridColSize;

using Avalonia.Controls;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class Overview : UserControl
{
    public Overview()
    {
        InitializeComponent();
    }

    public string MainInstall { get; set; } = "dotnet add package AtomUI --version 11.0.7";

    public string MainStyle { get; set; } = """
<Application.Styles>
    <StyleInclude Source="avares://AtomUI/Themes/Index.axaml" />
</Application.Styles>
""";

    public string ColorPickerInstall { get; set; } = "dotnet add package AtomUI.ColorPicker --version 11.0.7";

    public string ColorPickerStyle { get; set; } = """
<Application.Styles>
    <StyleInclude Source="avares://AtomUI.ColorPicker/Index.axaml" />
</Application.Styles>
""";

    public string DataGridInstall { get; set; } = "dotnet add package AtomUI.DataGrid --version 11.0.7";

    public string DataGridStyle { get; set; } = """
<Application.Styles>
    <StyleInclude Source="avares://AtomUI.DataGrid/Index.axaml" />
</Application.Styles>
""";

    public string TreeDataGridInstall { get; set; } = "dotnet add package AtomUI.TreeDataGrid --version 11.0.7";

    public string TreeDataGridStyle { get; set; } = """
<Application.Styles>
    <StyleInclude Source="avares://AtomUI.TreeDataGrid/Index.axaml" />
</Application.Styles>
""";
}
using System.Reflection;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Performance;

internal sealed record PerfScenario(string Name, Func<int, Control> Create);

internal sealed record PerfResult(
    string Name,
    int Count,
    TimeSpan Elapsed,
    long AllocatedBytes,
    TreeStats TreeStats,
    AddOnDecoratedBoxPerfSnapshot ProbeSnapshot)
{
    public double MillisecondsPerItem => Elapsed.TotalMilliseconds / Count;
    public double KilobytesPerItem    => AllocatedBytes / 1024.0 / Count;
}

internal sealed class RealizedScenario : IDisposable
{
    public RealizedScenario(Avalonia.Controls.Window window, IReadOnlyList<Control> rootControls)
    {
        Window       = window;
        RootControls = rootControls;
    }

    public Avalonia.Controls.Window Window { get; }
    public IReadOnlyList<Control> RootControls { get; }

    public void Dispose()
    {
        Window.Close();
        Dispatcher.UIThread.RunJobs();
    }
}

internal sealed record TreeStats(
    double VisualPerRoot,
    double LogicalPerRoot,
    double ContentPresenterPerRoot,
    double SpacePerRoot,
    double CompactSpacePerRoot,
    double CompactSpaceItemPerRoot,
    double CompactSpaceAddOnPerRoot,
    double ButtonPerRoot,
    double TextBlockPerRoot,
    double PanelPerRoot,
    double BorderPerRoot,
    double DockPanelPerRoot,
    double IconPerRoot,
    double IconPresenterPerRoot,
    double ButtonIconPresenterPerRoot,
    double PathIconPerRoot,
    double StackPanelPerRoot,
    double WaveSpiritDecoratorPerRoot,
    double DashedBorderPerRoot,
    double ButtonLoadingHostPerRoot,
    double AddOnDecoratedBoxPerRoot,
    double SelectPerRoot,
    double TreeSelectPerRoot,
    double CascaderPerRoot,
    double ComboBoxPerRoot,
    double SelectHandlePerRoot,
    double SelectAccessoryHostPerRoot,
    double SelectCandidateListPerRoot,
    double SelectFilterTextBoxPerRoot,
    double SelectResultOptionsBoxPerRoot,
    double SelectTagAwareTextBoxPerRoot,
    double TreeSelectTreeViewPerRoot,
    double CascaderViewPerRoot,
    double PopupPerRoot,
    double AutoCompletePerRoot,
    double AutoCompleteSearchEditPerRoot,
    double AutoCompleteTextAreaPerRoot,
    double CandidateListPerRoot,
    double AutoCompletePopupFieldPerRoot,
    double AutoCompleteCandidateListFieldPerRoot,
    double AvatarPerRoot,
    double AvatarGroupPerRoot,
    double ImagePerRoot,
    double SvgPerRoot,
    double FlyoutHostPerRoot,
    double CountBadgePerRoot,
    double DotBadgePerRoot,
    double RibbonBadgePerRoot,
    double CountBadgeAdornerPerRoot,
    double DotBadgeAdornerPerRoot,
    double RibbonBadgeAdornerPerRoot,
    double DotBadgeIndicatorPerRoot,
    double MotionActorPerRoot,
    double LabelPerRoot)
{
    public static TreeStats Collect(IReadOnlyList<Control> roots)
    {
        var visualCount              = 0;
        var logicalCount             = 0;
        var contentPresenterCount    = 0;
        var spaceCount               = 0;
        var compactSpaceCount        = 0;
        var compactSpaceItemCount    = 0;
        var compactSpaceAddOnCount   = 0;
        var buttonCount              = 0;
        var textBlockCount           = 0;
        var panelCount               = 0;
        var borderCount              = 0;
        var dockPanelCount           = 0;
        var iconCount                = 0;
        var iconPresenterCount       = 0;
        var buttonIconPresenterCount = 0;
        var pathIconCount            = 0;
        var stackPanelCount          = 0;
        var waveSpiritDecoratorCount = 0;
        var dashedBorderCount        = 0;
        var buttonLoadingHostCount   = 0;
        var addOnDecoratedBoxCount   = 0;
        var selectCount              = 0;
        var treeSelectCount          = 0;
        var cascaderCount            = 0;
        var comboBoxCount            = 0;
        var selectHandleCount        = 0;
        var selectAccessoryHostCount = 0;
        var selectCandidateListCount = 0;
        var selectFilterTextBoxCount = 0;
        var selectResultOptionsBoxCount = 0;
        var selectTagAwareTextBoxCount  = 0;
        var treeSelectTreeViewCount     = 0;
        var cascaderViewCount           = 0;
        var popupCount                  = 0;
        var autoCompleteCount           = 0;
        var autoCompleteSearchEditCount = 0;
        var autoCompleteTextAreaCount   = 0;
        var candidateListCount          = 0;
        var autoCompletePopupFieldCount = 0;
        var autoCompleteCandidateListFieldCount = 0;
        var avatarCount                 = 0;
        var avatarGroupCount            = 0;
        var imageCount                  = 0;
        var svgCount                    = 0;
        var flyoutHostCount             = 0;
        var countBadgeCount             = 0;
        var dotBadgeCount               = 0;
        var ribbonBadgeCount            = 0;
        var countBadgeAdornerCount      = 0;
        var dotBadgeAdornerCount        = 0;
        var ribbonBadgeAdornerCount     = 0;
        var dotBadgeIndicatorCount      = 0;
        var motionActorCount            = 0;
        var labelCount                  = 0;

        foreach (var root in roots)
        {
            var visuals = root.GetSelfAndVisualDescendants().ToList();
            visualCount += visuals.Count;

            foreach (var visual in visuals)
            {
                var type = visual.GetType();
                if (type.Name == "ContentPresenter")
                {
                    contentPresenterCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.Space"))
                {
                    spaceCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.CompactSpace"))
                {
                    compactSpaceCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.CompactSpaceItem"))
                {
                    compactSpaceItemCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.CompactSpaceAddOn"))
                {
                    compactSpaceAddOnCount++;
                }
                if (visual is Avalonia.Controls.Button)
                {
                    buttonCount++;
                }
                if (type.Name == "TextBlock")
                {
                    textBlockCount++;
                }
                if (visual is Panel panel)
                {
                    panelCount++;
                    if (panel.Name == "PART_LoadingIconHost")
                    {
                        buttonLoadingHostCount++;
                    }
                }
                if (visual is Border)
                {
                    borderCount++;
                }
                if (visual is DockPanel)
                {
                    dockPanelCount++;
                }
                if (type.Name.EndsWith("Icon", StringComparison.Ordinal) || IsAtomIcon(type))
                {
                    iconCount++;
                }
                if (visual is IconPresenter iconPresenter)
                {
                    iconPresenterCount++;
                    if (iconPresenter.Name == "PART_ButtonIcon")
                    {
                        buttonIconPresenterCount++;
                    }
                }
                if (visual is PathIcon)
                {
                    pathIconCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.Avatar"))
                {
                    avatarCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.AvatarGroup"))
                {
                    avatarGroupCount++;
                }
                if (visual is Image)
                {
                    imageCount++;
                }
                if (IsTypeOrDerived(type, "Avalonia.Svg.Svg"))
                {
                    svgCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.FlyoutHost"))
                {
                    flyoutHostCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.CountBadge"))
                {
                    countBadgeCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.DotBadge"))
                {
                    dotBadgeCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.RibbonBadge"))
                {
                    ribbonBadgeCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.CountBadgeAdorner"))
                {
                    countBadgeAdornerCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.DotBadgeAdorner"))
                {
                    dotBadgeAdornerCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.RibbonBadgeAdorner"))
                {
                    ribbonBadgeAdornerCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Controls.Commons.DotBadgeIndicator"))
                {
                    dotBadgeIndicatorCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.MotionScene.BaseMotionActor"))
                {
                    motionActorCount++;
                }
                if (visual is Label)
                {
                    labelCount++;
                }
                if (visual is StackPanel)
                {
                    stackPanelCount++;
                }
                if (type.Name == "WaveSpiritDecorator")
                {
                    waveSpiritDecoratorCount++;
                }
                if (type.Name == "DashedBorder")
                {
                    dashedBorderCount++;
                }
                if (IsAddOnDecoratedBox(type))
                {
                    addOnDecoratedBoxCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.Select"))
                {
                    selectCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.TreeSelect"))
                {
                    treeSelectCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.Cascader"))
                {
                    cascaderCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.ComboBox"))
                {
                    comboBoxCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.SelectHandle"))
                {
                    selectHandleCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.SelectAccessoryHost"))
                {
                    selectAccessoryHostCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.SelectCandidateList"))
                {
                    selectCandidateListCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.SelectFilterTextBox"))
                {
                    selectFilterTextBoxCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.SelectResultOptionsBox"))
                {
                    selectResultOptionsBoxCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.SelectTagAwareTextBox"))
                {
                    selectTagAwareTextBoxCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.TreeSelectTreeView"))
                {
                    treeSelectTreeViewCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.CascaderView"))
                {
                    cascaderViewCount++;
                }
                if (visual is Avalonia.Controls.Primitives.Popup)
                {
                    popupCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.AbstractAutoComplete"))
                {
                    autoCompleteCount++;
                    if (HasFieldValue(visual, "AtomUI.Desktop.Controls.AbstractAutoComplete", "_popup"))
                    {
                        autoCompletePopupFieldCount++;
                    }
                    if (HasFieldValue(visual, "AtomUI.Desktop.Controls.AbstractAutoComplete", "_candidateList"))
                    {
                        autoCompleteCandidateListFieldCount++;
                    }
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.AutoCompleteSearchEdit"))
                {
                    autoCompleteSearchEditCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.AutoCompleteTextArea"))
                {
                    autoCompleteTextAreaCount++;
                }
                if (IsTypeOrDerived(type, "AtomUI.Desktop.Controls.Primitives.CandidateList"))
                {
                    candidateListCount++;
                }
            }

            logicalCount += root.GetSelfAndLogicalDescendants().Count();
        }

        var rootCount = Math.Max(1, roots.Count);
        return new TreeStats(
            visualCount / (double)rootCount,
            logicalCount / (double)rootCount,
            contentPresenterCount / (double)rootCount,
            spaceCount / (double)rootCount,
            compactSpaceCount / (double)rootCount,
            compactSpaceItemCount / (double)rootCount,
            compactSpaceAddOnCount / (double)rootCount,
            buttonCount / (double)rootCount,
            textBlockCount / (double)rootCount,
            panelCount / (double)rootCount,
            borderCount / (double)rootCount,
            dockPanelCount / (double)rootCount,
            iconCount / (double)rootCount,
            iconPresenterCount / (double)rootCount,
            buttonIconPresenterCount / (double)rootCount,
            pathIconCount / (double)rootCount,
            stackPanelCount / (double)rootCount,
            waveSpiritDecoratorCount / (double)rootCount,
            dashedBorderCount / (double)rootCount,
            buttonLoadingHostCount / (double)rootCount,
            addOnDecoratedBoxCount / (double)rootCount,
            selectCount / (double)rootCount,
            treeSelectCount / (double)rootCount,
            cascaderCount / (double)rootCount,
            comboBoxCount / (double)rootCount,
            selectHandleCount / (double)rootCount,
            selectAccessoryHostCount / (double)rootCount,
            selectCandidateListCount / (double)rootCount,
            selectFilterTextBoxCount / (double)rootCount,
            selectResultOptionsBoxCount / (double)rootCount,
            selectTagAwareTextBoxCount / (double)rootCount,
            treeSelectTreeViewCount / (double)rootCount,
            cascaderViewCount / (double)rootCount,
            popupCount / (double)rootCount,
            autoCompleteCount / (double)rootCount,
            autoCompleteSearchEditCount / (double)rootCount,
            autoCompleteTextAreaCount / (double)rootCount,
            candidateListCount / (double)rootCount,
            autoCompletePopupFieldCount / (double)rootCount,
            autoCompleteCandidateListFieldCount / (double)rootCount,
            avatarCount / (double)rootCount,
            avatarGroupCount / (double)rootCount,
            imageCount / (double)rootCount,
            svgCount / (double)rootCount,
            flyoutHostCount / (double)rootCount,
            countBadgeCount / (double)rootCount,
            dotBadgeCount / (double)rootCount,
            ribbonBadgeCount / (double)rootCount,
            countBadgeAdornerCount / (double)rootCount,
            dotBadgeAdornerCount / (double)rootCount,
            ribbonBadgeAdornerCount / (double)rootCount,
            dotBadgeIndicatorCount / (double)rootCount,
            motionActorCount / (double)rootCount,
            labelCount / (double)rootCount);
    }

    private static bool IsAtomIcon(Type type)
    {
        while (type.BaseType != null)
        {
            if (type.BaseType.FullName == "AtomUI.Controls.Icon")
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }

    private static bool IsAddOnDecoratedBox(Type type)
    {
        return IsTypeOrDerived(type, "AtomUI.Desktop.Controls.AddOnDecoratedBox");
    }

    private static bool IsTypeOrDerived(Type type, string fullName)
    {
        if (type.FullName == fullName)
        {
            return true;
        }

        while (type.BaseType != null)
        {
            if (type.BaseType.FullName == fullName)
            {
                return true;
            }

            type = type.BaseType;
        }

        return false;
    }

    private static bool HasFieldValue(object target, string declaringTypeName, string fieldName)
    {
        var type = target.GetType();
        while (type is not null)
        {
            if (type.FullName == declaringTypeName)
            {
                var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                return field?.GetValue(target) is not null;
            }

            type = type.BaseType;
        }

        return false;
    }
}

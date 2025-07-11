using System.Runtime.CompilerServices;
using Avalonia.LogicalTree;
using Avalonia.Rendering;

namespace AtomUI.Controls.Utils;

internal interface IRadioButton : ILogical
{
    string? GroupName { get; }
    ItemToggleType ToggleType { get; }
    bool? IsChecked { get; set; }
}

internal class RadioButtonGroupManager
{
    private static readonly RadioButtonGroupManager Default = new();
    private static readonly ConditionalWeakTable<IRenderRoot, RadioButtonGroupManager> RegisteredVisualRoots = new();

    private readonly Dictionary<string, List<WeakReference<IRadioButton>>> _registeredGroups = new();
    private bool _ignoreCheckedChanges;

    public static RadioButtonGroupManager GetOrCreateForRoot(IRenderRoot? root)
    {
        if (root == null)
        {
            return Default;
        }
        return RegisteredVisualRoots.GetValue(root, key => new RadioButtonGroupManager());
    }

    public void Add(IRadioButton radioButton)
    {
        var groupName = radioButton.GroupName;
        if (groupName is not null && radioButton.ToggleType == ItemToggleType.Radio)
        {
            if (!_registeredGroups.TryGetValue(groupName, out var group))
            {
                group = new List<WeakReference<IRadioButton>>();
                _registeredGroups.Add(groupName, group);
            }
            group.Add(new WeakReference<IRadioButton>(radioButton));
        }
    }

    public void Remove(IRadioButton radioButton, string? oldGroupName)
    {
        if (!string.IsNullOrEmpty(oldGroupName) && _registeredGroups.TryGetValue(oldGroupName, out var group))
        {
            int i = 0;
            while (i < group.Count)
            {
                if (!group[i].TryGetTarget(out var button) || button == radioButton)
                {
                    group.RemoveAt(i);
                    continue;
                }

                i++;
            }

            if (group.Count == 0)
            {
                _registeredGroups.Remove(oldGroupName);
            }
        }
    }

    public void OnCheckedChanged(IRadioButton radioButton)
    {
        if (_ignoreCheckedChanges || radioButton.ToggleType != ItemToggleType.Radio)
        {
            return;
        }

        _ignoreCheckedChanges = true;
        try
        {
            var groupName = radioButton.GroupName;
            if (!string.IsNullOrEmpty(groupName))
            {
                if (_registeredGroups.TryGetValue(groupName, out var group))
                {
                    var i = 0;
                    while (i < group.Count)
                    {
                        if (!group[i].TryGetTarget(out var current))
                        {
                            group.RemoveAt(i);
                            continue;
                        }

                        if (current != radioButton && current.IsChecked.HasValue && current.IsChecked.Value)
                        {
                            current.IsChecked = false;
                        }
                        i++;
                    }

                    if (group.Count == 0)
                    {
                        _registeredGroups.Remove(groupName);
                    }
                }
            }
            else
            {
                if (radioButton.LogicalParent is { } parent)
                {
                    foreach (var sibling in parent.LogicalChildren)
                    {
                        if (sibling != radioButton
                            && sibling is IRadioButton { ToggleType: ItemToggleType.Radio } button
                            && string.IsNullOrEmpty(button.GroupName)
                            && button.IsChecked.HasValue
                            && button.IsChecked.Value)
                        {
                            button.IsChecked = false;
                        }
                    }
                }
            }
        }
        finally
        {
            _ignoreCheckedChanges = false;
        }
    }
}
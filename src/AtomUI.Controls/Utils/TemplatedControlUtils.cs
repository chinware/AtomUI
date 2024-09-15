﻿using Avalonia;
using Avalonia.LogicalTree;

namespace AtomUI.Controls.Utils;

public static class TemplatedControlUtils
{
   /// <summary>
   /// Sets the TemplatedParent property for the created template children.
   /// </summary>
   /// <param name="control">The control.</param>
   /// <param name="templatedParent">The templated parent to apply.</param>
   internal static void ApplyTemplatedParent(StyledElement control, AvaloniaObject? templatedParent)
    {
        UIStructureUtils.SetTemplateParent(control, templatedParent);
        var logicalChildren = control.GetLogicalChildren();
        foreach (var child in logicalChildren)
        {
            if (child is StyledElement styledElement && styledElement.TemplatedParent == null)
            {
                ApplyTemplatedParent(styledElement, templatedParent);
            }
        }
    }
}
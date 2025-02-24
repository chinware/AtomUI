﻿using AtomUI.Controls.Internal;
using AtomUI.IconPkg;
using AtomUI.IconPkg.AntDesign;
using AtomUI.Theme.Styling;
using Avalonia.Controls;

namespace AtomUI.Controls;

[ControlThemeProvider]
internal class TimePickerTheme : InfoPickerInputTheme
{
    public TimePickerTheme() : base(typeof(TimePicker))
    {
    }
    
    protected override Icon BuildInfoIcon(InfoPickerInput infoPickerInput, INameScope scope)
    {
        return AntDesignIconPackage.ClockCircleOutlined();
    }

    protected override TextBox BuildPickerTextBox(string name)
    {
        var pickerTextBox = base.BuildPickerTextBox(name);
        CreateTemplateParentBinding(pickerTextBox, TextBox.WidthProperty, InfoPickerInput.PreferredInputWidthProperty);
        return pickerTextBox;
    }
}
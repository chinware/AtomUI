namespace AtomUI.Theme;

internal interface ITokenResourceLayer
{
    void Calculate();
    void MountTokenResources(ThemeConfigProvider themeConfigProvider);
}
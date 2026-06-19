using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AtomUI.Theme;

namespace AtomUI.Theme
{
    internal class ControlTokenTypePool
    {
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AtomUI.Toolkits.GalleryBase.Controls.GalleryStickyTabsHostToken))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AtomUI.Toolkits.GalleryBase.Controls.GalleryWindowTitleBarToken))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AtomUI.Toolkits.GalleryBase.Controls.ShowCaseItemToken))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AtomUI.Toolkits.GalleryBase.Controls.ShowCasePanelToken))]
        internal static IList<ControlTokenRegistration> GetTokenTypes()
        {
            List<ControlTokenRegistration> tokenTypes = new List<ControlTokenRegistration>(4);
            tokenTypes.Add(new ControlTokenRegistration(typeof(AtomUI.Toolkits.GalleryBase.Controls.GalleryStickyTabsHostToken)));
            tokenTypes.Add(new ControlTokenRegistration(typeof(AtomUI.Toolkits.GalleryBase.Controls.GalleryWindowTitleBarToken)));
            tokenTypes.Add(new ControlTokenRegistration(typeof(AtomUI.Toolkits.GalleryBase.Controls.ShowCaseItemToken)));
            tokenTypes.Add(new ControlTokenRegistration(typeof(AtomUI.Toolkits.GalleryBase.Controls.ShowCasePanelToken)));
            return tokenTypes;
        }
    }
}
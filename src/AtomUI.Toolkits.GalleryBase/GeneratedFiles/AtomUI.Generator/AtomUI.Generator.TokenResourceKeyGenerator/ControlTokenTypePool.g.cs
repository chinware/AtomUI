using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AtomUI.Theme;

namespace AtomUI.Theme
{
    internal class ControlTokenTypePool
    {
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AtomUI.Toolkits.GalleryBase.Controls.GalleryShowCaseHeaderToken))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AtomUI.Toolkits.GalleryBase.Controls.GalleryStickyTabsHostToken))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AtomUI.Toolkits.GalleryBase.Controls.GalleryWindowTitleBarToken))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AtomUI.Toolkits.GalleryBase.Controls.ShowCaseItemToken))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AtomUI.Toolkits.GalleryBase.Controls.ShowCasePanelToken))]
        internal static IList<ControlTokenRegistration> GetTokenTypes()
        {
            List<ControlTokenRegistration> tokenTypes = new List<ControlTokenRegistration>(5);
            tokenTypes.Add(new ControlTokenRegistration(typeof(AtomUI.Toolkits.GalleryBase.Controls.GalleryShowCaseHeaderToken), "GalleryShowCaseHeader", null));
            tokenTypes.Add(new ControlTokenRegistration(typeof(AtomUI.Toolkits.GalleryBase.Controls.GalleryStickyTabsHostToken), "GalleryStickyTabsHost", null));
            tokenTypes.Add(new ControlTokenRegistration(typeof(AtomUI.Toolkits.GalleryBase.Controls.GalleryWindowTitleBarToken), "GalleryWindowTitleBar", null));
            tokenTypes.Add(new ControlTokenRegistration(typeof(AtomUI.Toolkits.GalleryBase.Controls.ShowCaseItemToken), "ShowCaseItem", null));
            tokenTypes.Add(new ControlTokenRegistration(typeof(AtomUI.Toolkits.GalleryBase.Controls.ShowCasePanelToken), "ShowCasePanel", null));
            return tokenTypes;
        }
    }
}
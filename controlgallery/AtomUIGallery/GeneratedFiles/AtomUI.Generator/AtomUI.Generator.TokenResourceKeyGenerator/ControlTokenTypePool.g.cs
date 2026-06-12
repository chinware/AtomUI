using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AtomUI.Theme;

namespace AtomUI.Theme
{
    internal class ControlTokenTypePool
    {
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AtomUIGallery.Controls.GalleryStickyTabsHostToken))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AtomUIGallery.Controls.GalleryWindowTitleBarToken))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AtomUIGallery.Controls.ShowCaseItemToken))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties, typeof(AtomUIGallery.Controls.ShowCasePanelToken))]
        internal static IList<ControlTokenRegistration> GetTokenTypes()
        {
            List<ControlTokenRegistration> tokenTypes = new List<ControlTokenRegistration>(4);
            tokenTypes.Add(new ControlTokenRegistration(typeof(AtomUIGallery.Controls.GalleryStickyTabsHostToken)));
            tokenTypes.Add(new ControlTokenRegistration(typeof(AtomUIGallery.Controls.GalleryWindowTitleBarToken)));
            tokenTypes.Add(new ControlTokenRegistration(typeof(AtomUIGallery.Controls.ShowCaseItemToken)));
            tokenTypes.Add(new ControlTokenRegistration(typeof(AtomUIGallery.Controls.ShowCasePanelToken)));
            return tokenTypes;
        }
    }
}
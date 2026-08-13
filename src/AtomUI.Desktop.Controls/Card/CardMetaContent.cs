using AtomUI.Controls;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

[SemanticPart(
    "section",
    SelectorClass = "semantic-section",
    ContractType = typeof(Control),
    Since = "6.0")]
[SemanticPart(
    "avatar",
    SelectorClass = "semantic-avatar",
    ContractType = typeof(ContentPresenter),
    Since = "6.0")]
[SemanticPart(
    "title",
    SelectorClass = "semantic-title",
    ContractType = typeof(ContentPresenter),
    Since = "6.0")]
[SemanticPart(
    "description",
    SelectorClass = "semantic-description",
    ContractType = typeof(ContentPresenter),
    Since = "6.0")]
public class CardMetaContent : HeaderedContentControl
{
    #region 公共属性定义
    
    public static readonly StyledProperty<Avatar?> AvatarProperty = 
        AvaloniaProperty.Register<CardMetaContent, Avatar?>(nameof (Avatar));
    
    public Avatar? Avatar
    {
        get => GetValue(AvatarProperty);
        set => SetValue(AvatarProperty, value);
    }
    
    #endregion
}

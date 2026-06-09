# AtomUI Gallery 组织规范

本文档约定 `controlgallery/AtomUIGallery/ShowCases` 的文件组织方式。目标是让每个 ShowCase 的界面、状态模型和多语言文案就近维护，避免后续补充示例或翻译时在多棵目录之间来回查找。

## 基本结构

每个 ShowCase 按左侧导航分类放置，并在分类下建立独立的 ShowCase 目录：

```text
ShowCases/
  <Category>/
    <ShowCase>/
      Views/
      ViewModels/
      Localization/
```

以 `AutoComplete` 为例：

```text
ShowCases/
  DataEntry/
    AutoComplete/
      Views/
        AutoCompleteShowCase.axaml
        AutoCompleteShowCase.axaml.cs
      ViewModels/
        AutoCompleteViewModel.cs
      Localization/
        en_US.cs
        zh_CN.cs
```

## Namespace 规则

代码命名空间只体现具体 ShowCase，不包含分类层级，也不包含 `Views`、`ViewModels`、`Localization` 后缀。

```csharp
namespace AtomUIGallery.ShowCases.AutoComplete;
```

也就是说，以下文件使用同一个 namespace：

```text
ShowCases/DataEntry/AutoComplete/Views/AutoCompleteShowCase.axaml.cs
ShowCases/DataEntry/AutoComplete/ViewModels/AutoCompleteViewModel.cs
ShowCases/DataEntry/AutoComplete/Localization/en_US.cs
ShowCases/DataEntry/AutoComplete/Localization/zh_CN.cs
```

这样做的原因：

| 项 | 约定 |
| --- | --- |
| 分类目录 | 只承担导航和文件归类职责，例如 `DataEntry`、`DataDisplay` |
| ShowCase namespace | 使用 `AtomUIGallery.ShowCases.<ShowCase>` |
| `Views` / `ViewModels` / `Localization` | 只作为文件夹职责，不进入 namespace |
| XAML `using:` | 指向具体 ShowCase namespace，例如 `using:AtomUIGallery.ShowCases.AutoComplete` |

## 多语言规则

ShowCase 自己的标题、描述和场景文案必须跟随 ShowCase 放在同一个目录下的 `Localization` 中。

```text
ShowCases/
  DataEntry/
    AutoComplete/
      Localization/
        en_US.cs
        zh_CN.cs
```

不要在 `Localization` 下再套一层 `AutoCompleteShowCaseLang/`。目录已经处在 `AutoComplete` 作用域内，额外层级会增加路径深度但没有维护收益。

语言资源仍然使用 AtomUI 推荐的 provider / generator 方式：

```csharp
[LanguageProvider(LanguageCode.en_US, AutoCompleteShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicUsageTitle = "Basic Usage";
    public const string BasicUsageDescription = "Basic usage of AutoComplete.";

    protected override Type GetResourceKindType()
        => typeof(AutoCompleteShowCaseLangResourceKind);
}
```

XAML 中使用生成的资源扩展：

```xml
<gallery:ShowCaseItem
    Title="{gallery:AutoCompleteShowCaseLangResource BasicUsageTitle}"
    Description="{gallery:AutoCompleteShowCaseLangResource BasicUsageDescription}">
```

### 选项数据多语言

不要把 ShowCase 语言资源直接写到普通选项数据对象上，例如 `SelectOption`、`CascaderOption`、`TreeItemNode`、`AutoCompleteOption` 等。

这些对象不是 Avalonia `StyledElement`，`{gallery:...LangResource ...}` 在它们身上会退化成一次性静态值，语言切换后不会自动刷新。正确做法是把选项数据放到对应 ShowCase 的 ViewModel 中，由 code-behind 使用 `LanguageResourceBinder.GetLangResource(...)` 构建，并在 `ThemeManager.LanguageVariantChanged` 后重建数据源。

选项数据必须保留稳定身份：

| 控件数据 | 稳定身份 |
| --- | --- |
| `SelectOption` / `AutoCompleteOption` | 优先 `ItemKey`，否则 `Content` |
| `CascaderOption` / `TreeItemNode` | 优先 `ItemKey`，否则 `Value` |

`Header`、`Description`、分组标题等自然语言字段可以跟随语言变化，但不要作为选择值、默认值或节点路径的身份字段。这样语言切换时控件可以按稳定身份把旧选中项映射到新语言的数据对象上。

## 复杂 ShowCase

如果一个 ShowCase 拆分为多个子场景，子场景页面仍放在同一个 `Views` 目录下。

```text
ShowCases/
  DataDisplay/
    DataGrid/
      Views/
        DataGridShowCase.axaml
        DataGridBasicShowCase.axaml
        DataGridFilteringShowCase.axaml
        DataGridPagingShowCase.axaml
      ViewModels/
        DataGridViewModel.cs
      Localization/
        en_US.cs
        zh_CN.cs
```

子场景不单独建立新的 ShowCase namespace，除非它已经成为左侧导航中的独立 ShowCase。

## 辅助控件

只服务于某个 ShowCase 的辅助 demo 控件，放在该 ShowCase 目录下。多个 ShowCase 共用的辅助控件，才保留在公共 `ShowCaseControls` 目录。

推荐：

```text
ShowCases/
  DataEntry/
    Form/
      ShowCaseControls/
        Captcha.cs
        CaptchaTheme.axaml
```

公共复用时：

```text
ShowCases/
  ShowCaseControls/
```

## 迁移规则

迁移现有 ShowCase 时按以下顺序执行：

1. 先做纯目录重组，不改业务逻辑。
2. 同步调整 namespace、XAML `x:Class` 和 `using:`。
3. 确认 `ShowCaseRegister`、导航和 lazy tab 引用仍能正常解析。
4. 再补充该 ShowCase 的 `Localization/en_US.cs` 和 `Localization/zh_CN.cs`。
5. 每迁移一批后执行 Gallery Debug build。

目录重组和多语言补齐可以分 commit 处理，避免一个提交同时包含大量移动和文案变更。

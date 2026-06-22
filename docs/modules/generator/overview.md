# AtomUI.Generator 模块概览

`AtomUI.Generator` 是 AtomUI 的 Roslyn 源生成器项目，TargetFramework 为 `netstandard2.0`。它不作为运行时依赖使用，而是以 Analyzer 方式被多个项目引用。

## 职责

- 根据 Design Token Attribute 生成 Token 资源键常量。
- 根据 Control Token Attribute 生成控件 Token 类型池。
- 根据 Language Provider Attribute 生成语言资源键和 Provider 池。
- 降低控件包手工维护 Token/语言注册列表的成本。

## 生成器

| 生成器 | 说明 |
|---|---|
| `TokenResourceKeyGenerator` | 扫描全局 Token 与 Control Token，生成资源键和 Token 类型池 |
| `LanguageGenerator` | 扫描语言 Provider，生成语言资源键和 Provider 池 |
| `DataMemberAccessorGenerator` | 根据数据模型 Attribute 生成 AOT 友好的数据成员访问器注册 |
| `ScopedResourceHostGenerator` | 根据 `[GenerateScopedResourceHost]` 为非 Visual `AvaloniaObject` 生成 scoped 资源宿主生命周期样板代码 |

## 关键目录

| 目录 | 说明 |
|---|---|
| `DesignToken/` | Token Walker、TokenInfo、资源键和类型池 Writer |
| `Language/` | LanguageProvider Walker、语言键和 Provider 池 Writer |
| `DataMemberAccessors/` | 数据成员访问器 Generator、Analyzer 和 SourceWriter |
| `ResourceHost/` | 非 Visual `AvaloniaObject` scoped resource host Generator、TypeInfo 和 SourceWriter |
| `TargetMarkConstants.cs` | 生成器识别的 Attribute 元数据名 |

## 统一开发范式

- [Scoped Resource Host Source Generator 范式](scoped-resource-host-generator.md)：非 Visual `AvaloniaObject` 需要承载 Avalonia 属性绑定和动态资源时，默认通过 Source Generator 生成 scoped `IResourceHost` / `IThemeVariantHost` 生命周期样板代码。

## 维护注意

新增控件 Token 或语言 Provider 后，应检查对应项目的 `GeneratedFiles/AtomUI.Generator/` 输出，确认生成器已识别目标类型。由于生成目录被 `<Compile Remove=...>` 排除，不应把生成文件当成普通源码维护。

新增或修改 Generator 时，应同时检查 writer 代码、诊断规则和生成物稳定性。对于非 Visual `AvaloniaObject` 资源宿主类需求，不要在控件对象中复制手写资源宿主代码，应优先按 [Scoped Resource Host Source Generator 范式](scoped-resource-host-generator.md) 落地。

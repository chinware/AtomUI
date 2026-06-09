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

## 关键目录

| 目录 | 说明 |
|---|---|
| `DesignToken/` | Token Walker、TokenInfo、资源键和类型池 Writer |
| `Language/` | LanguageProvider Walker、语言键和 Provider 池 Writer |
| `TargetMarkConstants.cs` | 生成器识别的 Attribute 元数据名 |

## 维护注意

新增控件 Token 或语言 Provider 后，应检查对应项目的 `GeneratedFiles/AtomUI.Generator/` 输出，确认生成器已识别目标类型。由于生成目录被 `<Compile Remove=...>` 排除，不应把生成文件当成普通源码维护。


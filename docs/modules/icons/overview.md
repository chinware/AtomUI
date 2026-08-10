# Icons 模块概览

图标相关项目分为共享生成基础设施、Ant Design 图标包和 Ant Design 图标生成器。

## 项目

| 项目 | 说明 |
|---|---|
| `AtomUI.Icons.Shared` | SVG 解析、图标包生成基础类型 |
| `AtomUI.Icons.AntDesign` | Ant Design 图标运行时包 |
| `AtomUI.Icons.AntDesign.Generator` | 从 Ant Design SVG 资源生成图标源码 |

## Ant Design 图标包

`AtomUI.Icons.AntDesign` 依赖 `AtomUI.Core`，包含：

- `AntDesignIcon`
- `AntDesignIconProvider`
- `GeneratedIcons/*.g.cs`

生成图标按 Filled、Outlined、TwoTone 等 Ant Design 类型组织，并生成 `AntDesignIconKind` 供控件和使用者引用。

## 与控件包关系

`AtomUI.Controls` 依赖 `AtomUI.Icons.AntDesign`，因此桌面主控件包通过公共控件包间接获得图标能力。


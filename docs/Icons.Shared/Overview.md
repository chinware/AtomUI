# AtomUI Icon 系统文档

## 概述

AtomUI Icon 系统是一套**构建时代码生成 + 运行时高性能渲染**的图标解决方案。它将 SVG 图标在构建时转换为原生 Avalonia 绘图指令（`DrawingInstruction`），实现零运行时解析开销、类型安全的图标引用、以及自动 Tree-shaking。

### 核心设计理念

1. **构建时生成**：SVG → C# 代码，编译后成为原生绘图指令，无运行时 SVG 解析
2. **性能优先**：`DrawingInstruction` 静态共享 + `Geometry` 惰性缓存 + `IconProvider` 三级工厂缓存
3. **可扩展性**：通过继承生成器框架，可轻松创建任意 Icon 包（Material Icons、Fluent Icons、自定义业务图标等）
4. **类型安全**：每个图标是独立的 C# 类，枚举引用提供编译时检查和 IDE 智能提示
5. **主题集成**：图标颜色通过 Design Token 系统控制，自动适配亮/暗主题

---

## 文档目录

| 文档 | 说明 | 目标读者 |
|---|---|---|
| [Architecture.md](./Architecture.md) | **底层原理设计解析** — 架构分层、关键类型与接口、设计决策分析 | 框架开发者、需要深入理解实现原理的工程师 |
| [AntDesignIconPackage.md](./AntDesignIconPackage.md) | **AntDesign Icon 包实例解析** — 完整的生成流程、生成代码示例、依赖关系 | 框架开发者、需要维护或修改 Ant Design 图标包的工程师 |
| [IconPackageCreationGuide.md](./IconPackageCreationGuide.md) | **Icon 包制作指导手册** — 创建自定义 Icon 包的完整步骤、注意事项与最佳实践 | 需要创建自定义图标包的工程师 |
| [UsageGuide.md](./UsageGuide.md) | **Icon 使用规范指南** — C# 和 AXAML 中的使用方式、属性速查、编码规范 | 所有使用 AtomUI 的开发者 |

---

## 快速参考

### 在 AXAML 中使用 Ant Design 图标

```xml
<!-- 1. 声明命名空间 -->
xmlns:antdicons="https://atomui.net/icons/antdesign"

<!-- 2a. 作为 XML 元素 -->
<antdicons:LoadingOutlined LoadingAnimation="Spin" />

<!-- 2b. 作为属性值（通过 MarkupExtension） -->
<atom:Button Icon="{antdicons:AntDesignIconProvider CheckCircleFilled}" />
```

### 在 C# 中使用 Ant Design 图标

```csharp
using AtomUI.Icons.AntDesign;

// 直接实例化
var icon = new CheckCircleFilled();

// 带动画
var loading = new LoadingOutlined { LoadingAnimation = IconAnimation.Spin };

// 设置到控件属性
SetCurrentValue(IconProperty, new CloseCircleFilled());
```

### 项目依赖关系

```
                   构建时                                    运行时
┌──────────────────────────────┐       ┌──────────────────────────────────┐
│ Icons.AntDesign.Generator    │       │ Icons.AntDesign                  │
│   └── Icons.Shared           │       │   └── AtomUI.Core                │
│        └── AtomUI.Core       │       │       (Icon, DrawingInstruction,  │
│            (枚举定义)        │       │        IconProvider<T>)           │
└──────────────────────────────┘       └──────────────────────────────────┘
        ↓ dotnet run 生成代码                    ↑ 引用生成的代码
        └────────────────────────────────────────┘
```


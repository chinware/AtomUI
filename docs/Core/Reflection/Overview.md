# Avalonia 内部成员反射访问规范

> 本文档是 AtomUI 控件库访问 Avalonia 框架私有/内部成员的强制性规范。  
> 适用版本：AtomUI v5.0+，基于 Avalonia v11  
> 最后更新：2026-04-11

---

## 文档索引

| 文档 | 内容 |
|---|---|
| [架构设计与原理](./Architecture.md) | 为什么需要反射访问 Avalonia 内部成员、`DynamicDependency` 注解的作用、Trimming 安全性、设计决策 |
| [编码规范](./CodingStandards.md) | `XxxReflectionExtensions` 类的标准写法、命名约定、反射信息定义区域、扩展方法封装规则 |
| [基础设施 API 参考](./InfrastructureAPI.md) | `AtomUI.Reflection` 命名空间提供的反射工具方法：`TypeMemberExtension`、`ObjectExtension` |
| [现有反射扩展清单](./ExistingExtensions.md) | 项目中所有 `ReflectionExtensions` 类的完整清单、所属项目、Hook 的 Avalonia 目标成员 |

---

## 核心原则

### 1. 反射是最后手段

AtomUI 作为一个深度定制的 UI 控件库，需要对 Avalonia 内置控件进行细粒度控制。当 Avalonia 框架未通过公开 API 暴露某些关键能力时（如内部方法、私有字段、受保护属性），我们 **不得不** 使用反射来访问这些成员。

**只有在以下场景中才允许使用反射**：
- Avalonia 未提供公开 API 实现所需功能
- 需要深度定制 Avalonia 内置控件的行为
- 需要访问框架内部状态以实现正确的控件生命周期管理

### 2. 集中管理，统一封装

所有反射访问 **必须** 封装在专门的 `XxxReflectionExtensions` 静态扩展类中，**禁止** 在业务逻辑代码中直接使用 `System.Reflection` API。

### 3. Trimming 安全

所有反射字段/方法/属性的缓存声明 **必须** 标注 `[DynamicDependency]` 注解，确保 .NET Trimmer 不会在发布时裁剪掉被反射访问的成员。

### 4. 复用已有扩展

在编写新的反射访问代码之前，**必须** 先检查以下位置是否已存在所需的反射扩展：
- `AtomUI.Core` 中的 `Reflection/`、`Controls/`、`Animations/`、`Data/`、`Media/`、`Utils/`、`Input/` 目录
- `AtomUI.Desktop.Controls` 中各控件文件夹和 `Primitives/` 目录

### 5. 感知升级风险

反射访问的成员不受 Avalonia 语义版本控制保护。每次升级 Avalonia 版本时，**必须** 验证所有反射扩展类中 Hook 的成员是否仍然存在并且签名未变更。`GetXxxInfoOrThrow` 系列方法会在成员缺失时立即抛出异常，确保在开发阶段及早发现兼容性问题。


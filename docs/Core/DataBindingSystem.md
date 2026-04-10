# AtomUI 数据绑定系统（Data Binding System）

## 概述

数据绑定系统位于 `src/AtomUI.Core/Data/` 目录，是连接 AtomUI 主题 Token、语言资源与 UI 控件属性的**桥梁层**。它在 Avalonia 原生绑定机制之上构建了一套面向设计系统的绑定基础设施，实现了 Token 值到控件属性的自动、响应式传递。

### 在整体架构中的位置

```
主题系统（Token 定义与管理）
    ↓ 提供 Token 值
Data Binding System（本文档 — Token/资源 → 属性绑定）
    ↓ 驱动
UI 控件属性（样式、尺寸、颜色等）
```

## 目录结构

```
Data/
├── BindUtils.cs                                 # 属性中继绑定
├── TokenResourceBinder.cs                       # Token 资源绑定器（核心）
├── TokenResourceUtils.cs                        # Token 资源同步查找
├── TokenFinderUtils.cs                          # Token 层次遍历查找
├── LanguageResourceBinder.cs                    # 语言资源绑定器
├── InstancedBindingFactory.cs                   # Rx 绑定工厂
├── RenderScaleAwareDoubleConfigure.cs           # DPI 感知值转换器
└── DynamicResourceReflectionExtension.cs        # DynamicResource 反射扩展
```

## 核心组件

### 1. TokenResourceBinder — Token 资源绑定器

`TokenResourceBinder` 是整个数据绑定系统的**核心 API**，提供将主题 Token 绑定到控件属性的多种策略。

#### 三种绑定策略

| 策略 | 方法 | 特点 |
|------|------|------|
| **DynamicResource** | `CreateTokenBinding` | 使用 Avalonia 资源系统，自动响应主题切换 |
| **Context Observable** | `CreateTokenBinding`（带 context） | 基于控件上下文的响应式绑定，支持自定义转换 |
| **Global Observable** | `CreateGlobalTokenBinding` | 绑定到全局 Token，不依赖控件上下文 |

#### DynamicResource 绑定

```csharp
// 将主题 Token 作为 DynamicResource 绑定到属性
TokenResourceBinder.CreateTokenBinding(
    control,
    TextBlock.ForegroundProperty,
    GlobalTokenResourceKey.ColorText  // Token 资源键
);
```

当主题切换时，Avalonia 资源系统自动更新绑定值。

#### 上下文响应式绑定

```csharp
// 基于控件上下文的 Token 绑定（支持值转换）
TokenResourceBinder.CreateTokenBinding(
    control,
    Border.CornerRadiusProperty,
    SharedTokenResourceKey.BorderRadius,
    BindingPriority.Style,
    converter: v => new CornerRadius((double)v)
);
```

#### 全局 Token 绑定

```csharp
// 绑定全局 Token（不依赖控件在视觉树中的位置）
TokenResourceBinder.CreateGlobalTokenBinding(
    control,
    Control.FontSizeProperty,
    GlobalTokenResourceKey.FontSize
);
```

### 2. TokenResourceUtils — Token 同步查找

提供**同步**的 Token 资源查找能力，用于需要立即获取 Token 值的场景：

```csharp
// 从控件上下文查找 Token（沿视觉树向上搜索）
object? value = TokenResourceUtils.FindTokenResource(
    control, 
    SharedTokenResourceKey.BorderRadius
);

// 从全局 Token 注册表查找
object? value = TokenResourceUtils.FindGlobalTokenResource(
    GlobalTokenResourceKey.ColorPrimary
);
```

### 3. TokenFinderUtils — Token 层次遍历

实现沿控件层次结构查找 Token 的逻辑，支持三种 Token 类型的遍历：

```
控件（Control）
    ↑ 查找 ControlToken
控件模板父级（TemplatedParent）
    ↑ 查找 SharedToken
主题变体（ThemeVariant）
    ↑ 查找 GlobalToken
```

#### 查找优先级

1. **ControlToken** — 控件级别的专属 Token（如 Button 的特定颜色）
2. **SharedToken** — 共享 Token（如通用的边框圆角）
3. **GlobalToken** — 全局 Token（如基础字体大小、主色调）

### 4. BindUtils — 属性中继绑定

提供 `AvaloniaObject` 之间的属性中继绑定，将一个对象的属性变化自动传递到另一个对象：

```csharp
// 基础中继绑定
BindUtils.RelayBind(source, SourceProperty, target, TargetProperty);

// 带转换器的泛型中继绑定
BindUtils.RelayBind<TSource, TTarget>(
    source, SourceProperty, 
    target, TargetProperty, 
    converter: v => Transform(v)
);
```

#### 三种重载

| 重载 | 说明 |
|------|------|
| 基础版 | 直接属性到属性的单向绑定 |
| 优先级版 | 指定绑定优先级（如 Style/Template/Local） |
| 泛型转换版 | 带类型安全的值转换函数 |

### 5. LanguageResourceBinder — 语言资源绑定器

为国际化系统提供资源绑定能力，将语言资源键绑定到控件的文本属性：

```csharp
// 绑定语言资源（语言切换时自动更新）
LanguageResourceBinder.CreateBinding(
    control,
    TextBlock.TextProperty,
    LanguageResourceKey.OK
);
```

支持通过 Observable 方式响应语言切换事件。

### 6. InstancedBindingFactory — Rx 绑定工厂

基于 `System.Reactive` 创建 Avalonia 的 `InstancedBinding`，支持所有绑定模式：

| 模式 | 说明 |
|------|------|
| `OneTime` | 一次性赋值 |
| `OneWay` | 源到目标的单向绑定 |
| `OneWayToSource` | 目标到源的反向绑定 |
| `TwoWay` | 双向绑定 |

### 7. RenderScaleAwareDoubleConfigure — DPI 感知转换

实现 DPI 感知的 `double` 值转换，确保在不同显示缩放比例下的渲染精度：

```csharp
// 根据渲染缩放比例调整值
// 例如：1px 边框在 2x DPI 下需要调整为 0.5 以保持视觉一致
```

#### 弱引用管理

使用 `WeakReference` 持有目标控件引用，避免转换器持有控件导致内存泄漏。

### 8. DynamicResourceReflectionExtension

通过反射访问 `DynamicResourceExtension` 的私有 `_anchor` 字段，获取资源查找的锚点控件。使用 `[DynamicDependency]` 确保 AOT 安全。

## 绑定流程

### Token 绑定的完整链路

```
1. 主题系统定义 Token 值
       ↓
2. Token 注册为 Avalonia 资源（DynamicResource）
       ↓
3. TokenResourceBinder 创建绑定
       ↓
4. 控件属性自动更新
       ↓
5. 主题切换时，资源系统自动推送新值
       ↓
6. 绑定管道触发属性更新 → 控件重绘
```

### 主题切换响应

```
用户切换主题（Light ↔ Dark）
    ↓
ThemeManager 更新 ThemeVariant
    ↓
Avalonia ResourceDictionary 切换
    ↓
DynamicResource 自动解析到新值
    ↓
TokenResourceBinder 的绑定自动更新
    ↓
控件属性变更 → 视觉更新
```

## 设计模式

| 模式 | 应用 |
|------|------|
| **中介者模式** | `TokenResourceBinder` 作为 Token 系统与控件属性之间的中介 |
| **策略模式** | 三种绑定策略（DynamicResource/Context/Global）可按场景选择 |
| **责任链模式** | `TokenFinderUtils` 沿控件层次逐级向上查找 Token |
| **工厂模式** | `InstancedBindingFactory` 创建不同模式的绑定实例 |
| **观察者模式** | 基于 Rx Observable 的响应式绑定和语言资源绑定 |
| **弱引用模式** | `RenderScaleAwareDoubleConfigure` 防止内存泄漏 |

## 相关文档

- [主题系统概览](./ThemeSystem/Overview.md) — Token 的定义与来源
- [Token 架构](./ThemeSystem/TokenArchitecture.md) — Token 的分层设计
- [Token 资源绑定](./ThemeSystem/TokenResourceBinding.md) — Token 到资源的映射
- [语言系统概览](./LanguageSystem/Overview.md) — 国际化资源体系
- [架构概览](./Architecture.md) — AtomUI.Core 整体架构

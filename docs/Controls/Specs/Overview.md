# AtomUI 控件 API 定义规范

> 本规范是 AtomUI 控件库所有控件必须强制遵守的 API 定义标准。  
> 适用版本：AtomUI v5.0+，基于 Avalonia v11  
> 最后更新：2026-04-11

---

## 文档索引

| 文档 | 内容 |
|---|---|
| [属性定义规范](./PropertyDefinitionStandards.md) | StyledProperty、DirectProperty、AttachedProperty 的选型规则、注册方式、命名约定与 CLR 包装器规范 |
| [控件类结构规范](./ControlClassStructure.md) | 控件的层次继承模型、接口实现、构造函数、代码区域组织与命名空间约定 |
| [Design Token 集成规范](./TokenSystemIntegration.md) | 组件 Token 定义、CalculateTokenValues 派生规则、Token 资源引用方式 |
| [主题文件结构规范](./ThemeFileStructure.md) | Theme 文件夹布局、ControlTheme AXAML 结构、Themes.axaml 注册 |
| [伪类定义规范](./PseudoClassConventions.md) | 伪类常量定义、StdPseudoClass 复用规则、`[PseudoClasses]` 注解与运行时设置 |
| [事件与生命周期规范](./EventsAndLifecycle.md) | RoutedEvent 定义、OnApplyTemplate 模板部件获取、OnPropertyChanged 响应、Transition 配置与生命周期管理 |

---

## 核心原则

### 1. 忠于 Avalonia 属性系统

AtomUI 构建在 Avalonia 之上，所有控件属性 **必须** 通过 Avalonia 属性系统（`StyledProperty`、`DirectProperty`、`AttachedProperty`）定义。不允许使用纯 CLR 属性暴露需要被样式系统或数据绑定消费的值。

### 2. 忠于 Ant Design 5.0

所有控件的外观与交互行为必须严格遵循 [Ant Design 5.0 设计规范](https://ant.design/components/overview-cn)。控件的属性命名、默认值、枚举值均应对齐 Ant Design 5.0 的 React API。

### 3. 三层架构分离

```
AtomUI.Controls.Shared (Foundation)  →  AtomUI.Controls (Base)  →  AtomUI.Desktop.Controls (Platform)
```

- **Foundation 层**（`AtomUI.Controls.Shared`）：定义共享接口、枚举、属性持有器、转换器。**不包含 UI 控件**。
- **Base 层**（`AtomUI.Controls`）：定义设备无关的抽象基类控件（如 `AbstractTag`、`AbstractAvatar`）。包含共享行为、属性、伪类和逻辑。**不定义 Token 或 Theme**。
- **Platform 层**（`AtomUI.Desktop.Controls`）：定义平台特定的具体控件（如 `Tag`、`Avatar`）。包含 Token、Theme、平台特定交互。

### 4. 反冗余

- 已存在的工具函数、转换器、共享属性 **必须复用**，禁止重复定义。
- 共享属性 **必须** 使用 `AddOwner<T>()` 模式，而非重新注册。
- 伪类字符串 **必须** 先检查 `StdPseudoClass` 常量。
- Token 值 **必须** 从 `SharedToken` 派生，禁止硬编码。

### 5. 向后兼容

- 禁止移除或重命名已有的公共 API 成员。
- 弃用成员使用 `[Obsolete]` 注解并提供迁移指引。
- 新增 `StyledProperty` 必须提供合理默认值。

---

## 控件组成（Component Composition）

一个完整的 AtomUI 桌面控件由以下部分组成：

```
AtomUI.Controls/                         ← Base 层
└── ControlName/
    ├── AbstractControlName.cs           ← 抽象基类（行为、属性、逻辑）
    ├── ControlNameEnums.cs              ← 共享枚举
    └── ControlNamePseudoClass.cs        ← 共享伪类常量

AtomUI.Desktop.Controls/                 ← Platform 层
└── ControlName/
    ├── ControlName.cs                   ← 具体控件类（继承抽象基类，注册 Token 作用域）
    ├── ControlNameToken.cs              ← 组件 Token 定义
    └── Themes/
        ├── ControlNameTheme.axaml       ← ControlTheme XAML 模板
        ├── ControlNameTheme.cs          ← Theme 代码后台
        └── ControlNameThemes.axaml      ← ResourceDictionary 注册
```

> **判断依据**：编写代码时始终自问 *"未来的 Mobile 版本是否会共享这段行为？"* 如果是 → 放入 `AtomUI.Controls` 基类；如果否 → 放入 `AtomUI.Desktop.Controls` 具体类。

---

## 快速检查清单

新建控件时，逐项核对以下要求：

- [ ] 属性使用正确的 Avalonia 属性类型（参见 [PropertyDefinitionStandards](./PropertyDefinitionStandards.md)）
- [ ] 控件继承正确的基类，实现所有必要接口（参见 [ControlClassStructure](./ControlClassStructure.md)）
- [ ] Token 从 SharedToken 派生，无硬编码值（参见 [TokenSystemIntegration](./TokenSystemIntegration.md)）
- [ ] Theme 文件夹结构完整，使用 Token 资源引用（参见 [ThemeFileStructure](./ThemeFileStructure.md)）
- [ ] 伪类正确定义并使用 `[PseudoClasses]` 注解（参见 [PseudoClassConventions](./PseudoClassConventions.md)）
- [ ] 事件与生命周期管理正确（参见 [EventsAndLifecycle](./EventsAndLifecycle.md)）
- [ ] 所有公共 API 成员有 XML 文档注释
- [ ] 用户可见文本通过 `LanguageProvider` 本地化
- [ ] 在 Gallery ShowCase 中添加示例


# AtomUI — 基于 Avalonia / .NET 的 Ant Design 实现

AtomUI 借助 Avalonia 强大的跨平台能力，为 .NET 实现了完整的 Ant Design 设计体系，致力于将其精炼的设计语言和高效的用户体验带入跨平台桌面应用开发领域。

---

## AtomUI 是什么？

[Ant Design](https://ant.design/) 是由蚂蚁集团推出的企业级设计体系。它将复杂的业务场景提炼为一套完整的设计价值观——**自然**、**确定**、**意义**、**生长**——并配以全面的视觉规范、交互模式和经过实践验证的组件库。自诞生以来，Ant Design 已成为业界应用最广泛的 UI 设计语言之一，为阿里巴巴、腾讯、百度、美团及全球数千家企业和组织的产品提供支撑。

AtomUI 将这一设计体系——特别是 [Ant Design 5.0](https://ant.design/docs/react/introduce) 规范——通过跨平台 .NET UI 框架 [Avalonia UI](https://avaloniaui.net/) 引入 .NET 桌面开发领域。这不是简单的视觉模仿；AtomUI 是对 Ant Design 组件行为、Design Token 架构、主题算法、动效系统和图标集的严格、规范级复刻，完全使用 C# 和 AXAML 从零构建。

AtomUI 已在 [Ant Design 规范页面](https://ant.design/docs/spec/introduce#front-end-implementation)上被官方收录为社区实现。

### 概览

- **60+ 个控件**，涵盖通用、布局、导航、数据录入、数据展示和反馈六大类别——与 Ant Design 组件分类体系完全对齐。
- **完整的 Design Token 系统** — 四层 Token 推导链（Seed → Map → Alias → Component），与 Ant Design 5.0 TypeScript 实现功能等价。
- **主题算法** — Default（亮色）、Dark（暗色）和 Compact（紧凑）三种算法，可自由组合。
- **运行时主题切换** — 运行时切换主题，无需重启应用。
- **Ant Design 图标集** — 完整的 Ant Design 图标库，从 SVG 源自动生成。
- **国际化** — 内置本地化系统，开箱即支持英文和中文。
- **跨平台** — 单一代码库即可支持 Windows、macOS 和 Linux。

---

## 为什么选择 AtomUI？

### 将成熟的设计语言引入桌面端

Web 开发生态长期以来拥有 Ant Design、Material Design、Fluent UI 等久经验证的设计体系。然而，.NET 桌面应用开发领域一直缺乏对等的方案。开发者要么采用各操作系统各异的平台原生风格，要么投入大量精力进行自定义设计。

AtomUI 填补了这一空白。通过在 Avalonia 上实现 Ant Design 5.0 规范，它为 .NET 桌面开发者提供：

- **成熟且系统化的设计语言** — 无需从零发明间距体系、调色板或交互模式。Ant Design 积累多年的企业级产品设计精髓直接内置于控件之中。
- **跨平台视觉一致性** — 同一应用在 Windows、macOS 和 Linux 上呈现完全一致的 UI，像素级忠实于 Ant Design 规范。
- **熟悉的开发体验** — 曾在 Web 端使用过 Ant Design 的开发者，将在 AtomUI 中找到相同的组件名称、属性语义、Token 命名和主题定制概念。

### 基于 Avalonia 构建

[Avalonia](https://avaloniaui.net/) 是一个跨平台 .NET UI 框架，拥有高性能渲染引擎、强大的样式/模板系统和基于 XAML 的声明式 UI。AtomUI 构建在 Avalonia 之上，而非取代它：

- AtomUI 控件是标准的 Avalonia `Control` / `TemplatedControl` 子类，与 Avalonia 的样式、绑定和布局系统完全兼容。
- 已有的 Avalonia 知识可直接迁移——开发者继续使用 XAML、`StyledProperty`、`ControlTheme` 以及熟悉的 Avalonia DevTools。
- AtomUI 可以与原生 Avalonia 控件及第三方库在同一应用中共存。

### 提升开发效率

AtomUI 的设计目标是减少样板代码、最大化产出：

- **开箱即用** — 安装 NuGet 包，调用 `UseAtomUI()`，即可开始构建。默认体验无需手动配置样式表或主题文件。
- **一致的 API** — 共享接口（`ISizeTypeAware`、`IInputControlStyleVariantAware`、`IInputControlStatusAware`）确保所有控件具有统一的属性名称和行为规范。
- **Design Token 定制** — 只需覆盖一个 Seed Token（如 `ColorPrimary`），变更即自动级联到所有控件，无需逐个修改组件样式。
- **Form 集成** — Form 系统自动向子控件传播 `SizeType`、`StyleVariant` 和验证状态，省去重复的属性设置。

---

## 设计理念与原则

### 规范严格遵循

AtomUI 以 Ant Design 5.0 规范作为唯一真实来源。每个控件的视觉外观、交互行为、间距、尺寸和动效均源自规范，而非近似模仿。以 Ant Design React 实现（`antd`）为参照，AtomUI 复刻了以下各项：

- **组件分类体系** — 通用、布局、导航、数据录入、数据展示、反馈。
- **尺寸系统** — 三级尺寸（Large / Middle / Small），每级的高度、内边距、字号和圆角半径均由 Token 驱动。
- **样式变体** — 输入类控件支持 Outlined（描边）、Filled（填充）、Borderless（无边框）以及 Underlined（下划线，AtomUI 扩展）。
- **动效系统** — Fade（渐变）、Slide（滑动）、Zoom（缩放）、Collapse（折叠）和 Move（移动）动画预设，支持全局启用/禁用开关。
- **色彩系统** — 基于 HSV 的 10 色调色板生成算法，与 Ant Design 的 `@ant-design/colors` 算法一致。

### Design Token 架构

主题系统是 AtomUI 视觉一致性的基石。它严格实现了 Ant Design 5.0 的四层 Design Token 架构：

```
Seed Token ──(算法)──► Map Token ──(别名计算)──► Alias Token ──(派生)──► Component Token
  (~20 个)               (~100+ 个)                (~100+ 个)              (每个组件独有)
```

| 层级 | 用途 | 示例 |
|---|---|---|
| **Seed Token** | 设计意图 — 定义品牌的最小值集合 | `ColorPrimary`、`FontSize`、`BorderRadius` |
| **Map Token** | 算法派生的梯度值 — 调色板、尺寸阶梯、字号阶梯 | `ColorPrimaryHover`、`FontSizeLG`、`ControlHeightSM` |
| **Alias Token** | 供组件批量消费的语义化 Token | `ColorTextDisabled`、`ColorBgContainer`、`PaddingContentHorizontal` |
| **Component Token** | 按组件 ID 隔离的组件级覆盖 | `ButtonToken.PrimaryColor`、`TagToken.DefaultBg` |

修改一个 Seed Token（例如将 `ColorPrimary` 设为品牌色），主题算法将自动重新计算所有派生 Token——覆盖全部组件，实时生效。

### 主题算法

三种可组合的算法将 Seed Token 展开为完整的 Map Token 集合：

| 算法 | 效果 |
|---|---|
| **Default** | 标准亮色主题 |
| **Dark** | 暗色主题，反转亮度曲线 |
| **Compact** | 缩减间距和尺寸，适用于信息密集型界面 |

算法可自由组合——例如 Dark + Compact 可生成暗色紧凑主题。运行时切换算法将触发全量 Token 重新计算并实时更新 UI。

---

## 技术架构

AtomUI 采用三层平台感知架构，以最大化代码复用：

```
┌───────────────────────────────────────────────────────────┐
│  平台控件层 (Platform Control Layer)                        │
│  AtomUI.Desktop.Controls (+ .ColorPicker, .DataGrid)      │
│  桌面平台的具体控件实现，包含平台特定的主题和 Token          │
├───────────────────────────────────────────────────────────┤
│  基础控件层 (Base Control Layer)                            │
│  AtomUI.Controls                                           │
│  与设备无关的抽象控件（共享行为）                             │
├───────────────────────────────────────────────────────────┤
│  基础设施层 (Foundation Layer)                              │
│  AtomUI.Core — 主题、Token、动效、动画、国际化               │
│  AtomUI.Controls.Shared — 接口、枚举、转换器                │
│  AtomUI.Native — 平台原生交互                               │
├───────────────────────────────────────────────────────────┤
│  共享资源                                                   │
│  AtomUI.Icons.AntDesign — Ant Design 图标集                 │
│  AtomUI.Fonts.AlibabaSans — 阿里巴巴普惠体字体包            │
│  AtomUI.Generator — Roslyn 源代码生成器                     │
└───────────────────────────────────────────────────────────┘
```

- **基础设施层**（`AtomUI.Core`、`AtomUI.Controls.Shared`、`AtomUI.Native`）— 平台无关的基础设施，不依赖任何 UI 控件。包含完整的主题/Token 系统、动画引擎、本地化框架和共享接口。
- **基础控件层**（`AtomUI.Controls`）— 与设备无关的抽象控件，定义共享行为、属性和伪类。作为各平台具体实现的基类。
- **平台控件层**（`AtomUI.Desktop.Controls`）— 面向桌面平台的具体控件实现。每个控件注册其组件 Token 作用域和桌面特定主题。扩展包（`ColorPicker`、`DataGrid`）可按需引入。

这种分层设计确保共享逻辑集中在基础层，而平台特定的主题和交互模式被清晰分离——未来可扩展至其他平台（如移动端），而无需重复核心行为。

### 关键技术要点

| 领域 | 实现 |
|---|---|
| **运行时** | .NET 10（开发期）/ .NET 8（生产环境）；多目标构建 |
| **UI 框架** | Avalonia 11.3.x |
| **开发语言** | C#（最新版本，nullable 已启用） |
| **主题绑定** | 自定义 `SharedTokenResource` / `TokenResource` XAML 标记扩展 |
| **代码生成** | Roslyn Source Generator，自动生成 Token ResourceKey 枚举和 Language ResourceKey 枚举 |
| **图标系统** | 800+ 个 Ant Design 图标，通过 `AtomUI.Icons.AntDesign.Generator` 从 SVG 自动生成 |
| **字体** | 内置阿里巴巴普惠体，由 `AtomUI.Fonts.AlibabaSans` 提供 |
| **响应式编程** | ReactiveUI.Avalonia + System.Reactive |

---

## 组件覆盖

AtomUI 实现了一整套 Ant Design 5.0 组件，按照 Ant Design 文档中相同的分类方式组织：

### 通用
Button（按钮）、SplitButton（分割按钮）、FloatButton（悬浮按钮）、Icon（图标）、Separator（分隔符）

### 布局
Space（间距）、CompactSpace（紧凑间距）、Grid（栅格）、FlexPanel（弹性面板）、BoxPanel（盒面板）、Splitter（分割器）

### 导航
Menu（菜单）、Breadcrumb（面包屑）、Dropdown（下拉菜单）、Pagination（分页）、Steps（步骤条）、TabControl（标签页）

### 数据录入
AutoComplete（自动完成）、Cascader（级联选择）、CheckBox（多选框）、ColorPicker（颜色选择器）、DatePicker（日期选择器）、RangeDatePicker（日期范围选择器）、TimePicker（时间选择器）、RangeTimePicker（时间范围选择器）、Form（表单）、LineEdit（输入框）、TextArea（文本域）、Mentions（提及）、NumericUpDown（数字输入框）、RadioButton（单选框）、Rate（评分）、Select（选择器）、Slider（滑动条）、Switch（开关）、Transfer（穿梭框）、TreeSelect（树选择）、Upload（上传）

### 数据展示
Avatar（头像）、Badge（徽标数）、Calendar（日历）、Card（卡片）、Carousel（走马灯）、Collapse（折叠面板）、DataGrid（数据表格）、Descriptions（描述列表）、Empty（空状态）、Expander（展开器）、GroupBox（分组框）、ImagePreviewer（图片预览）、List（列表）、InfoFlyout（气泡卡片）、QRCode（二维码）、Segmented（分段控制器）、Statistic（统计数值）、Tag（标签）、Timeline（时间轴）、Tooltip（文字提示）、Tour（漫游式引导）、TreeView（树形控件）

### 反馈
Alert（警告提示）、Drawer（抽屉）、Message（全局提示）、Modal（对话框）、Notification（通知提醒）、PopupConfirm（气泡确认框）、ProgressBar（进度条）、CircleProgress（环形进度）、Result（结果）、Skeleton（骨架屏）、Spin（加载中）、Watermark（水印）

---

## 与 Ant Design 生态的关系

AtomUI 是面向 Avalonia / .NET 平台的 Ant Design 规范**社区实现**。它在 .NET 桌面开发领域所扮演的角色，与 `antd`（React）、`Ant Design Vue`（Vue）、`NG-ZORRO`（Angular）和 `Ant Design Blazor`（Blazor）在各自生态中的角色相同。

| 技术生态 | 实现 | 平台 |
|---|---|---|
| React | `antd`（官方） | Web |
| Vue | Ant Design Vue | Web |
| Angular | NG-ZORRO | Web |
| Blazor | Ant Design Blazor | Web (.NET) |
| **Avalonia / .NET** | **AtomUI** | **桌面端 (Win/Mac/Linux)** |

AtomUI 对齐 Ant Design **5.0**，实现了该大版本引入的 Design Token 系统、主题算法和组件 API 语义。

---

## 快速开始

### 安装

```bash
dotnet add package AtomUI
```

或按需安装各独立包，实现更精细的控制：

| 包名 | 描述 |
|---|---|
| `AtomUI.Core` | 核心基础设施 — 主题系统、Token 系统、动画 |
| `AtomUI.Controls.Shared` | 面向控件开发的共享接口与枚举 |
| `AtomUI.Desktop.Controls` | 桌面控件库 — 主要安装包 |
| `AtomUI.Desktop.Controls.DataGrid` | DataGrid 数据表格控件（按需引入） |
| `AtomUI.Desktop.Controls.ColorPicker` | ColorPicker 颜色选择器控件（按需引入） |
| `AtomUI.Fonts.AlibabaSans` | 阿里巴巴普惠体字体包 |
| `AtomUI.Generator` | 面向自定义控件开发的源代码生成器 |

### 启用

```csharp
public partial class App : Application
{
    public override void Initialize()
    {
        base.Initialize();
        AvaloniaXamlLoader.Load(this);
        this.UseAtomUI(builder =>
        {
            builder.WithDefaultTheme(IThemeManager.DEFAULT_THEME_ID);
            builder.UseAlibabaSansFont();
            builder.UseDesktopControls();
            builder.UseDesktopDataGrid();      // 可选
            builder.UseDesktopColorPicker();   // 可选
        });
    }
}
```

### 使用

```xml
<atom:Window xmlns="https://github.com/avaloniaui"
             xmlns:atom="https://atomui.net"
             xmlns:antdicons="https://atomui.net/icons/antdesign">
    <atom:Space Orientation="Horizontal">
        <atom:Button ButtonType="Primary">开始使用</atom:Button>
        <atom:Button Icon="{antdicons:AntDesignIconProvider StarOutlined}">在 GitHub 上点赞</atom:Button>
    </atom:Space>
</atom:Window>
```

### 探索

运行内置的控件展示厅，查看所有控件的实际效果：

```bash
git clone https://github.com/AtomUI/AtomUI.git
cd AtomUI
dotnet run --project controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj
```

或访问 [AtomUI.Samples](https://github.com/AtomUI/AtomUI.Samples) 获取最小化的入门示例项目。

---

## 许可证

AtomUI 基于 **LGPL v3** 许可证发布。通过二进制链接方式使用 AtomUI 的商业应用免费。源码级修改需将修改部分开源或购买商业许可。如需商业授权，请联系 [北京秦派软件科技有限公司](https://github.com/AtomUI/AtomUI)。


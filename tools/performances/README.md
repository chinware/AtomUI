# AtomUI Performance Tools

本目录存放 AtomUI 性能测量和验证工具。工具分为两类，数据口径不能混用。

## 工具分类

| 工具 | 类型 | 用途 | 数据口径 |
| --- | --- | --- | --- |
| [AtomUI.Performance](AtomUI.Performance/AtomUI.Performance.csproj) | 控件级基准 | 批量创建单个控件或小组合，观测实例化、布局、visual tree、分配和专项行为验证 | 微基准，不代表 Gallery 页面打开体验 |
| [AtomUI.GalleryPerformance](AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj) | Gallery 场景复现 | 启动 Gallery Workspace，走真实 route/view/XAML，测量 showcase 从导航触发到视觉树和布局稳定 | 体验路径基准，必须复现 Gallery 真实使用方式 |

## AtomUI.Performance 结构

`AtomUI.Performance` 按职责和控件体系拆分：

- `Program.cs`: 入口、参数解析后的调度，不放具体控件创建和验证逻辑。
- `Core/`: runner、结果模型、树统计、Markdown/table 渲染。
- `Suites/AddOnDecoratedBox/`: AddOnDecoratedBox、LineEdit、TextArea、SearchEdit、CompactSpace 相关场景和专项验证。
- `Suites/Button/`: Button、DropdownButton、SplitButton 相关场景和状态/模板验证。
- `Suites/ButtonSpinner/`: ButtonSpinner、Gallery shape batch、NumericUpDown 联动基线。
- `Suites/Drawer/`: Drawer 关闭态、extra/footer、嵌套场景基线。
- `Suites/GroupBox/`: GroupBox header icon lazy、Gallery shape 和状态/生命周期验证。
- `Suites/Icon/`: Icon micro benchmark、隐藏 icon slot、AntDesign metadata、provider cache 验证。
- `Suites/ImagePreviewer/`: ImagePreviewer 关闭态 source list lazy、Gallery shape 和状态/生命周期验证。
- `Suites/NavMenu/`: NavMenu/NavMenuItem 默认路径、全局关闭订阅、container binding 生命周期验证。
- `Suites/ScrollViewer/`: ScrollViewer/ScrollBar 模板、overlay host、lite/auto-hide 和 motion 状态验证。
- `TestSupport/`: 断言、测试 brush、marker template、probe icon 等测试辅助类型。

新增控件级基准时优先在 `Suites/<ControlName>/` 下建文件；跨控件复用能力放到 `Core/` 或 `TestSupport/`。

## 使用原则

- 控件优化先用 `AtomUI.Performance` 建立低噪声控件级基线，定位单实例和批量实例化成本。
- Gallery 体验结论必须用 `AtomUI.GalleryPerformance` 或同类 Gallery 场景工具确认。
- Gallery 场景工具必须加载真实 Gallery XAML，不能用合成控件替代真实 showcase。
- Gallery 场景工具需要在结果里输出源 XAML 形态和运行时视觉树形态，用来证明测量对象一致。
- `Cold first navigation` 必须用 `--cold-iterations <N>` 做独立进程多样本统计；单样本只用于 smoke，不能作为优化提升或回退结论。
- 文档中必须说明触发点、是否包含鼠标事件、是否包含 GPU 上屏、是否是 headless。

## 当前命令

控件级 AddOnDecoratedBox/LineEdit 基准：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --count 60 \
  --markdown /tmp/atomui-addon-control.md
```

控件级 Icon 基准：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite icon --count 60 \
  --markdown /tmp/icon-micro-baseline.md
```

控件级 Button 基准：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite button --count 60 \
  --markdown /tmp/atomui-button-control.md
```

Icon / AddOn 专项行为验证：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --verify-accessories --verify-effective-brushes --verify-addon-states \
  --verify-antdesign-metadata --verify-icon-hidden-slots --verify-icon-provider-cache
```

Button 状态和模板验证：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --verify-button-states
```

ButtonSpinner 状态和生命周期验证：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --verify-buttonspinner-states
```

控件级 ButtonSpinner 基准：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite buttonspinner --count 60 \
  --markdown /tmp/atomui-buttonspinner-control.md
```

控件级 Breadcrumb 基准与状态验证：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite breadcrumb --count 60 \
  --markdown /tmp/atomui-breadcrumb-control.md

dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --verify-breadcrumb-states
```

控件级 NavMenu 基准与状态验证：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite navmenu --count 80 \
  --markdown /tmp/atomui-navmenu-control.md

dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --verify-navmenu-states
```

控件级 Drawer 基准：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite drawer --count 60 \
  --markdown /tmp/drawer-control-baseline.md
```

控件级 ImagePreviewer 基准：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite imagepreviewer --count 60 \
  --markdown /tmp/atomui-imagepreviewer-control.md
```

ImagePreviewer 状态和生命周期验证：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --verify-imagepreviewer-states
```

控件级 ScrollViewer 基准与状态验证：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --suite scrollviewer --count 220 \
  --markdown /tmp/atomui-scrollviewer-control.md

dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --verify-scrollviewer-states
```

Gallery 真实 `LineEditShowCase` 导航基准：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase lineedit --label optimized --cold-iterations 10 --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/lineedit-showcase-navigation-optimized.md
```

Gallery 真实 `IconShowCase` 导航基准：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase icon --label icon-baseline --cold-iterations 10 \
  --iterations 10 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/icon-showcase-navigation-baseline.md
```

Gallery 真实 `ImagePreviewerShowCase` 导航基准：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase imagepreviewer --label imagepreviewer \
  --cold-iterations 10 --iterations 40 --warmup 6 --timeout-ms 45000 \
  --markdown /tmp/atomui-imagepreviewer-gallery.md
```

Gallery 工具支持多个真实 ShowCase，常用例子包括 `lineedit`、`icon`、`button`、`space`、`select`、`cascader`、`drawer`、`groupbox`、`imagepreviewer`、`menu`、`breadcrumb`。`menu` 的 ready 条件包含 `NavMenu` 默认路径完整展开，避免把未完全显示的 `791 visuals` 形态误判为完成。例如验证 Icon Phase 4 影响到的控件真实场景：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --showcase select --label phase4-icon-slots \
  --iterations 10 --warmup 2 --timeout-ms 30000 \
  --markdown /tmp/select-showcase-navigation-phase4.md
```

## 扩展建议

- 新控件先补控件级 scenario，再补对应 Gallery showcase scenario。
- 多个控件共享的基础能力，例如 addon、popup、selector、虚拟化，应优先在控件级工具里做小样本和批量样本对照。
- 对用户可感知的打开、切换、弹出、滚动路径，应在 Gallery 工具里用真实页面复现。

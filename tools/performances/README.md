# AtomUI Performance Tools

本目录存放 AtomUI 性能测量和验证工具。工具分为两类，数据口径不能混用。

## 工具分类

| 工具 | 类型 | 用途 | 数据口径 |
| --- | --- | --- | --- |
| [AtomUI.Performance](AtomUI.Performance/AtomUI.Performance.csproj) | 控件级基准 | 批量创建单个控件或小组合，观测实例化、布局、visual tree、分配和专项行为验证 | 微基准，不代表 Gallery 页面打开体验 |
| [AtomUI.GalleryPerformance](AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj) | Gallery 场景复现 | 启动 Gallery Workspace，走真实 route/view/XAML，测量 showcase 从导航触发到视觉树和布局稳定 | 体验路径基准，必须复现 Gallery 真实使用方式 |

## 使用原则

- 控件优化先用 `AtomUI.Performance` 建立低噪声控件级基线，定位单实例和批量实例化成本。
- Gallery 体验结论必须用 `AtomUI.GalleryPerformance` 或同类 Gallery 场景工具确认。
- Gallery 场景工具必须加载真实 Gallery XAML，不能用合成控件替代真实 showcase。
- Gallery 场景工具需要在结果里输出源 XAML 形态和运行时视觉树形态，用来证明测量对象一致。
- 文档中必须说明触发点、是否包含鼠标事件、是否包含 GPU 上屏、是否是 headless。

## 当前命令

控件级 AddOnDecoratedBox/LineEdit 基准：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --count 60 \
  --markdown docs/performances/AddOnDecoratedBox/addon-decorated-box-final.md
```

Gallery 真实 `LineEditShowCase` 导航基准：

```bash
dotnet run --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --label optimized --iterations 30 --warmup 5 --timeout-ms 30000 \
  --markdown /tmp/lineedit-showcase-navigation-optimized.md
```

## 扩展建议

- 新控件先补控件级 scenario，再补对应 Gallery showcase scenario。
- 多个控件共享的基础能力，例如 addon、popup、selector、虚拟化，应优先在控件级工具里做小样本和批量样本对照。
- 对用户可感知的打开、切换、弹出、滚动路径，应在 Gallery 工具里用真实页面复现。

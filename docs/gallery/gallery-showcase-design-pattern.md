# Gallery ShowCase Design Pattern

本文档总结 ButtonShowCase 本轮改造，并沉淀为后续控件 ShowCase 页面的设计范式。目标是让 Gallery 的控件介绍页同时满足文档阅读、示例浏览、API 查询和 Design Token 查询四类任务。

## ButtonShowCase 改造总结

ButtonShowCase 从“单一 ShowCasePanel 承载全部内容”升级为“文档页头 + sticky 场景导航 + 延迟加载内容”的结构。

当前页面主体由三段组成：

1. Header：控件标题、分类和状态 Tag、简介、基础信息卡片。
2. StickyContent：`TabStrip` 场景导航，包含 `Examples`、`API`、`Design Token`。
3. Content：`ScenarioContentHost`，根据当前 Tab 显示示例瀑布流、API DataGrid 或 Design Token DataGrid。

核心结构如下：

```xml
<gallery:GalleryStickyTabsHost StickyContentPadding="28,0,28,0">
    <gallery:GalleryStickyTabsHost.Header>
        <!-- title, tags, description, metadata -->
    </gallery:GalleryStickyTabsHost.Header>

    <gallery:GalleryStickyTabsHost.StickyContent>
        <atom:TabStrip Name="ScenarioTabs" SizeType="Large" SelectedIndex="0">
            <atom:TabStripItem Content="Examples" Tag="Examples" />
            <atom:TabStripItem Content="API" Tag="Api" />
            <atom:TabStripItem Content="Design Token" Tag="DesignToken" />
        </atom:TabStrip>
    </gallery:GalleryStickyTabsHost.StickyContent>

    <ContentControl Name="ScenarioContentHost">
        <gallery:ShowCasePanel Name="ExamplesContent"
                               IsScrollEnabled="False">
            <!-- ShowCaseItem demo content -->
        </gallery:ShowCasePanel>
    </ContentControl>
</gallery:GalleryStickyTabsHost>
```

这次改造解决了几个主要问题：

- Header 不再固定占用视口，用户向下滚动时 Header 会自然离开。
- Tab 到达顶部后固定，用户在长示例页、API 页、Token 页之间切换时不会丢失导航。
- Examples 继续使用卡片瀑布流，保留控件示例的浏览节奏。
- API 和 Design Token 改为独立 DataGrid，并保持首次切换时延迟加载。
- Button 的具体演示内容被测试快照保护，改造只允许移动容器，不允许改变 `ShowCaseItem` 内部控件演示。

## 设计范式

一个标准控件 ShowCase 页面应采用下面的页面模型：

```text
ShowCase Page
  GalleryStickyTabsHost
    Header
      Title Row
      Summary
      Metadata
    StickyContent
      TabStrip
    Content
      ScenarioContentHost
        ExamplesContent
        Lazy API Content
        Lazy Design Token Content
```

这个模型的关键原则是：页面像文档一样滚动，场景导航像工具条一样保持可达，示例内容像 Gallery 一样可浏览，表格内容像参考文档一样可查询。

## Header 范式

Header 负责让用户快速理解这个控件是什么、处于什么状态、来自哪个包。

推荐内容：

- 控件名，例如 `Button`。
- 控件类别 Tag，例如 `General`、`Data Entry`、`Data Display`。
- 稳定性或状态 Tag，例如 `Stable`、`Preview`。
- 一句话 subtitle，说明控件用途。
- 一段 description，说明主要使用场景。
- 一块紧凑 metadata 信息，例如 namespace、package、base class。

Header 不应放：

- 大量营销文案。
- 复杂交互示例。
- 可替代 Examples 的演示内容。
- 会让首屏过高的长篇 Vision/Mission 类内容。

Header 的布局要求：

- 标题和描述左对齐。
- 信息区允许横向排列，也要支持 WrapPanel 换行。
- 中文和英文环境都要避免挤压、错位和不可读。
- metadata 的 label 可以固定窄宽度，value 可以固定合理宽度并使用省略。

## Tabs 范式

ShowCase 页面的一级场景导航使用 `TabStrip`，不使用 `TabControl`。

原因：

- `TabControl` 同时管理 Tab 头和内容，不适合 sticky header 场景。
- `TabStrip + ContentControl` 可以让 sticky host 只固定导航，内容由页面自己控制。
- API 和 Design Token 可以真正延迟创建，避免页面初始加载时构建表格。

推荐 Tab：

- `Examples`：控件交互示例。
- `API`：属性、事件、方法等参考信息。
- `Design Token`：控件相关 token。

可选 Tab：

- `Usage`：只有当控件有明显复杂业务用法时才增加。
- `Accessibility`：只有当控件有专门的可访问性说明时才增加。
- `Theming`：只有当控件主题定制非常复杂时才增加。

不推荐：

- 第一个 Tab 命名为 `Gallery`。这里展示的是 examples，不是 Gallery 自身。
- 为了排版把示例拆成过多一级 Tab。
- 让 Tab 内容和 Header 各自拥有独立滚动条。

## Examples 范式

Examples 仍然使用 `ShowCasePanel + ShowCaseItem`。

页面级滚动场景下，Examples 的 `ShowCasePanel` 必须设置：

```xml
<gallery:ShowCasePanel IsScrollEnabled="False">
```

这样滚动权交给 `GalleryStickyTabsHost`，避免 Header 固定、内外滚动条冲突、滚动条位置异常。

Examples 内容组织规则：

- 使用瀑布流卡片承载独立示例。
- 每个 `ShowCaseItem` 只说明一个明确能力。
- 卡片内演示内容必须保持真实控件形态，不为了版式重写示例。
- 如果某个示例需要占满整行，使用 `ShowCaseItem` 的整行能力，而不是在页面层硬编码特殊布局。
- 不把多个不相关能力合并进一个大卡片。
- 不把 Examples 改成固定的四大区块，除非控件本身确实只有四个线性主题。

硬边界：

- 改造页面结构时，不允许改动 `ShowCaseItem` 内部演示内容。
- 控件演示内容变更必须是独立需求，并有单独 review。
- 对已整理过的页面，应保留或新增 snapshot 测试，确保示例内容不会被布局改造误改。

## API 与 Design Token 范式

API 和 Design Token 应使用独立 UserControl 承载 DataGrid。

推荐结构：

```text
ButtonShowCase.axaml
  ScenarioContentHost

ButtonShowCase.axaml.cs
  ResolveScenarioContent("Api") -> new ButtonApiDataGrid()
  ResolveScenarioContent("DesignToken") -> new ButtonDesignTokenDataGrid()

ButtonApiDataGrid.axaml
ButtonApiDataGrid.axaml.cs
ButtonDesignTokenDataGrid.axaml
ButtonDesignTokenDataGrid.axaml.cs
```

DataGrid 规则：

- 首次切换到对应 Tab 时创建。
- 创建后缓存，避免重复构建。
- `DataContext` 变化时同步到已创建内容。
- 第一列用于 key/name，应固定。
- 描述列/说明列应使用 `Width="*"` 承接剩余宽度；只设置合理 `MinWidth`，不要为了当前窗口宽度写死大像素值。
- 表格自身处理横向滚动，避免把横向滚动泄漏到页面底部。
- 行线、hover、transition 等视觉表现应来自 DataGrid 控件能力或 token，不在 ShowCase 页里复制一套表格。

不推荐：

- 在主 ShowCase XAML 里直接写 API 表格。
- 页面初始化时直接创建所有 DataGrid。
- 用零散 Border/Grid 模拟表格。
- 为 API/Token 使用与 Examples 相同的卡片瀑布流。

## Sticky Host 范式

`GalleryStickyTabsHost` 是 Gallery 专用页面宿主，不是通用 Window 或 NavMenu 改造。

职责：

- 提供页面级 ScrollViewer。
- 让 Header、StickyContent、Content 处于同一个滚动上下文。
- 当 StickyContent 滚动到顶部后，保持在可视区域顶部。
- 使用 Gallery 自己的 token 定义背景、边线和 padding。

实现边界：

- sticky 行为由 `GalleryStickyTabsPanel` arrange 单个 sticky 子项实现。
- 真实 StickyContent 必须始终保留在模板里的 inline presenter，不允许移动到 Popup、OverlayLayer 或其他 presenter。
- 不允许通过清空 inline presenter 再把同一个 TabStrip 交给 overlay 的方式实现吸顶；这会导致 Tabs 消失或视觉树父级冲突。
- Badge、Ribbon 等窗口级 Adorner 可能压过普通 sticky 内容时，只允许使用只读的 overlay mirror：在高层绘制 inline StickyContent 的视觉镜像，不能承载真实 TabStrip，且必须 `IsHitTestVisible=false`。
- 不在滚动时创建或销毁真实 TabStrip 视觉树。
- 不在 Content 外层包局部 `VisualLayerManager`，避免 Badge、Ribbon 等基于窗口级 `AdornerLayer` 的控件丢失装饰层。
- sticky pinned 后，视觉镜像必须位于高于窗口级 `AdornerLayer` 的 overlay 层；`GalleryStickyTabsPanel` 不允许裁剪后续 Content，避免深滚动、Tab 切换或重排后出现大面积空白。遮挡关系由真实 sticky 行的背景、ZIndex 和只读 overlay mirror 负责。
- `ScrollChanged` 订阅必须在 detach 时释放。

Token 规则：

- 新增 Gallery 自定义控件必须有对应 token 类。
- 主题文件中通过 `Gallery...TokenResource` 使用 token。
- 主题必须加入 `GalleryControlThemesProvider`。
- 需要让生成器产出对应 token kind 和 resource extension。

## ShowCasePanel 范式

`ShowCasePanel` 的默认行为仍然是自带内部滚动，保证老页面不受影响。

对于新文档式 ShowCase 页面：

```xml
<gallery:ShowCasePanel IsScrollEnabled="False" />
```

适用规则：

- 独立旧页面或未迁移页面：保留默认 `IsScrollEnabled=True`。
- 新文档式页面：关闭内部滚动，使用 `GalleryStickyTabsHost` 的页面级滚动。
- `ContentMargin` 由页面根据 Header/Tab 对齐关系设置，但左右应与 sticky Tab 主体一致。
- Masonry 布局参数使用 ShowCasePanel token，不在每个页面重复写死。

### Deferred Loading

示例数量较多或单个示例创建成本较高时，可以在 `ShowCasePanel` 上手动开启延迟加载：

```xml
<gallery:ShowCasePanel IsDeferredLoadingEnabled="True"
                       InitialDeferredLoadItemCount="4"
                       DeferredLoadBatchSize="2">
```

规则：

- 默认保持 `IsDeferredLoadingEnabled="False"`，未迁移页面和简单页面不受影响。
- `ShowCasePanel` 只允许使用一个 panel 级 `EffectiveViewportChanged` 监听来判断 viewport，不允许给每个 `ShowCaseItem` 单独挂监听。
- `InitialDeferredLoadItemCount` 控制首屏预先 materialize 的 item 数量。
- `DeferredLoadBatchSize` 控制每次滚动到 viewport 附近后 materialize 的批大小，避免一次性卡顿。
- `DeferredLoadViewportBuffer` 控制提前加载距离，保证用户滚到附近前内容已经准备好。
- 已 materialize 的内容不回收。ShowCase 是文档式页面，不做无限列表虚拟化，避免状态、焦点、Popup/Flyout 生命周期被破坏。
- 如果 `ShowCaseItem` 仍然直接写普通 Content，Avalonia 会在页面初始化时创建这些控件；这种情况下只能减少部分挂载/布局成本。要真正延迟创建演示控件，必须把演示内容放到 `DeferredContentTemplate` 中。

推荐写法：

```xml
<gallery:ShowCaseItem Title="..."
                      Description="..."
                      IsDeferredContentEnabled="True">
    <gallery:ShowCaseItem.DeferredContentTemplate>
        <DataTemplate>
            <!-- 原 ShowCaseItem 演示内容 -->
        </DataTemplate>
    </gallery:ShowCaseItem.DeferredContentTemplate>
</gallery:ShowCaseItem>
```

硬边界：

- 移入 `DeferredContentTemplate` 只能改变创建时机，不能改变演示控件内容。
- 已迁移页面必须保留 snapshot 测试，测试应剥离 deferred wrapper 后继续比对原始演示内容。
- placeholder 的高度和圆角走 `ShowCaseItemToken`，不在页面里写私有视觉值。

## Spacing 与对齐规则

ButtonShowCase 当前采用：

- Header 左右：`28px`。
- Sticky Tab 左右：`28px`。
- Examples content 左右：`28px`。
- Examples content 顶部：默认 `10px`。

原则：

- Header、Tab、Content 的左右边界必须一致。
- 滚动条应属于最外层页面滚动，不应被内容 padding 推离窗口右边。
- Tab 与下面内容的垂直间距要统一，不应该只在第一个 Tab 生效。
- DataGrid 页和 Examples 页的内容容器应共享同一套外边距策略。
- 三个 Tab 的内容容器必须使用同一套底部间距规则：底部都保留 `28px`，避免某个 Tab 的内容或横向滚动条贴住页面底边。
- Examples 必须使用 `ContentMargin="28,10,28,28"`。
- API 和 Design Token 的 lazy DataGrid 必须设置 `Margin="28,10,28,28"`，左右与 Examples 对齐，底部间距一致；尤其是出现横向滚动条时，滚动条下方不能贴住容器底边。
- Badge、Ribbon 等示例控件会通过 adorner 向上溢出时，Examples 可单独增加顶部留白，例如 Badge 使用 `ContentMargin="28,28,28,28"`，但左右边界和底部间距仍必须保持一致。

## ViewModel 与数据加载规则

ShowCase 主页面负责场景切换，不负责填满所有场景数据。

推荐模式：

```csharp
private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);

private Control ResolveScenarioContent(string scenario)
{
    if (scenario == ExamplesScenario)
    {
        return ExamplesContent;
    }

    if (!_lazyScenarioContentCache.TryGetValue(scenario, out var content))
    {
        content = CreateScenarioContent(scenario);
        content.DataContext = DataContext;
        _lazyScenarioContentCache.Add(scenario, content);
    }

    return content;
}
```

规则：

- Examples 可以在 XAML 中声明，因为它是默认首屏内容。
- API 和 Design Token 使用 lazy UserControl。
- `OnDataContextChanged` 必须同步已创建 lazy content。
- detach 时清理 lazy cache，避免持有旧视觉树和旧 DataContext。

## 测试范式

每个迁移后的 ShowCase 至少需要覆盖三类测试。

结构测试：

- 页面使用 `GalleryStickyTabsHost`。
- 页面使用 `TabStrip`，不使用 `TabControl`。
- 存在 `ScenarioContentHost`。
- Examples 的 `ShowCasePanel` 设置 `IsScrollEnabled=False`。
- API 和 Design Token 不在主 XAML 中直接声明 DataGrid。

延迟加载测试：

- code-behind 存在场景切换处理。
- API 和 Design Token 的 UserControl 在切换时 `new`。
- API 和 Design Token DataGrid 自己绑定数据源。

演示内容保护测试：

- 从第一个 `ShowCaseItem` 到 `ShowCasePanel` 结束提取示例内容。
- 与 approved snapshot 比较。
- 任何布局改造不应导致 snapshot 变化。

## 迁移检查清单

迁移其它控件 ShowCase 时按以下顺序处理：

1. 保留原 `ShowCaseItem` 内容，先建立 snapshot。
2. 增加 Header 区域，包括 title、tags、subtitle、description、metadata。
3. 用 `GalleryStickyTabsHost` 包裹页面主体。
4. 用 `TabStrip` 替代 `TabControl`。
5. 新增 `ScenarioContentHost`。
6. Examples 放入 `ShowCasePanel IsScrollEnabled=False`。
7. API 和 Design Token 拆成独立 UserControl。
8. code-behind 实现场景切换和 lazy cache。
9. 统一 Header、Tab、Content 左右边距。
10. 跑结构测试、snapshot 测试、Gallery Desktop 构建。

## 不做事项

为了避免再次把页面结构和示例内容耦合在一起，下面这些行为应避免：

- 在页面改造时顺手改控件演示内容。
- 修改全局 Window 或 NavMenu 来解决单个 ShowCase 页面布局问题。
- 为 sticky tabs 复制一份隐藏 TabStrip。
- 在 ShowCase 页面里手写模拟 DataGrid。
- 每个页面私有一套 sticky 或表格样式。
- 让 Header、Examples、API、Token 分别拥有不同滚动上下文。

## Button 之后的推广策略

Button 是范式样板，不建议一次性批量改所有控件。

推荐推广顺序：

1. 选择 1 个简单控件，验证范式可迁移性。
2. 选择 1 个复杂控件，验证 API/Token 和 Examples 的布局稳定性。
3. 把重复代码提炼成更明确的 Gallery 页面基类或辅助控件。
4. 再制定批量迁移清单。

只有当至少两个控件迁移后仍然保持结构稳定，才考虑进一步抽象，例如 `GalleryShowCaseDocumentPage`。

# Popup 性能优化

> 状态：本轮已完成 structural 阶段 + 微基准回填。

---

## 0. 微基准对比（控件级，60 instances/scenario）

工具：`tools/performances/AtomUI.Performance` 哈纳斯。
口径：`--suite popup --count 60`，同进程跑双侧。
注意：headless 环境无 OverlayLayer/IPopupImpl，IsOpen=true 跑不通；本套基准只覆盖 Popup shell + 内容树构造成本，正是本轮优化目标（构造器 token binding + ShadowsAwareContainer 影子 Border）。

| Scenario | ms/item baseline → optimized | Δ ms/item | KB/item baseline → optimized | Δ KB/item |
| --- | --- | --- | --- | --- |
| `Popup.Empty` | 0.154 → 0.107 | **−31 %** | 23.4 → 21.9 | **−6 %** |
| `Popup.WithTextChild` | 0.127 → 0.107 | **−16 %** | 23.3 → 21.8 | **−6 %** |
| `Popup.WithComposite` | 0.226 → 0.206 | **−9 %** | 38.1 → 36.6 | **−4 %** |
| `Popup.MotionDisabled` | 0.121 → 0.105 | **−13 %** | 23.3 → 21.4 | **−8 %** |

每个 popup 实例的构造路径：
- 4 条 dynamic resource binding 观察者 → 共享 ControlTheme Setter（活动器共享）
- ShadowsAwareContainer 影子 `Border` 默认未创建（仅在 `HasBoxShadow` 真为 true 时延迟创建）

测得到的稳定收益：**ms/item -9% 到 -31%**，**KB/item -4% 到 -8%**。Fan-in 19 时一次 Gallery 加载就消除约 76 条订阅。

---

## 1. 范围

`src/AtomUI.Desktop.Controls/Popup/` 下：

- `Popup` — fan-in 19，AtomUI 所有下拉/弹出场景的 shell。本轮把构造器内的 4 条 token binding 迁移到 `ControlTheme` Setter。
- `ShadowsAwareContainer` — 在 popup root 内承担阴影渲染。本轮把影子 Border 创建延迟到真正需要 (`HasBoxShadow == true`)。
- `PopupRoot` / `OverlayPopupHost` — 主题不变。
- `PopupReflectionExtensions` / `PopupUtils` — 不变。

---

## 2. 改动

### 2.1 `Popup()` 构造器：4 条 token binding 迁移到 ControlTheme Setter

**原实现**（每实例 4 条 binding + 3 条事件订阅）：

```csharp
public Popup()
{
    this.ConfigureMotionBindingStyle();   // → IsMotionEnabled token binding
    TokenResourceBinder.CreateTokenBinding(this, PopupRootShadowProperty,  PopupHostTokenKind.PopupRootShadow);
    TokenResourceBinder.CreateTokenBinding(this, OverlayHostShadowProperty, PopupHostTokenKind.OverlayHostShadow);
    TokenResourceBinder.CreateTokenBinding(this, MotionDurationProperty,    SharedTokenKind.MotionDurationMid);

    this.AddClosingEventHandler(HandlePopupClosing);
    Opened += HandlePopupOpened;
    Closed += HandlePopupClosed;
}
```

`TokenResourceBinder.CreateTokenBinding` 内部走 `target.Bind(prop, new DynamicResourceExtension(key))`，**`DynamicResourceExtension` 默认优先级是 `BindingPriority.Template`**（`Avalonia.Markup.Xaml/MarkupExtensions/DynamicResourceExtension.cs:32`）。

**Tier 1 §7 同优先级 binding 碰撞**：每个上层控件（AutoComplete / Cascader / DatePicker / ComboBox / Mentions 等）的 axaml 模板里都做了 `<atom:Popup IsMotionEnabled="{TemplateBinding IsMotionEnabled}">` —— TemplateBinding 也是 Template 优先级，与构造器 binding 同优先级，重新模板化时存在互相 dispose 的风险。

**fan-in 影响**：fan-in 19 → 整 Gallery 加载时约 76 条 dynamic resource binding 观察者（4 × 19）。

**新实现**（`Popup/Themes/PopupTheme.axaml`，新建）：

```xml
<ControlTheme x:Key="{x:Type atom:Popup}" TargetType="atom:Popup">
    <Setter Property="PopupRootShadow"   Value="{atom:PopupHostTokenResource PopupRootShadow}" />
    <Setter Property="OverlayHostShadow" Value="{atom:PopupHostTokenResource OverlayHostShadow}" />
    <Setter Property="MotionDuration"    Value="{atom:SharedTokenResource MotionDurationMid}" />
    <Setter Property="IsMotionEnabled"   Value="{atom:SharedTokenResource EnableMotion}" />
</ControlTheme>
```

注册：`Popup/Themes/PopupThemes.axaml`（merged dictionary 加一行）。

**节省（结构性，每 N 个 Popup 实例）**：

| 资源 | 旧 | 新 |
| --- | --- | --- |
| 每实例 dynamic resource binding 观察者 | 4 N | 0 |
| 共享 ControlTheme 级 Setter | 0 | 1 ControlTheme（含 4 个 Setter） |
| Tier 1 §7 同优先级碰撞风险 | 有（`IsMotionEnabled`） | 无 |

**保留行为**：

- 用户在控件级用 `<atom:Popup IsMotionEnabled="{TemplateBinding IsMotionEnabled}">` 仍然有效（Template 优先级 < Style 优先级 ⇒ TemplateBinding 赢）。
- `LocalValue`（用户直接 `popup.IsMotionEnabled = ...`）继续覆盖 ControlTheme Setter。
- 主题 `PopupHostTokenResource`/`SharedTokenResource` 内容不变。

### 2.2 `ShadowsAwareContainer.EnsureShadowsRenderer` 延迟到 `HasBoxShadow`

**原实现**（每个 ShadowsAwareContainer 实例无条件创建 shadow Border）：

```csharp
private void ChildChanged(AvaloniaPropertyChangedEventArgs e)
{
    EnsureShadowsRenderer();  // 创建 Border，挂入 VisualChildren/LogicalChildren
    ...
}
```

`EnsureShadowsRenderer` 在 Child 第一次变化时被调用一次，之后通过 `if (_shadowsRenderer != null) return;` 早 return。但即使 `BoxShadow` 是空（`HasBoxShadow == false`），Border 也已经创建并挂在 VisualChildren 里。

**新实现**：

```csharp
static ShadowsAwareContainer()
{
    ...
    BoxShadowProperty.Changed.AddClassHandler<ShadowsAwareContainer>((x, _) =>
    {
        if (x.HasBoxShadow)
        {
            x.EnsureShadowsRenderer();
        }
    });
    AffectsMeasure<ShadowsAwareContainer>(...);
}

private void ChildChanged(AvaloniaPropertyChangedEventArgs e)
{
    if (HasBoxShadow)
    {
        EnsureShadowsRenderer();
    }
    ...
}
```

**节省（结构性）**：

| 场景 | 旧 | 新 |
| --- | --- | --- |
| `BoxShadow=default`（空）的 popup | 1 Border + 2 子节点条目（VisualChildren + LogicalChildren） | 0 |
| `BoxShadow` 非空的 popup | 1 Border | 1 Border（attach 后由 `BoxShadow` 变化触发懒创建） |

**保留行为**：

- 已经创建的 Border 不会因为 `BoxShadow` 重新变空而拆除（与原行为一致）。
- `ArrangeOverride` 已有 `if (HasBoxShadow && _shadowsRenderer != null)` 保护，新实现下 `_shadowsRenderer` 可能为 null，但 `HasBoxShadow == false` 时本就不会渲染影子，逻辑等价。
- `ConfigureShadowsInfo` / `_contentPresenterChildSubscription` 子节点 info 提取逻辑保持不变（CornerRadius / 箭头方向等独立于 BoxShadow，需要无条件订阅）。

---

## 3. 不在本轮范围

- **`_isPlayingCloseMotion` / `_ignoreRequestedPlacementChange` ignore-flag 重入屏蔽**：SKILL Tier 1 §12 + Re-entrancy & Ignore-Flag Guardrails 明令禁止的反模式（与 Cascader `_ignoreSelectedPropertyChanged`、AbstractSelect `IsPlayingCloseMotion` 同根，详见 SKILL Incident Callouts）。修复属 correctness 提交，不混入 perf。
- **`HandleCustomPlacement` 中 `Child.Measure(Size.Infinity) + Child.Arrange(...)` 强制 layout pass**：仅在 `IArrowAwareShadowMaskInfoProvider` 且 `IsArrowVisible() && CanEnabledArrow(placement)` 时触发，每次定位 re-evaluate 一次。可以缓存到首次 ready 后，但需要谨慎处理 placement re-evaluate 时机；本轮先观察。
- **`PopupReflectionExtensions` 反射调用**：每实例一次性 `AddClosingEventHandler`，单次 cost 小，非热点。

---

## 4. 验证

### 4.1 控件库构建

```
dotnet build src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj -c Debug --framework net10.0
→ Build succeeded. 0 Warning(s) 0 Error(s)
```

### 4.2 Gallery 构建

```
dotnet build controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj -c Debug --framework net10.0
→ Build succeeded. 0 Errors（仅 DataGrid 预存的 1 个无关 warning）
```

### 4.3 Gallery 视觉走查（人工）

`AtomUI.Performance` 哈纳斯失同步（参见 README "Tooling" 行），微基准比对暂缓。本轮 Gate 1 正确性验证由 Gallery 视觉走查完成。

需走查的最小用例集合：

- **AutoCompleteShowCase**：打开下拉、过滤、选择，确认 popup 出现/消失动画正常，阴影正确。
- **CascaderShowCase**：层级展开、过滤选择（已修过 [filter-click no-op](#)），动画与阴影正常。
- **ComboBoxShowCase / SelectShowCase**：popup 开关动画、阴影。
- **DatePickerShowCase**：date picker popup + 内嵌 calendar。
- **MentionsShowCase**：trigger 触发的 popup。
- **TooltipShowCase**：hover 触发，确认阴影、动画方向。

特别关注：

1. 各 popup 的 `IsMotionEnabled` 仍然受控件级 `<atom:Popup IsMotionEnabled="{TemplateBinding IsMotionEnabled}">` 与全局 token 影响（默认 ENABLE）。
2. 各 popup 的 `MotionDuration` 默认 `MotionDurationMid`。
3. 各 popup 的 `PopupRootShadow` / `OverlayHostShadow` 默认走 `PopupHostTokenResource` token，与改动前视觉一致。

### 4.4 资源生命周期检查

```bash
rg "TokenResourceBinder.CreateTokenBinding\(this, (PopupRootShadowProperty|OverlayHostShadowProperty|MotionDurationProperty)" \
   --type cs src/AtomUI.Desktop.Controls/
# expect: no match

rg "ConfigureMotionBindingStyle\(\)" --type cs src/AtomUI.Desktop.Controls/Popup/
# expect: no match
```

无新增 disposable 字段、Ensure*/Clear* 链、timer、global subscription，Lifecycle Pairing Checklist 0 条新增。`_shadowsRenderer` 仍在 `OnDetachedFromLogicalTree` 路径里跟随 ShadowsAwareContainer 自身释放，未变化。

---

## 5. 复杂度自评（SKILL Process Gate 3）

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*/Sync*` 方法 | 0 |
| 新增 try/finally 标志位 | 0 |
| 同一文件新增 disposable 字段 | 0 |
| axaml 删除行数 vs C# 增加行数 | axaml +12（新主题 + 一行 ResourceInclude），C# -9 / +9 净 0 |
| Theme Static Rule 检查 | **未触发** — C# token binding → axaml ControlTheme Setter，方向合规 |
| Tier 1 §7 同优先级碰撞 | **修复**（`IsMotionEnabled` 同优先级问题） |

---

## 6. 改动文件清单

| 路径 | 改动 |
| --- | --- |
| `src/AtomUI.Desktop.Controls/Popup/Popup.cs` | 删除构造器内 4 条 token binding（3 个 TokenResourceBinder + 1 个 ConfigureMotionBindingStyle）+ 多余 using |
| `src/AtomUI.Desktop.Controls/Popup/Themes/PopupTheme.axaml` | 新建，定义 `<ControlTheme TargetType="atom:Popup">` 含 4 个 Setter |
| `src/AtomUI.Desktop.Controls/Popup/Themes/PopupThemes.axaml` | 注册新 ResourceInclude |
| `src/AtomUI.Desktop.Controls/Popup/ShadowsAwareContainer.cs` | `ChildChanged` 中 `EnsureShadowsRenderer` 调用前加 `HasBoxShadow` 守卫 + 静态构造器加 `BoxShadowProperty.Changed` 懒创建钩子 |

总计：3 个生产文件改动 + 1 个新主题 + 1 个 ResourceInclude。

---

## 7. 已知不足 / 后续

- 微基准未跑（perf 哈纳斯失同步）。等 harness 修复后回填 cold/repeated mean、KB/instance、visual count 数据。
- Popup 仍存在 ignore-flag 反模式（`_isPlayingCloseMotion` / `_ignoreRequestedPlacementChange` / 以及 Cascader 关联的 `_ignoreSelectedPropertyChanged`），登记为独立 correctness debt（Task #23），不混入 perf 提交。
- `ShadowsAwareContainer.OnAttachedToLogicalTree` 用 `BindUtils.RelayBind` 跨 popup 边界，是 SKILL 允许的"跨生命周期"用法（popup 与内容分别管理），未改。

---

## 8. 追加结构优化：screen fallback 空间判断

`PopupUtils.CalculatePopupRootFlipInfo()` 的选屏 fallback 保持原优先级：anchor 所在屏 -> TopLevel 所在屏 -> Primary -> 第一块屏幕。最后一步从 `screens.All.FirstOrDefault()` 改为 `IReadOnlyList` 的 `Count` + indexer，避免进入 LINQ extension 路径。

| 指标 | 优化前 | 优化后 | 公式 | 提升 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| PopupRoot flip fallback LINQ calls / placement calculation | 1 call | 0 calls | `(1 - 0) / 1` | 100.00% | 结构收益；仅影响主屏 fallback 后的兜底路径，不声明页面 timing 提升 |

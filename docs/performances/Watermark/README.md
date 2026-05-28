# Watermark 性能优化

> 路线图位置：[`../desktop-controls-optimization-roadmap.md`](../desktop-controls-optimization-roadmap.md) Tier 2 #11
> 状态：本轮完成重复 Glyph 替换的 adorner 生命周期修复；页面级耗时仅 smoke-only，不作为收益证明。

---

## 0. 结论

本轮优化修复 `Watermark` 没有通过 `ScopeAwareAdornerLayer.SetAdorner()` 注册自身的问题。旧实现直接把 `Watermark` 加进 layer children，但后续又用 `ScopeAwareAdornerLayer.GetAdorner(target)` 判断是否已安装；这个判断永远拿不到旧 Watermark，导致 `Glyph` 连续替换时会堆多个 Watermark visual，`Glyph = null` 也无法可靠清理。

现在 Watermark 统一走 scope-aware adorner attached property：重复设置同一个 glyph 会复用当前 Watermark，替换 glyph 会先释放旧 Watermark，再安装新 Watermark；清空 glyph 会移除 Watermark，并且不会误清其他非 Watermark adorner。

| 指标 | baseline | optimized | 计算 | 改善 | 结论 |
| --- | ---: | ---: | --- | ---: | --- |
| pending 4 次 Glyph 替换后的 visual/root | 12 | 5 | `(12 - 5) / 12` | 58.33% fewer | 主收益，重复 Watermark 被消除 |
| pending 4 次 Glyph 替换后的 KB/item | 244.1 KB | 73.0 KB | `(244.1 - 73.0) / 244.1` | 70.09% less | 主收益 |
| arranged 后 4 次 Glyph 替换后的 visual/root | 13 | 5 | `(13 - 5) / 13` | 61.54% fewer | 主收益，重复 Watermark 被消除 |
| arranged 后 4 次 Glyph 替换后的 KB/item | 280.3 KB | 81.3 KB | `(280.3 - 81.3) / 280.3` | 71.00% less | 主收益 |
| `Glyph = null` 后残留 Watermark | 1 | 0 | `(1 - 0) / 1` | 100.00% removed | 正确性 + 生命周期收益 |
| pending 4 次 Glyph 替换 ms/item | 2.817 ms | 0.527 ms | `(2.817 - 0.527) / 2.817` | 81.29% faster | smoke-only，不作为收益证明 |
| arranged 后 4 次 Glyph 替换 ms/item | 3.475 ms | 0.627 ms | `(3.475 - 0.627) / 3.475` | 81.96% faster | smoke-only，不作为收益证明 |

---

## 1. 资格门槛

Gallery `WatermarkShowCase` 当前有 4 个示例：基础文字、多行文字、图片水印、自定义内容区水印。实例数不高，但 `Watermark` 是 overlay / adorner 生命周期控件，真实产品里常见场景是页面切换、权限/环境标识切换、动态启停水印。

本轮保留优化的主要原因不是页面加载微收益，而是正确性和生命周期：连续替换 glyph 会堆叠 visual，清空 glyph 无法可靠释放。这个问题随替换次数线性放大，属于必须修的结构性问题。

---

## 2. 根因

旧实现的安装路径：

```csharp
var watermark = ScopeAwareAdornerLayer.GetAdorner(target);
if (watermark != null)
{
    return;
}

watermark = new Watermark(target, GetGlyph(target));
ScopeAwareAdornerLayer.SetAdornedElement(watermark, target);
layer.Children.Add(watermark);
```

问题有三层：

| 问题 | 结果 |
| --- | --- |
| 直接 `layer.Children.Add(watermark)`，没有 `SetAdorner(target, watermark)` | `GetAdorner(target)` 后续仍为 null |
| `OnGlyphChanged` 每次都追加 `LayoutUpdated` handler | 首次 layout 前多次替换 glyph 会排多个待安装回调 |
| `OnDetachedFromVisualTree` 调 `UnInstallWatermark(this)` | `this` 是 Watermark，不是 target，卸载目标不对 |

因此只要 `Glyph` 在首次 layout 前或 arranged 后连续变化，旧 Watermark 不会被识别为已安装实例，新 Watermark 会继续加入视觉树。

---

## 3. 改动

### 3.1 Glyph 变化路径去重

`OnGlyphChanged()` 现在先移除 pending `LayoutUpdated` handler，避免同一 target 累积多个安装回调。

| 状态 | 行为 |
| --- | --- |
| `Glyph == null` | 立即卸载 Watermark |
| target 已 arranged | 立即安装 / 替换 Watermark |
| target 未 arranged | 只挂一个 `LayoutUpdated` pending 回调 |

### 3.2 统一使用 SetAdorner 管理 Watermark

安装时：

- `Glyph == null`：转卸载；
- 当前 adorner 是同一个 glyph 的 Watermark：直接返回；
- 当前 adorner 是非 Watermark：保持旧语义，不覆盖；
- 其他情况：`ScopeAwareAdornerLayer.SetAdorner(target, new Watermark(target, glyph))`。

卸载时：

- 只清理当前 adorner 是 `Watermark` 的情况；
- 非 Watermark adorner 不会被 `Glyph = null` 清掉。

### 3.3 释放路径

`Watermark.OnDetachedFromVisualTree()` 只释放自身订阅的 glyph `PropertyChanged`。目标层面的挂载 / 卸载统一交给 `ScopeAwareAdornerLayer.SetAdorner()`，避免 Watermark 实例误用自己作为 target。

---

## 4. 实测结果

命令：

```bash
dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite watermark --count 80
```

| Scenario | ms/item baseline | ms/item optimized | KB/item baseline | KB/item optimized | Visual baseline | Visual optimized | Logical baseline | Logical optimized |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| `Watermark.Text.Basic` | 0.409 | 0.406 | 47.7 | 48.2 | 4 | 4 | 2 | 2 |
| `Watermark.Text.PendingReplace4` | 2.817 | 0.527 | 244.1 | 73.0 | 12 | 5 | 3 | 3 |
| `Watermark.Text.ArrangedReplace4` | 3.475 | 0.627 | 280.3 | 81.3 | 13 | 5 | 3 | 3 |

`Basic` 没有重复替换场景，visual/logical 保持不变；`KB/item` 的 `47.7 -> 48.2` 属于 0.5 KB 级别噪声，不作为回退依据。本轮主收益看重复替换场景的 visual 和分配下降。

耗时来自单次 headless run，只标记 smoke-only。当前不声明 Gallery 页面加载百分比提升。

---

## 5. 正确性验证

命令：

```bash
dotnet build -c Release -f net10.0 --no-restore \
  tools/performances/AtomUI.Performance/AtomUI.Performance.csproj

dotnet run -c Release -f net10.0 --no-build \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-watermark-states
```

结果：

| 验证 | 结果 |
| --- | --- |
| Release build | passed, 0 warnings, 0 errors |
| `--verify-watermark-states` | passed |
| 首次 layout 前 4 次 Glyph 替换 | 只安装 1 个 Watermark |
| arranged 后 Glyph 替换 | 替换 Watermark 实例，不重复堆叠 |
| `Glyph = null` | 清空 scope-aware adorner 引用并移除 Watermark visual |
| 已存在非 Watermark adorner | Watermark 不覆盖，清空 Glyph 也不误清 |

---

## 6. 复杂度自评

| 指标 | 数值 |
| --- | --- |
| 新增 `Ensure*/Clear*` 方法 | 0 |
| 新增 `_ignoreXxx` / suppression flag | 0 |
| 新增 disposable 字段 | 0 |
| 新增 timer / global subscription | 0 |
| axaml 节点搬到 C# 动态创建 | 否 |
| 生产文件范围 | `Watermark.cs` |

结论：这是 Watermark adorner 所有权修复，不改变 public API，不引入新的长期生命周期对象；收益来自重复 visual 堆叠被消除。

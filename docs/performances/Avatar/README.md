# Avatar 性能优化

`Avatar` 是高频 Data Display 控件，本轮目标是验证并消除默认模板里的互斥内容隐形成本，同时保证 `AvatarGroup` 折叠体系不引入 visual parent 冲突或资源泄露。

## 问题基线

优化前 `AvatarTheme` 为每个 Avatar 同时创建 `IconPresenter`、`Image`、`Svg`、`TextBlock`，再通过 `ContentType` selector 控制显示。真实 `AvatarShowCase` 源 XAML 有 37 个 `Avatar`、5 个 `AvatarGroup`，运行稳定后可见树中仍有 34 个 Avatar，但同时存在 34 个 `IconPresenter`、34 个 `Image`、34 个 `Svg` 和 48 个 `TextBlock`。

优化前真实 Gallery 数据：

| 场景 | Mean | Median | P95 | Alloc mean | Visuals |
| --- | ---: | ---: | ---: | ---: | ---: |
| `AvatarShowCase` cold | `265.86ms` | `265.86ms` | `265.86ms` | `9070.04KB` | `360` |
| `AvatarShowCase` repeated | `42.33ms` | `38.03ms` | `67.92ms` | `7252.55KB` | `360` |

## 实施内容

- `Avatar` 内容 presenter 改为按 `ContentType` 创建，只保留当前使用的 `IconPresenter`、`Image`、`Svg` 或 `TextBlock`。
- `TextBlock` 和 `SizeChanged` 订阅只在文本 Avatar 中创建，并在切换内容类型时解除订阅、清理 visual parent。
- `BitmapSrc` 变化纳入 `ContentType` 重新计算，修复运行时切换 bitmap 不更新内容类型的问题。
- 文本 autoscale 增加输入缓存，避免相同 `Text/Font/Width/Gap` 下重复计算 `TextUtils.CalculateTextSize()`。
- `AvatarGroup` 折叠 flyout 改为按需创建，未折叠时不创建 `_foldCountAvatar`、`FlyoutHost`、`StackPanel`、`Flyout`。
- `AvatarGroup` 折叠 rebuild 改为幂等流程，支持 `MaxDisplayCount` 运行时开关、重复 attach/detach 和 children 变化，不重复添加 visual child。
- `AvatarGroup` 初始化阶段延迟到 attached 后统一构建 visible/fold children，避免 XAML 加载期间每新增一个 child 都 rebuild。

## 最终结果

真实 `AvatarShowCase`，Debug headless，`1300x900`，warmup 10，measured 30。以下为 `2026-05-15 09:07:40 +08:00` 复跑结果：

| 指标 | 优化前 | 优化后 | 改善 |
| --- | ---: | ---: | ---: |
| Cold first navigation | `265.86ms` | `258.35ms` | `2.82%` |
| Repeated mean | `42.33ms` | `32.54ms` | `23.13%` |
| Repeated median | `38.03ms` | `30.31ms` | `20.30%` |
| Repeated P95 | `67.92ms` | `43.19ms` | `36.41%` |
| Alloc mean | `7252.55KB` | `5824.01KB` | `19.70%` |
| Visuals | `360` | `258` | `28.33%` |
| IconPresenter | `34` | `16` | `52.94%` |
| Image | `34` | `1` | `97.06%` |
| Svg | `34` | `4` | `88.24%` |
| TextBlock | `48` | `27` | `43.75%` |

控件级 suite：

| 场景 | 优化前 ms/item | 优化后 ms/item | 改善 |
| --- | ---: | ---: | ---: |
| `Avatar.Icon.Middle` | `0.751` | `0.454` | `39.55%` |
| `Avatar.Text.Single` | `0.706` | `0.340` | `51.84%` |
| `Avatar.Svg` | `1.576` | `1.006` | `36.17%` |
| `Avatar.Bitmap` | `0.512` | `0.389` | `24.02%` |
| `Avatar.Group.NoFold` | `3.244` | `3.245` | `-0.03%` |
| `Avatar.Group.Fold2` | `2.714` | `2.627` | `3.21%` |

`Avatar.GalleryShape` synthetic batch 最新复跑为 `22.082ms`，但该 synthetic 场景对 Gallery route、ShowCaseItem 和导航稳定过程覆盖不足；真实 Gallery navigation 是本轮决策口径。

## 验证覆盖

- Icon/Text/Svg/Bitmap 四类 Avatar 只创建当前需要的 presenter。
- 运行时 `Icon -> Text -> Svg -> Bitmap` 切换会移除旧 presenter，旧实例不保留 visual parent。
- 非折叠 `AvatarGroup` 不创建 fold flyout 私有字段。
- 折叠 `AvatarGroup` 只创建一个 `FlyoutHost`，`MaxDisplayCount` 关闭后释放，重新开启后可恢复。
- folded group detach 后释放 `_foldCountFlyout`，隐藏 children 不残留在 popup stack panel。

## 复现命令

```bash
dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --verify-avatar-states
```

```bash
dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- \
  --suite avatar --count 60
```

```bash
dotnet run --framework net10.0 \
  --project tools/performances/AtomUI.GalleryPerformance/AtomUI.GalleryPerformance.csproj -- \
  --showcase avatar --iterations 30 --warmup 10 --label avatar-optimized-final
```

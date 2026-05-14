# Icon Phase 5 结果

Phase 5 聚焦 Provider / Generator 深化优化。目标不是缓存 `Icon` Control 实例，而是把可安全缓存或生成的不可变元数据前移，并明确 provider cache 的清理边界。

## 改动范围

- `IconProviderCache`
  - 新增 `EnumTypeToIconTypes`，记录 enum provider 类型对应的 icon class。
  - `ClearCache(enumType)` 现在会同时清理 enum 级 type cache、creator cache，以及对应 `TypeToCreator` delegate。
  - `ClearAllCache()` 同步清理新增 tracking cache。
  - cache 超出 `MaxCacheSize` 时通过统一的 `RemoveEnumCache(...)` 回收，避免 enum cache 与 type creator cache 生命周期不一致。
- `AntDesignGenerator`
  - generated icon 文件不再输出无用 `using System;` / `using AtomUI.Media;`。
  - generated transform 从运行时 `TransformParser.Parse("...")` 改为生成期解析并输出 `new Matrix(...)` literal。
  - generator 内部仍使用 `TransformParser.Parse(...)`，但只发生在生成期。
- `MaterialIconsPackages` 与 `IconParkIconsPackage`
  - 两个外部包的 generator 同步输出 `Matrix` literal。
  - 生成代码使用更少 using。
  - `Application` / `Point` 等潜在 generated class 名称冲突通过 `Avalonia.Application` / `Avalonia.Point` 规避。
  - 重新生成全部 icon class。
- 性能工具
  - `tools/performances/AtomUI.Performance` 新增 `--verify-icon-provider-cache`。
  - 验证 `ProvideValue()` 后 cache populate、`ClearCache()` 后 `TypeToCreator` 清理、`ClearAllCache()` 后全量清理。

本阶段继续遵守资源边界：不缓存 `Icon`、`PathIcon`、`IconPresenter` 或任何 visual/logical control 实例，只缓存 type、factory、matrix、bounds 这类不可变值或 delegate。

## Provider switch 评估

计划里提过“生成 enum -> factory switch”。本阶段没有实施，原因是当前数据不支持把它作为收益点：

| Scenario | Phase 6 current |
| --- | ---: |
| `Icon.Provider.SearchOutlined` | `0.065 ms/item`, `30.9 KB/item` |

Provider 当前不是 Gallery 里的主导成本。继续生成大型 switch 会增加 generator 与生成代码复杂度，但收益很可能低于模板节点、layout、item materialization 和 render 成本。因此保留现有 XAML API 与 reflection/factory cache 模型，只补齐清理语义。

## 生成结果

| 包 | generated metadata class | generated `TransformParser.Parse` |
| --- | ---: | ---: |
| `AtomUI.Icons.AntDesign` | 843 | 0 |
| `MaterialIconsPackages` | 10751 | 0 |
| `IconParkIconsPackage` | 2658 | 0 |

说明：

- 上表的 `TransformParser.Parse` 统计只看 generated icon 文件。
- generator 源码里仍保留 `TransformParser.Parse(...)`，用于生成期把 SVG transform 转成 `Matrix` literal。

## 验证

AtomUI 专项验证：

```bash
dotnet run --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj \
  -c Debug --framework net10.0 --no-build -- \
  --verify-accessories --verify-effective-brushes --verify-addon-states \
  --verify-antdesign-metadata --verify-icon-hidden-slots --verify-icon-provider-cache
```

结果：

- AddOn accessory / effective brush / addon state 验证通过。
- AntDesign generated metadata 验证通过。
- Icon hidden slot 验证通过。
- Icon provider cache 清理验证通过。

外部包本地源验证：

```bash
cd /Users/chinboy/Projects/dotnet/AtomUIV6/scripts
pwsh ./PublishToLocalSources.ps1 \
  -localSourcesDir /Users/chinboy/Data/nuget-repo \
  -buildType Debug

cd /Users/chinboy/Projects/dotnet/MaterialIconsPackages
NUGET_PACKAGES=/tmp/atomui-material-local-verify-pkgs \
  dotnet restore Material.slnx --force-evaluate --no-cache
NUGET_PACKAGES=/tmp/atomui-material-local-verify-pkgs \
  dotnet build Material.slnx -c Debug --no-restore

cd /Users/chinboy/Projects/dotnet/IconParkIconsPackage
NUGET_PACKAGES=/tmp/atomui-iconpark-local-verify-pkgs \
  dotnet restore IconPark.slnx --force-evaluate --no-cache
NUGET_PACKAGES=/tmp/atomui-iconpark-local-verify-pkgs \
  dotnet build IconPark.slnx -c Debug --no-restore
```

最终验证结果：

| 仓库 | Restore | Build |
| --- | --- | --- |
| `MaterialIconsPackages` | 通过，临时 NuGet cache | 通过，0 warning / 0 error |
| `IconParkIconsPackage` | 通过，临时 NuGet cache | 通过，0 warning / 0 error |

## 判断

Phase 5 的主要价值是把 cache 生命周期补完整，并移除 generated code 的运行时 transform parse 成本。它不是大幅降低 Gallery 打开耗时的阶段，但它减少了基础设施层面的隐性成本和后续动态图标包场景的缓存泄露风险。

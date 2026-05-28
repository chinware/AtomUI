# InfoFlyout Performance Notes

## Scope

`InfoFlyout` 在代码中对应 `FlyoutHost` + `Flyout`。本轮只处理关闭态和生命周期成本，不改模板结构、不改打开后的 presenter 视觉行为。

## Root Cause

- `FlyoutHost` 原先在 `Flyout` 变为 `null` 时不释放旧 flyout 上的 13 条 host-to-flyout property binding，旧 flyout 会继续被 binding 链路保留。
- `FlyoutHost.OnPropertyChanged()` 使用 logical-tree 状态决定是否注册 flyout binding，和实际 visual-tree attach/detach 生命周期不一致。
- `FlyoutStateHelper.SetupTriggerHandler()` 为设置 light-dismiss 直接访问 `Flyout.Popup`。Avalonia `PopupFlyoutBase.Popup` 会读取 `_popupLazy.Value`（`.referenceprojects/Avalonia/src/Avalonia.Controls/Flyouts/PopupFlyoutBase.cs:86`），因此关闭态也会提前创建 Popup shell。
- AtomUI `Flyout.CreatePopup()` 已经把 `Popup.IsLightDismissEnabled` 绑定到 `Flyout.IsLightDismissEnabled`，所以设置 Flyout 自身属性即可，不需要提前访问 `Popup`。

## Changes

- `FlyoutHost` 增加 `_registeredFlyout`，将 property binding 注册/释放改成 visual-tree attach gated 的显式 `RegisterFlyoutProperties()` / `UnregisterFlyoutProperties()`。
- `FlyoutHost.Flyout = null`、替换 Flyout、detach 都会释放旧 flyout 的 13 条 host-to-flyout binding。
- `FlyoutStateHelper` 对 AtomUI `Flyout` 设置 `IsLightDismissEnabled`，避免关闭态触发 `Popup` lazy shell 创建。
- `PopupConfirmFlyout` 构造函数同样改为写 `IsLightDismissEnabled`，避免关闭态访问 `Popup`。
- 新增 `--suite infoflyout` 和 `--verify-infoflyout-states`。

## Benefit

| Metric | Baseline | Optimized | Formula | Improvement | Conclusion |
| --- | ---: | ---: | --- | ---: | --- |
| Closed Popup shell / InfoFlyout host | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；关闭态不再创建 Popup shell |
| Host-to-flyout bindings after `Flyout=null` / host | 13 | 0 | `(13 - 0) / 13` | 100.00% | 有效；置空后释放旧 flyout binding 链路 |
| Registered flyout reference after detach / host | 1 | 0 | `(1 - 0) / 1` | 100.00% | 有效；detach 后不再保留 registered flyout |
| `Flyouts.GalleryShape` closed Popup shells | 12 / 29 | 0 / 29 | `(12 - 0) / 12` | 100.00% | 有效；共享 flyout 关闭态 lazy shell 验证通过 |
| `Flyouts.GalleryShape` KB/item | 13650.5 KB | 13399.4 KB | `(13650.5 - 13399.4) / 13650.5` | 1.84% | smoke-only；分配下降符合结构优化，不作为稳定页面收益证明 |
| `Flyouts.GalleryShape` ms/item | 47.363 ms | 44.121 ms | `(47.363 - 44.121) / 47.363` | 6.84% | smoke-only；单次 timing 只作异常检查 |

## Verification

```bash
dotnet build -c Release -f net10.0 --no-restore tools/performances/AtomUI.Performance/AtomUI.Performance.csproj
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --verify-infoflyout-states --verify-flyout-states --verify-dropdownbutton-states --verify-splitbutton-states
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite infoflyout --count 80
dotnet run -c Release -f net10.0 --no-build --project tools/performances/AtomUI.Performance/AtomUI.Performance.csproj -- --suite flyouts --count 80
```

Notes:

- `--verify-infoflyout-states` covers detached no-binding, attach binding count, Flyout replacement, `Flyout=null` cleanup, detach cleanup, and closed Popup shell laziness.
- `--verify-flyout-states` now passes for the existing shared flyout suite after the light-dismiss lazy fix.
- `--verify-popupconfirm-states` still has existing failures in `PopupConfirmContainer` optional slot removal/button lifecycle. Those failures are outside this round's flyout lazy shell path and should be handled as a separate correctness task.

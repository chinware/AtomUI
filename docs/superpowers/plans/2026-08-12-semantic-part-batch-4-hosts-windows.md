# Semantic Part 第四批 Popup 与独立宿主实施计划

> **供智能体执行者使用：** 使用 `superpowers:executing-plans` 在当前会话中执行，不得使用 subagent。每个控件修改源码前都必须明确 Visual root ownership 并获得批准。

**目标：** 为 9 个具有 Ant Design 6.6.0 稳定发布源码公开 Semantic DOM 对应 API 的 Popup、Overlay 和服务宿主控件家族建立 Semantic Part 契约，并覆盖完整生命周期与多 root 隔离。

**架构：** 每个生产 owner 通过现有 host/session 生命周期公开 selector Part。Gallery 可以提供由示例显式拥有的 additional root，但生产控件不得引入 Preview API、全局 root registry 或运行时搜索。服务型控件使用真实 owner 边界。

**技术栈：** .NET 10、Avalonia 12、AtomUI Desktop Controls、AXAML、xUnit v3、Avalonia Headless、平台宿主、AtomUI Gallery、NativeAOT。

## 全局约束

- 遵循[全量改造总计划](2026-08-12-semantic-part-control-rollout.md)和[全量改造设计](../specs/2026-08-12-semantic-part-control-rollout-design.md)。
- Gate A 必须列明每个 Visual root、owner、创建点、attach/open 状态转换和 close/detach 释放路径。
- 不得向生产控件添加全局 TopLevel registry、服务查找或 Preview 专用属性、事件、接口。
- Popup/Overlay 以及服务所使用的 Window host 测试覆盖打开、关闭、重新打开、owner detach 和多宿主隔离；不得因此给
  被排除的 `Window` / `WindowTitleBar` owner 增加 Semantic Part。
- Gallery additional root 必须是示例显式提供的输入，并且只能在打开 Semantic Parts Tab 后创建。
- Gate B 改动保持未提交，直到用户明确完成验证并授权提交。

---

### 任务 1：ImagePreviewer

**控件文档：** `docs/controls/desktop/data-display/image-previewer/overview.md`, `docs/controls/desktop/data-display/image-previewer/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/ImagePreviewer/**/*.cs`, 全部 `Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/ImagePreviewer`；Gallery `controlgallery/AtomUIGallery/ShowCases/DataDisplay/ImagePreviewer`.

**风险类型：** Overlay/Dialog 宿主、多个 public 子控件、异步图片加载、renderer 和 motion。

- [x] **Gate A 设计审核：** 审计 previewer/group/dialog/overlay host、viewer/renderer、title bar、toolbar/nav/cover/item 的 owner；确认 image viewport、loading/error、title、toolbar/nav/close/mask 区域，明确 service/dialog/overlay Visual root、source 切换和释放路径。
- [x] 更新两份控件文档，写明准确的 Descriptor、cross-root/runtime 标志、owner/session 生命周期、真实节点、兼容性和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [x] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/ImagePreviewer/ImagePreviewerSemanticPartTests.cs`，覆盖 single/group、loading/error/success、navigation、toolbar/title、overlay/dialog open-close-reopen、source 替换和 owner 隔离；Gallery 使用显式 additional root，并执行 NativeAOT 验证。
- [x] 运行 Generator Semantic 测试、目标宿主/控件测试、GalleryBase 和 Gallery 测试、LLMS verify、NativeAOT publish 以及 `git diff --check`；当契约依赖原生/窗口行为时执行平台冒烟检查。
- [x] **强制停止：** 保持 ImagePreviewer 的所有实现改动未提交，直到用户验证真实宿主行为并明确授权提交。

### 任务 2：InfoFlyout

**控件文档：** `docs/controls/desktop/data-display/info-flyout/overview.md`, `docs/controls/desktop/data-display/info-flyout/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Flyouts/**/*.cs`, `src/AtomUI.Desktop.Controls/Flyouts/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/Flyouts`；Gallery `controlgallery/AtomUIGallery/ShowCases/DataDisplay/InfoFlyout`.

**风险类型：** PopupFlyoutBase、延迟创建的 Popup、menu/tree presenter、资源生命周期。

- [x] **Gate A 设计审核：** 审计 `FlyoutHost`、Flyout/FlyoutPresenter、Menu/TreeView flyout presenter 的 owner；确认 anchor content 与 popup arrow/surface/content/item 区域，区分 host root 与 Flyout presenter root，记录 Popup 延迟创建、overlay 与 PopupRoot 差异、open-close 和 resource bridge。
- [x] 更新两份控件文档，写明准确的 Descriptor、cross-root/runtime 标志、owner/session 生命周期、真实节点、兼容性和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [x] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/Flyouts/FlyoutSemanticPartTests.cs`，覆盖 content/menu/tree variants、arrow placements/flips、overlay/PopupRoot、open-close-reopen、presenter 资源生命周期 和 item containers。
- [x] 运行 Generator Semantic 测试、目标宿主/控件测试、GalleryBase 和 Gallery 测试、LLMS verify、NativeAOT publish 以及 `git diff --check`；当契约依赖原生/窗口行为时执行平台冒烟检查。
- [x] **强制停止：** 保持 InfoFlyout 的所有实现改动未提交，直到用户验证真实宿主行为并明确授权提交。

### 任务 3：ToolTip

**控件文档：** `docs/controls/desktop/data-display/tooltip/overview.md`, `docs/controls/desktop/data-display/tooltip/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Tooltip/*.cs`, `src/AtomUI.Desktop.Controls/Tooltip/Themes/ToolTipTheme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/Tooltip`；Gallery `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tooltip`.

**风险类型：** Attached service、Popup/overlay 模式、延迟、detached owner。

- [x] **Gate A 设计审核：** 审计 ToolTip instance、attached `ToolTipService` 和 OverflowTip owner，确认 content/arrow/surface regions、service-created tooltip ownership 和 Popup modes；记录 delay timers、anchor detach、overlay vs PopupRoot 和 repeated show/hide。
- [x] 更新两份控件文档，写明准确的 Descriptor、cross-root/runtime 标志、owner/session 生命周期、真实节点、兼容性和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [x] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/Tooltip/ToolTipSemanticPartTests.cs`，覆盖 explicit/service/overflow tooltip、placement、overlay/PopupRoot、show-hide-reopen、delay cancellation 和 detach cleanup；Gallery additional root 只存在于示例侧。
- [x] 运行 Generator Semantic 测试、目标宿主/控件测试、GalleryBase 和 Gallery 测试、LLMS verify、NativeAOT publish 以及 `git diff --check`；当契约依赖原生/窗口行为时执行平台冒烟检查。
- [x] **强制停止：** 保持 ToolTip 的所有实现改动未提交，直到用户验证真实宿主行为并明确授权提交。

### 任务 4：Tour

**控件文档：** `docs/controls/desktop/data-display/tour/overview.md`, `docs/controls/desktop/data-display/tour/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Tour/*.cs`, `src/AtomUI.Desktop.Controls/Tour/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/Tour`；Gallery `controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tour`.

**风险类型：** Popup 与 overlay mask、step 容器、target tracking、motion。

- [x] **Gate A 设计审核：** 审计 Tour/TourLayer、step/steps view、indicators 和 Popup owner；确认 mask/spotlight、panel/title/content/actions/indicator/arrow regions，记录 target switch、placement, step rebuild, open-close 和 target detach。
- [x] 更新两份控件文档，写明准确的 Descriptor、cross-root/runtime 标志、owner/session 生命周期、真实节点、兼容性和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [x] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/Tour/TourSemanticPartTests.cs`，覆盖 steps navigation、default/text indicator、placement、target change/detach、Popup reopen、overlay mask 和 step collection lifecycle；验证 NativeAOT 和 additional root。
- [x] 运行 Generator Semantic 测试、目标宿主/控件测试、GalleryBase 和 Gallery 测试、LLMS verify、NativeAOT publish 以及 `git diff --check`；当契约依赖原生/窗口行为时执行平台冒烟检查。
- [x] **强制停止：** 保持 Tour 的所有实现改动未提交，直到用户验证真实宿主行为并明确授权提交。

### 任务 5：Drawer

**控件文档：** `docs/controls/desktop/feedback/drawer/overview.md`, `docs/controls/desktop/feedback/drawer/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Drawer/*.cs`, `src/AtomUI.Desktop.Controls/Drawer/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/Drawer`；Gallery `controlgallery/AtomUIGallery/ShowCases/Feedback/Drawer`.

**风险类型：** Window overlay layer、运行时容器、stacked session、motion。

- [x] **Gate A 设计审核：** 审计 Drawer owner、DrawerContainer/InfoContainer 和 overlay layer；确认 mask/panel/header/title/extra/content/footer/close regions，明确 nested/stacked drawers、placement, motion, drawn-titlebar overlay bounds 和 teardown。
- [x] 更新两份控件文档，写明准确的 Descriptor、cross-root/runtime 标志、owner/session 生命周期、真实节点、兼容性和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [x] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/Drawer/DrawerSemanticPartTests.cs`，覆盖四种 placement、header/footer/close 变体、stacked/open-close-reopen、owner Window detach，并证明不会保留旧容器；验证 Gallery additional root 和 NativeAOT。
- [x] 运行 Generator Semantic 测试、目标宿主/控件测试、GalleryBase 和 Gallery 测试、LLMS verify、NativeAOT publish 以及 `git diff --check`；当契约依赖原生/窗口行为时执行平台冒烟检查。
- [x] **强制停止：** 保持 Drawer 的所有实现改动未提交，直到用户验证真实宿主行为并明确授权提交。

### 任务 6：Message

**控件文档：** `docs/controls/desktop/feedback/message/overview.md`, `docs/controls/desktop/feedback/message/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Message/*.cs`、`src/AtomUI.Desktop.Controls/Message/Themes/*Theme.axaml`；在 `tests/AtomUI.Desktop.Controls.Tests/Message` 下创建测试；Gallery `controlgallery/AtomUIGallery/ShowCases/Feedback/Message`。

**风险类型：** 服务创建的 Window 宿主、运行时 card、queue 和 motion。

- [ ] **Gate A 设计审核：** 审计 `Message` service/session、WindowMessageManager 和 public `MessageCard` owner；确认 icon/content/action/status surface 区域，明确 manager host layer、多个 message stack、timeout/manual close、motion 和 Window detach。
- [ ] 更新两份控件文档，写明准确的 Descriptor、cross-root/runtime 标志、owner/session 生命周期、真实节点、兼容性和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 创建 `tests/AtomUI.Desktop.Controls.Tests/Message/MessageSemanticPartTests.cs`，覆盖全部类型、custom content/icon、多项 queue、timeout/manual close、host attach/detach 和 card 保留检查；Gallery 只在选择 Tab 后创建显式示例 root，并执行 NativeAOT 验证。
- [ ] 运行 Generator Semantic 测试、目标宿主/控件测试、GalleryBase 和 Gallery 测试、LLMS verify、NativeAOT publish 以及 `git diff --check`；当契约依赖原生/窗口行为时执行平台冒烟检查。
- [ ] **强制停止：** 保持 Message 的所有实现改动未提交，直到用户验证真实宿主行为并明确授权提交。

### 任务 7：Modal / Dialog

**控件文档：** `docs/controls/desktop/feedback/modal/overview.md`、`docs/controls/desktop/feedback/modal/implementation.md`；只有已批准的 Part 模型改变稳定的尺寸契约时，才更新现有 `host-sizing-design.md`。

**证据范围：** `src/AtomUI.Desktop.Controls/Dialog/**/*.cs`、`src/AtomUI.Desktop.Controls/MessageBox/**/*.cs` 和全部相关 Dialog/MessageBox 主题；测试 `tests/AtomUI.Desktop.Controls.Tests/Dialog` 和 `MessageBox`；Gallery `controlgallery/AtomUIGallery/ShowCases/Feedback/Modal`。

**风险类型：** Overlay 和 native Window 宿主、session state machine、多个 public 子控件、resize/motion。

- [ ] **Gate A 设计审核：** 审计 Dialog/MessageBox owner、DialogSurface、button box/header/resizer、overlay presenter/mask 和 window presenter；确认 surface/title/icon/content/footer/actions/close/mask/resize regions 和 owner 覆盖 Overlay/Window，记录 session open-close, teardown, host sizing 和 nested dialogs。
- [ ] 更新两份控件文档，写明准确的 Descriptor、cross-root/runtime 标志、owner/session 生命周期、真实节点、兼容性和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/Dialog/DialogSemanticPartTests.cs` 并扩展 MessageBox 测试，覆盖 Overlay/Window、modal/modeless、MessageBox 类型、button、resize/motion、嵌套 session、close failure 和 root release；验证 Gallery additional root 和 NativeAOT。
- [ ] 运行 Generator Semantic 测试、目标宿主/控件测试、GalleryBase 和 Gallery 测试、LLMS verify、NativeAOT publish 以及 `git diff --check`；当契约依赖原生/窗口行为时执行平台冒烟检查。
- [ ] **强制停止：** 保持 Modal / Dialog 的所有实现改动未提交，直到用户验证真实宿主行为并明确授权提交。

### 任务 8：Notification

**控件文档：** `docs/controls/desktop/feedback/notification/overview.md`, `docs/controls/desktop/feedback/notification/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/Notifications/*.cs`, `src/AtomUI.Desktop.Controls/Notifications/Themes/*Theme.axaml`；测试 `tests/AtomUI.Desktop.Controls.Tests/Notifications`；Gallery `controlgallery/AtomUIGallery/ShowCases/Feedback/Notification`.

**风险类型：** 服务创建的 Window 宿主、card/progress 运行时节点、queue 和 placement。

- [ ] **Gate A 设计审核：** 审计 Notification/session、WindowNotificationManager、NotificationCard/ProgressBar owner；确认 icon/message/description/action/close/progress/surface regions，记录 placements、stack, duration/progress, manual close, replacement 和 Window detach。
- [ ] 更新两份控件文档，写明准确的 Descriptor、cross-root/runtime 标志、owner/session 生命周期、真实节点、兼容性和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 新增 `tests/AtomUI.Desktop.Controls.Tests/Notifications/NotificationSemanticPartTests.cs`，覆盖 type/placement、custom content/action、progress/duration、多项 queue、close/detach 和 card 保留检查；验证 Gallery additional root 和 NativeAOT。
- [ ] 运行 Generator Semantic 测试、目标宿主/控件测试、GalleryBase 和 Gallery 测试、LLMS verify、NativeAOT publish 以及 `git diff --check`；当契约依赖原生/窗口行为时执行平台冒烟检查。
- [ ] **强制停止：** 保持 Notification 的所有实现改动未提交，直到用户验证真实宿主行为并明确授权提交。

### 任务 9：PopupConfirm

**控件文档：** `docs/controls/desktop/feedback/popup-confirm/overview.md`, `docs/controls/desktop/feedback/popup-confirm/implementation.md`

**证据范围：** `src/AtomUI.Desktop.Controls/PopupConfirm/*.cs`、`src/AtomUI.Desktop.Controls/PopupConfirm/Themes/*Theme.axaml` 及已批准的 Flyout/Button 共享契约；在 `tests/AtomUI.Desktop.Controls.Tests/PopupConfirm` 下创建测试；Gallery `controlgallery/AtomUIGallery/ShowCases/Feedback/PopupConfirm`。

**风险类型：** 派生自 FlyoutHost 的 Popup、运行时 presenter binding、action Button。

- [ ] **Gate A 设计审核：** 审计 PopupConfirm host、PopupConfirmFlyout 和 container owner；确认 icon/title/content/actions/surface/arrow regions，避免穿透 nested Button descriptor，记录 relay binding acquire/release, Popup reopen 和 confirm loading。
- [ ] 更新两份控件文档，写明准确的 Descriptor、cross-root/runtime 标志、owner/session 生命周期、真实节点、兼容性和验证矩阵；运行 LLMS verify 和 `git diff --check`；随后停止并等待用户批准。
- [ ] **Gate B 实现与验证：** 创建 `tests/AtomUI.Desktop.Controls.Tests/PopupConfirm/PopupConfirmSemanticPartTests.cs`，覆盖 status/icon/content/cancel visibility、confirm loading、open-close-reopen、presenter binding disposal 和 nested Button owner 隔离；NativeAOT。
- [ ] 运行 Generator Semantic 测试、目标宿主/控件测试、GalleryBase 和 Gallery 测试、LLMS verify、NativeAOT publish 以及 `git diff --check`；当契约依赖原生/窗口行为时执行平台冒烟检查。
- [ ] **强制停止：** 保持 PopupConfirm 的所有实现改动未提交，直到用户验证真实宿主行为并明确授权提交。

## 批次收尾

> 2026-09-10 复核：ImagePreviewer、InfoFlyout、ToolTip、Tour、Drawer 五个家族的单项任务框已置为已完成（均已按用户授权提交）。Message、Modal / Dialog、Notification、PopupConfirm 四个家族未开始，本批次仍未收尾。

- [ ] 确认 9 个控件家族分别拥有用户授权的独立提交。
- [ ] 运行完整 Desktop Controls、Generator、GalleryBase 和 Gallery 测试，并执行 Popup/Overlay 生命周期筛选。
- [ ] 运行 LLMS verify、Gallery NativeAOT publish、适用的平台冒烟检查和 `git diff --check`。
- [ ] 更新总计划清单，不创建批次提交。

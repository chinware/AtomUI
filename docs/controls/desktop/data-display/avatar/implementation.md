# Avatar 桌面版实现原理

本文档描述 Avatar 控件家族的源码 ownership、图片加载状态流、模板生命周期和资源释放规则。公共契约见
[Avatar 控件家族架构设计](overview.md)，Token 语义见 [Avatar Token 设计](token.md)。

## 1. 实现定位

本文覆盖单头像、桌面 AvatarGroup、共享单图 controller、主题接入和图片租约生命周期。具体 StyledProperty 注册、Token 派生值和
AXAML selector 仍以源码为准；本文只记录跨文件协作和维护时不可破坏的实现边界。

## 2. 源码文件结构

`AtomUI.Controls` 拥有单头像实现：

```text
src/AtomUI.Controls/Avatar/
├── AbstractAvatar.cs
├── Avatar.cs
├── AvatarPseudoClass.cs
├── AvatarShape.cs
├── AvatarToken.cs
└── Themes/AvatarTheme.axaml
```

`AtomUI.Desktop.Controls` 只拥有桌面组合控件：

```text
src/AtomUI.Desktop.Controls/Avatar/
├── AvatarGroup.cs
├── AvatarGroupFoldInfo.cs
└── Themes/AvatarGroupTheme.axaml
```

`Avatar` 和 `AvatarToken` 不在 Desktop 包中保留副本。Desktop 通过对 Controls 的正常项目引用复用类型和 Token resource。

## 3. 核心类职责

- `AbstractAvatar`：Avalonia 属性、图片 controller host、内容优先级、尺寸/形状归一、加载状态和模板生命周期。
- `Avatar`：可实例化的公共控件类型，不建立第二套状态。
- `ImageLoadController`：与 `AsyncImage` 共用的单图片请求 owner，负责 generation、取消、fallback、Reload、progress 和租约提交。
- `AvatarToken`：从全局 Token 派生 Avatar 与 AvatarGroup 共用的视觉值。
- `AvatarGroup`：子 Avatar 排列、重叠宽度、折叠计数和 Flyout 生命周期。

## 4. 状态与数据流

```text
Source / FallbackSource / RequestOptions / Bounds / render scaling
  -> ImageLoadController generation
  -> Application.GetImageLoader().LoadAsync(...)
  -> ImageLoadResult lease
  -> UI dispatcher generation check
  -> LoadedImage + LoadState/Error/Progress
  -> ContentType(Image/Text/Icon) + pseudo-classes
  -> AvatarTheme
```

`AbstractAvatar` 实现 `IImageLoadControllerHost`。它只向 controller 暴露当前 Visual、Normal priority、attach 状态、16 px
量化后的物理解码尺寸和状态提交回调，不直接访问 transport 或 cache。

Source、FallbackSource 或 RequestOptions 变化调用 `SourceConfigurationChanged()`。Arrange 完成后调用 `RefreshSize()`；只有有效
Bounds 产生新的尺寸 bucket 时才需要新请求。属性快照和状态提交均在 UI 线程边界完成。

## 5. 组合结构模型

### 控件角色图

```text
Avatar
  -> Border#Frame (internal-observable)
     -> ContentPresenter#IconPresenter (internal-observable)
     -> Image#ImagePresenter (internal-observable)
     -> TextBlock#PART_TextPresenter (template-stable)

AvatarGroup
  -> child Avatar collection (public)
  -> fold Avatar + Flyout (internal-observable)
```

### 协作节点

| 节点 | 类型 | 来源 | 生命周期 owner | 影响的 public API | 稳定性 | Agent 使用边界 |
| --- | --- | --- | --- | --- | --- | --- |
| `Avatar` | public control | `AvatarTheme.axaml` | visual tree | 单头像全部 API | public | 用户可直接使用 |
| `ImageLoadController` | internal coordinator | C# | `AbstractAvatar` | Source、fallback、状态、Reload | internal-observable | 只用于理解统一状态机 |
| `PART_TextPresenter` | `TextBlock` | `AvatarTheme.axaml` | template | Text、Gap、Size | template-stable | 改名需同步实现、主题和文档 |
| `AvatarGroup` | public control | `AvatarGroupTheme.axaml` | visual tree | Children、折叠和尺寸投影 | public | 用户可直接使用 |
| fold Avatar/Flyout | internal visual | C# + Theme | `AvatarGroup` | MaxDisplayCount、trigger、fold colors | internal-observable | 不作为独立 API 暴露 |

内容选择只有三个结果：

1. `LoadedImage != null` 时选择 Image。
2. 否则 `Text != null` 时选择 Text。
3. 否则选择 Icon。

因此 Loading、取消或主来源失败不会主动清空 Text/Icon。Controller 在主来源终态失败后至多请求一次 FallbackSource；主来源
和 fallback 身份相同则不重复请求。fallback 成功时设置 `:loaded` 与 `:fallback`，最终失败时设置 `:failed`。

事件与状态提交顺序必须一致：先保存当前结果和状态，再触发 `ImageOpened`；最终失败先提交错误状态，再触发 `ImageFailed`。
取消、detach 和 generation 过期不属于图片失败事件。

## 6. 生命周期与模板接入

成功的 `ImageLoadResult` 是图片使用租约。Controller 持有当前租约，来源变化、detach、最终失败或 controller dispose 时释放。
替换图片时先接管新租约，再释放旧租约，避免 decoded cache entry 在视觉切换前被销毁。

`Reload()` 不创建新的公开来源状态；它对当前 Source 发起 `ImageCacheMode.Reload` 请求：

- HTTP 允许使用已有 metadata 做条件重验证。
- File、Asset、StorageFile、Bytes 和 Stream 强制重新读取来源。
- Reload 期间可保留旧图片；新结果只有通过 generation 校验后才能替换。

`ImageLoadSource.FromImage` 是 borrowed source。其结果租约只约束消费期，不能 dispose 调用方的 `IImage`。

| 获取/建立 | 对应释放 |
| --- | --- |
| controller attach 与 loader waiter | visual detach 时 controller detach/cancel |
| 当前 `ImageLoadResult` lease | Source 清空、替换终态、detach 或 dispose |
| `PART_TextPresenter.SizeChanged` | template reapply 前解除 |
| AvatarGroup motion binding | group detach 时 dispose |
| AvatarGroup fold Flyout/临时视觉 | rebuild、detach 或 owner 释放时清理 |

Reattach 时 controller 使用当前 Source 和当前尺寸重新请求。模板重套用必须先解除旧 TextBlock 事件，再连接新
`PART_TextPresenter`。AvatarGroup 只管理组合视觉，不取消或复用子 Avatar 的图片请求。

## 7. 交互与事件处理

Avatar 没有独立的 pointer 或 keyboard 状态机；基础交互由 Avalonia 控件和 Theme 处理。图片回调先在 UI dispatcher 校验
generation 和 attach 状态，再提交结果。成功时先保存 lease、状态和内容，再触发 `ImageOpened`；最终失败时先提交 error 状态，
再触发 `ImageFailed`。取消、detach 和过期 generation 不触发失败事件。`Reload()` 是唯一命令式图片入口。

## 8. 内部算法与关键流程

`AvatarTheme.axaml` 通过 internal `ContentType` 选择 Image、Text 或 Icon presenter。唯一稳定 template part 是
`PART_TextPresenter: Avalonia.Controls.TextBlock`。

模板重套用时先解除旧 `SizeChanged` 订阅，再取得并订阅新 TextBlock。文本缩放输入为最终控件宽度、`Gap`、FontSize 和
FontFamily；缩放上限为 1，不放大短文本。`Gap * 2 >= Width` 或非 Text 状态时清除 transform。

`Size` 非 `NaN` 时保存原 SizeType 并切换到 Custom；恢复 `NaN` 后回到原 SizeType。Circle 形状把最终宽度的一半写入模板
CornerRadius。图片请求尺寸来自 Bounds 与 TopLevel render scaling，而不是 Token 名称或逻辑像素直接值。

## 9. 资源、性能与 AOT 边界

- 不进行同步网络或文件 I/O。
- 不使用固定延迟等待布局；Arrange 和尺寸 bucket 是唯一尺寸就绪信号。
- 不创建控件私有 `HttpClient`、cache 或 scheduler。
- Theme、codec 和 loader 通过静态注册保留，不使用反射扫描。
- Browser 使用统一 loader 的 Browser 能力矩阵；Avatar 不增加平台分支 API。
- 快速 Source 切换只允许当前 generation 回写，旧结果必须及时释放。

## 10. 维护不变量

- `ImageLoadController` 是 Avatar 与 `AsyncImage` 的单图状态机 owner，不在 Avatar 内复制请求协调逻辑。
- 内容优先级始终为 Image、Text、Icon；Loading 和取消不清空可用降级内容。
- 每个已提交 cache image 必须由一个有效 lease 支撑；borrowed image 永不由控件销毁。
- Source、options、尺寸或 attach generation 变化后，旧结果只能释放，不能回写状态。
- Template reapply、detach、group rebuild 和 Application dispose 都有明确的取消、解绑和释放路径。
- 单头像和 Token 只存在于 `AtomUI.Controls`，Desktop 包只拥有 AvatarGroup 组合能力。

## 11. 测试与验证

维护 Avatar 时至少覆盖：

- Source、FallbackSource、Text、Icon 优先级与状态伪类。
- A -> B 快速切换、Reload、detach/reattach、尺寸 bucket 和 render scaling。
- 主失败/fallback 成功、最终失败、事件顺序和 progress generation。
- owned cache image 与 borrowed image 的释放责任。
- AvatarTheme 独立于 Desktop 类型，AvatarGroup 仍能消费同一 Token。
- Gallery 示例只使用 `Source`/`ImageLoadSource`。

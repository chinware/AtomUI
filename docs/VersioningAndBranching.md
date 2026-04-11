# AtomUI 版本规范与 Git 分支策略

> 本文档定义了 AtomUI 项目的版本号规范和 Git 分支管理策略。  
> 参考依据：[SemVer 2.0.0](https://semver.org/)、[Avalonia 分支与版本模型](https://github.com/AvaloniaUI/Avalonia)、[NuGet 版本约定](https://learn.microsoft.com/nuget/concepts/package-versioning)、以及 AtomUI 项目自身的历史实践。

---

## 1. 版本号规范

### 1.1 版本号格式

AtomUI 采用 **语义化版本 (Semantic Versioning 2.0.0)** 规范，完整格式如下：

```
MAJOR.MINOR.PATCH[-PRERELEASE][+BUILD]
```

| 字段 | 含义 | 示例 |
|---|---|---|
| `MAJOR` | **主版本号** — 与 Ant Design 设计规范大版本对齐（见 §1.2） | `5` |
| `MINOR` | **次版本号** — 新增功能（向后兼容） | `5.1` |
| `PATCH` | **修订号** — Bug 修复、性能优化（向后兼容） | `5.1.3` |
| `PRERELEASE` | **预发布标签** — 标识非正式发布（见 §1.3） | `5.2.0-beta.1` |
| `BUILD` | **构建元数据**（仅信息性，不影响排序） | `5.2.0-beta.1+20260410` |

### 1.2 主版本号 (MAJOR) 严格锚定 Ant Design

> **核心原则：AtomUI 的 MAJOR 版本号严格锚定 Ant Design 的 MAJOR 版本号，两者保持一致。**

AtomUI 是 Ant Design 设计规范在 .NET / Avalonia 平台上的完整实现。为了让使用者一眼就能识别 AtomUI 实现的是哪个版本的 Ant Design 设计语言，AtomUI 的主版本号 **直接采用** 对应 Ant Design 的主版本号。

#### Ant Design 发版节奏（参考 [antd Release Schedule](https://ant.design/changelog#release-schedule)）

Ant Design 自身遵循如下发版计划：

| 发布类型 | 频率 | 说明 |
|---|---|---|
| **Patch Release** (修订版) | 每周 | 常规 Bug 修复（紧急情况随时发布） |
| **Minor Release** (次版本) | 每月 | 新增功能、向后兼容的变更 |
| **Major Release** (大版本) | 不定期 | 包含破坏性变更 (Breaking Changes)，长周期演进 |

Ant Design 的大版本历史：

| Ant Design 版本 | 发布时间 | 关键变化 |
|---|---|---|
| Ant Design 1.x | 2016 | 初始版本 |
| Ant Design 2.x | 2016-12 | 全面重构 |
| Ant Design 3.x | 2017-12 | 引入新设计语言 |
| Ant Design 4.x | 2020-02 | 暗色主题、无障碍、性能优化 |
| **Ant Design 5.x** | **2022-11** | **CSS-in-JS、Design Token 系统、组件级定制** |
| Ant Design 6.x | *未来* | *待定* |

#### AtomUI ↔ Ant Design 版本映射

| AtomUI 版本 | 锚定 Ant Design 版本 | 状态 | 说明 |
|---|---|---|---|
| `0.x.x` | — | 🏁 已结束 | 早期孵化阶段，AtomUI 尚未正式对齐 AntD 版本 |
| `1.x.x` | — | 🏁 已结束 | 早期正式版，AtomUI 架构确立前的过渡版本 |
| **`5.x.x`** | **Ant Design 5.x** | **🟢 当前主线** | 完整实现 AntD 5 Design Token 系统、所有 5.x 组件 |
| `6.x.x` | Ant Design 6.x | 🔮 未来计划 | 待 AntD 6 正式发布后启动 |

> **注意**：AtomUI 没有 `2.x`、`3.x`、`4.x` 版本。这是因为 AtomUI 项目启动时 Ant Design 已经发布到 5.x，所以 AtomUI 从 `0.x` / `1.x`（孵化期）直接跳到 `5.x`，以实现主版本号对齐。

#### MAJOR 版本递增的严格规则

AtomUI 的 MAJOR 版本递增 **仅由 Ant Design 大版本驱动**：

- 当 Ant Design 从 5.x 升级到 6.x 时，AtomUI 同步从 `5.x.x` 升级到 `6.0.0`。
- 这是 MAJOR 递增的 **唯一驱动力**，没有任何其他因素可以独立触发 MAJOR 递增。

**不会触发 MAJOR 递增的情况：**
- AtomUI 自身的功能新增（使用 MINOR 递增）
- AtomUI 自身的 Bug 修复（使用 PATCH 递增）
- **Avalonia 的任何版本升级**（包括 MAJOR 升级，详见 §1.6）

#### AtomUI MINOR/PATCH 与 Ant Design MINOR/PATCH 的关系

AtomUI 的 MINOR 和 PATCH 版本号是 **独立管理的**，不与 Ant Design 的 MINOR/PATCH 一一对应：

```
Ant Design 5.x 系列:  5.0.0  5.1.0  5.2.0 ... 5.12.0 ... 5.22.0
                         │
                         ▼ (AtomUI 锚定 MAJOR=5，MINOR/PATCH 独立演进)
AtomUI   5.x 系列:    5.0.0  5.0.1  5.0.2  5.1.0  5.1.1 ... 5.2.0
```

**原因**：Ant Design（React 实现）的发版节奏非常快（每周 Patch、每月 Minor），而 AtomUI 作为跨平台 .NET 实现有自己的开发节奏和功能里程碑。强行对齐 MINOR/PATCH 会导致版本号跳跃或频繁空发版，没有实际意义。

**但需遵循以下原则：**
- AtomUI 每个 MAJOR 系列必须在该系列生命周期内 **最终实现** 对应 Ant Design 大版本的 **全部组件和设计规范**
- 如果 Ant Design 在某个 Minor 版本中引入了重要的设计变更（如新增 Design Token），AtomUI 应在后续版本中及时跟进

### 1.6 Avalonia 大版本升级的版本处理策略

AtomUI 的每个 MAJOR 系列绑定一个 Avalonia 大版本系列作为 **初始支持版本**，但 Avalonia 的大版本升级 **不会** 触发 AtomUI 的 MAJOR 递增。当 Avalonia 发布新的大版本时，AtomUI 通过 **MINOR 版本升级** 来新增支持。

#### 核心规则

> **AtomUI MAJOR 只跟 Ant Design MAJOR 走。Avalonia 大版本升级 → AtomUI MINOR 升级。**

每个 AtomUI MAJOR 系列内部，Avalonia 大版本的支持范围只增不减：

| AtomUI MAJOR | 初始绑定的 Avalonia 版本 | 后续可新增支持 |
|---|---|---|
| `5.x.x` | Avalonia 11.x | 不会新增 Avalonia 12.x 支持（5.x 系列固定在 Avalonia 11.x） |
| `6.x.x` | Avalonia 12.x | 若 Avalonia 13.x 在 AntD 仍为 6.x 期间发布，AtomUI 通过 MINOR 升级新增支持 |

#### 场景举例

**场景 A：Avalonia 发布新大版本，但 Ant Design 未发布新大版本**

> 假设 Avalonia 发布 v13，但 Ant Design 仍然是 6.x。

- AtomUI **不递增 MAJOR**（因为 Ant Design 没有变）
- AtomUI 在当前 `6.x` 系列中递增 MINOR，发布如 `6.3.0`
- 在 Release Notes 中注明："AtomUI 6.3.0 起新增对 Avalonia 13.x 的支持"
- 对应分支仍在 `release/6.0` 上开发

```
AtomUI 6.0.0 ── 6.1.0 ── 6.2.0 ── 6.3.0 (新增 Avalonia 13 支持) ── 6.3.1 ──►
                                      │
                                Avalonia 13.0 发布
                               Ant Design 仍为 6.x
```

**场景 B：Avalonia 发布新大版本，恰好 Ant Design 也发布新大版本**

> 假设 Avalonia 发布 v13，同时 Ant Design 发布 7.0。

- AtomUI 递增 MAJOR 到 `7.0.0`（因为 Ant Design 升级了）
- 新的 `7.x` 系列从一开始就基于 Avalonia 13.x
- 创建 `release/7.0` 分支

```
AtomUI 6.x 系列 (Avalonia 12.x, AntD 6.x) ── 维护模式
                                                  │
AtomUI 7.0.0 (Avalonia 13.x, AntD 7.x) ─────────┘── 新活跃开发主线
```

**场景 C：当前已有系列（如 5.x）不会跨 Avalonia 大版本**

> AtomUI 5.x 系列固定绑定 Avalonia 11.x，不会在 5.x 中支持 Avalonia 12.x。

- Avalonia 12.x 的支持从 AtomUI `6.0.0` 开始
- 这确保了同一 MAJOR 系列内的 Avalonia 依赖稳定性，用户不会在 PATCH/MINOR 升级中遭遇 Avalonia 大版本切换

#### 总结表

| 事件 | AtomUI 版本动作 | 说明 |
|---|---|---|
| Avalonia PATCH 升级 (11.3.x → 11.3.y) | 更新 `AvaloniaVersion`，AtomUI 版本不变 | 最小变更 |
| Avalonia MINOR 升级 (11.2 → 11.3) | AtomUI MINOR 或 PATCH 升级 | 视影响范围而定 |
| Avalonia MAJOR 升级 (12 → 13)，AntD 未变 | **AtomUI MINOR 升级** | 在当前 MAJOR 系列内新增支持 |
| Avalonia MAJOR 升级 + AntD MAJOR 升级同时发生 | **AtomUI MAJOR 升级**（跟随 AntD） | 新 MAJOR 系列直接基于新 Avalonia |

### 1.3 预发布版本标签

预发布版本遵循 SemVer 的预发布标识符规范，按稳定性递增排列：

| 标签 | 格式 | 用途 | NuGet 排序 |
|---|---|---|---|
| `build` | `X.Y.Z-build.N` | **内部开发构建**，仅用于团队内部测试和 CI，不对外发布 | 最低 |
| `alpha` | `X.Y.Z-alpha.N` | **Alpha 预览版**，功能不完整，API 可能大幅变动 | ↓ |
| `beta` | `X.Y.Z-beta.N` | **Beta 测试版**，功能基本完整，API 趋于稳定 | ↓ |
| `rc` | `X.Y.Z-rc.N` | **发布候选版**，除关键 Bug 外不再变更 | ↓ |
| *(无)* | `X.Y.Z` | **正式发布版** (Stable Release) | 最高 |

**示例版本演进：**
```
5.2.0-build.1 → 5.2.0-build.2 → ... → 5.2.0-alpha.1 → 5.2.0-alpha.2 → 5.2.0-beta.1 → 5.2.0-rc.1 → 5.2.0
```

**规则：**
- `N` 为从 1 开始的递增整数（如 `alpha.1`, `alpha.2`）
- `build` 标签的内部构建版本不发布到公共 NuGet，仅推送到本地/私有源
- 同一预发布阶段内若需修复问题，递增 `N`
- 预发布标签中 **不得** 包含日期或 commit hash（这些信息放入 `+BUILD` 元数据）

### 1.4 版本号递增规则

| 场景 | 递增规则 | 示例 |
|---|---|---|
| Bug 修复、文档修正、无功能变更 | `PATCH += 1` | `5.1.3` → `5.1.4` |
| 新增控件、新增 API、新增功能（向后兼容） | `MINOR += 1, PATCH = 0` | `5.1.4` → `5.2.0` |
| 破坏性变更、AntD 大版本对齐 | `MAJOR = AntD版本, MINOR = 0, PATCH = 0` | `5.2.0` → `6.0.0` |
| 预发布版 Bug 修复 | 递增预发布计数器 `N` | `5.2.0-beta.1` → `5.2.0-beta.2` |
| 预发布阶段升级 | 切换到下一个阶段标签 | `5.2.0-beta.3` → `5.2.0-rc.1` |

### 1.5 版本号在代码中的管理

所有版本号统一定义在 `build/Version.props` 中：

```xml
<Project>
    <PropertyGroup>
        <AvaloniaVersion>11.3.12</AvaloniaVersion>
        <AtomUIVersion>5.2.0-build.3</AtomUIVersion>
        <AtomUIGalleryVersion>5.2.0-build.3</AtomUIGalleryVersion>
    </PropertyGroup>
</Project>
```

**规则：**
- `AtomUIVersion` 和 `AtomUIGalleryVersion` **必须保持同步**
- 版本更新通过修改此文件完成，不允许在单个项目 `.csproj` 中硬编码版本号
- `AvaloniaVersion` 记录当前依赖的 Avalonia 版本，用于跨团队协调

---

## 2. Git 分支策略

AtomUI 采用 **Release Branch 模型**（与 Avalonia 一致），以 `release/X.Y` 分支作为开发主线。

### 2.1 分支类型总览

```
长期分支 (Long-lived)
├── main                      # 历史归档主干（不活跃开发）
├── release/5.0               # 5.0.x 系列稳定维护线（当前活跃开发主线）
├── release/6.0               # 6.0.x 系列开发线（下一大版本）
│
短期分支 (Short-lived)
├── feature/<name>            # 功能开发分支
├── bugfix/<name>             # Bug 修复分支
├── hotfix/<name>             # 紧急修复分支（针对已发布版本）
└── docs/<name>               # 文档专用分支
```

### 2.2 长期分支

#### `main`

| 属性 | 值 |
|---|---|
| **用途** | 项目历史归档主干 |
| **保护** | 受保护分支，禁止直接推送 |
| **合并来源** | 里程碑发布时由 `release/X.Y` 合并入 |
| **说明** | 早期项目使用 `main` 作为活跃开发主线。在切换到 Release Branch 模型后，`main` 保留为历史归档，所有活跃开发转移到 `release/X.Y` 分支 |

#### `release/X.Y` — **活跃开发主线**

| 属性 | 值 |
|---|---|
| **命名** | `release/{MAJOR}.{MINOR}` — 如 `release/5.0`, `release/6.0` |
| **用途** | 该 MAJOR.MINOR 系列的所有开发、发布、维护活动的主线 |
| **保护** | 受保护分支，禁止直接推送，必须通过 PR 合并 |
| **生命周期** | 从首个预发布版创建，到该系列停止维护 (EOL) |
| **从何分叉** | 新 MINOR：从前一个 `release/X.{Y-1}` 分叉；新 MAJOR：从前一个 `release/{X-1}.Y` 最新稳定点分叉 |

**与 Avalonia 模型对比：**

| 特性 | Avalonia | AtomUI |
|---|---|---|
| 活跃开发主线 | `master` (下一大版本) | `release/X.Y` (当前活跃版本) |
| 发布分支 | `release/11.3`, `release/11.3.12` | `release/5.0` (整个 MAJOR.MINOR 系列) |
| 每个 PATCH 独立分支 | ✅ `release/11.3.12` | ❌ 在 `release/X.Y` 上打 Tag |
| 预发布分支 | ✅ `release/11.3.0-beta1` | ❌ 在 `release/X.Y` 上打 Tag |

**简化说明：** AtomUI 项目规模较 Avalonia 小，因此不为每个 PATCH 或预发布版本创建独立分支。所有 `X.Y.Z` 版本均在 `release/X.Y` 分支上通过 **Git Tag** 标记发布点。

### 2.3 短期分支

#### `feature/<name>` — 功能开发

| 属性 | 值 |
|---|---|
| **命名** | `feature/<描述性名称>` — 如 `feature/qrcode`, `feature/tour`, `feature/dark-theme` |
| **从何分叉** | 从目标 `release/X.Y` 分叉 |
| **合并目标** | 合并回对应的 `release/X.Y` |
| **生命周期** | 开发完成并合并后删除 |

**命名规则：**
- 使用小写英文 + 连字符分隔：`feature/color-picker`
- 若关联 Issue，可包含编号：`feature/123-color-picker`
- 保持简短、描述性：不超过 3~4 个单词

#### `bugfix/<name>` — Bug 修复

| 属性 | 值 |
|---|---|
| **命名** | `bugfix/<描述性名称>` — 如 `bugfix/memoryleak`, `bugfix/input-focus` |
| **从何分叉** | 从目标 `release/X.Y` 分叉 |
| **合并目标** | 合并回对应的 `release/X.Y` |
| **生命周期** | 修复完成并合并后删除 |

#### `hotfix/<name>` — 紧急修复

| 属性 | 值 |
|---|---|
| **命名** | `hotfix/<描述性名称>` — 如 `hotfix/crash-on-startup` |
| **从何分叉** | 从最近的正式发布 Tag（如 `v5.1.4`）分叉 |
| **合并目标** | 合并回 `release/X.Y`，并打新的 PATCH Tag |
| **用途** | 仅用于已发布正式版本的紧急 Bug 修复，需要立即发布新 PATCH |
| **生命周期** | 修复、发布后立即删除 |

#### `docs/<name>` — 文档分支

| 属性 | 值 |
|---|---|
| **命名** | `docs/<描述性名称>` — 如 `docs/api-reference`, `docs/getting-started` |
| **从何分叉** | 从目标 `release/X.Y` 分叉 |
| **合并目标** | 合并回对应的 `release/X.Y` |

### 2.4 分支流转示意

```
main ─────────────────────────────────────────────────────────────────────────
  │                                            ▲ (milestone merge)
  │                                            │
  └── release/5.0 ─── • ─── • ─── • ─── • ─── • ─── • ─── • ─── • ───►
        │   ▲            │   ▲       │   ▲         ▲
        │   │            │   │       │   │         │
        │   └── feature/qrcode      │   │         │
        │                │   │       │   │         │
        │                │   └── bugfix/memoryleak │
        │                │                         │
        │                Tag: v5.1.4               Tag: v5.2.0
        │
        └──── release/6.0 ─── • ─── • ─── • ───►
                   │   ▲
                   │   │
                   └── feature/tour
```

### 2.5 Tag 命名规范

所有正式发布和预发布均通过 **Git Tag** 标记：

| Tag 格式 | 示例 | 说明 |
|---|---|---|
| `v{MAJOR}.{MINOR}.{PATCH}` | `v5.1.4` | 正式发布版 |
| `v{MAJOR}.{MINOR}.{PATCH}-{PRERELEASE}` | `v5.2.0-beta.1` | 预发布版 |
| `v{MAJOR}.{MINOR}.{PATCH}-build.{N}` | `v5.2.0-build.3` | 内部构建版（可选，用于 CI） |

**规则：**
- Tag **必须** 以 `v` 前缀开头（保持与历史 Tag 一致）
- Tag 必须在 `release/X.Y` 分支上创建
- Tag 创建后 **不得** 删除或移动（除明显错误外）
- 正式发布 Tag 必须是 **Annotated Tag**，包含发布说明

**创建 Tag 示例：**
```bash
# 正式发布
git tag -a v5.2.0 -m "Release v5.2.0: ..."

# 预发布
git tag -a v5.2.0-rc.1 -m "Release Candidate v5.2.0-rc.1"

# 内部构建（Lightweight Tag 即可）
git tag v5.2.0-build.3
```

---

## 3. 发布流程

### 3.1 正式版本发布流程

```
1. 在 release/X.Y 分支上确认所有功能和修复已合并
2. 更新 build/Version.props 中的版本号（移除预发布标签）
3. 更新 CHANGELOG.md
4. 提交版本变更：git commit -m "chore(release): bump version to X.Y.Z"
5. 创建 Annotated Tag：git tag -a vX.Y.Z -m "Release vX.Y.Z"
6. 推送：git push origin release/X.Y --tags
7. CI/CD 自动构建并发布到 NuGet
8. （里程碑发布时）将 release/X.Y 合并到 main
```

### 3.2 预发布版本发布流程

```
1. 在 release/X.Y 分支上更新 build/Version.props（如 5.2.0-beta.1）
2. 提交：git commit -m "chore(release): bump version to 5.2.0-beta.1"
3. 创建 Tag：git tag -a v5.2.0-beta.1 -m "Pre-release v5.2.0-beta.1"
4. 推送：git push origin release/X.Y --tags
5. CI/CD 构建并发布到 NuGet（标记为 Pre-release）
```

### 3.3 Hotfix 发布流程

```
1. 从最近的正式版 Tag 创建 hotfix 分支：
   git checkout -b hotfix/critical-bug v5.1.4
2. 修复问题并提交
3. 更新 build/Version.props：5.1.4 → 5.1.5
4. 通过 PR 合并回 release/5.0
5. 在 release/5.0 上创建 Tag：v5.1.5
6. 推送并删除 hotfix 分支
```

---

## 4. 多版本并行维护策略

当存在多个活跃的 `release/X.Y` 分支时（如 `release/5.0` 和 `release/6.0`）：

### 4.1 维护窗口

| 分支 | 状态 | 接受的变更 |
|---|---|---|
| `release/6.0` | **活跃开发** (Active) | 新功能 + Bug 修复 + 文档 |
| `release/5.0` | **维护模式** (Maintenance) | 仅 Bug 修复和安全补丁 |
| `release/1.0` | **停止维护** (EOL) | 不再接受任何变更 |

### 4.2 变更回移 (Backport)

当 Bug 修复需要应用到多个维护版本时：

1. 在最新的活跃开发分支（如 `release/6.0`）上首先修复
2. 使用 `cherry-pick` 回移到维护分支（如 `release/5.0`）
3. 在维护分支上发布新的 PATCH 版本

---

## 5. 上下游版本依赖关系

AtomUI 处于 Ant Design（设计规范上游）和 Avalonia（技术框架上游）之间，三者的版本关系如下：

```
┌─────────────────────┐     设计规范锚定      ┌─────────────────────┐     技术框架依赖      ┌─────────────────────┐
│    Ant Design 5.x   │ ◄══════════════════► │    AtomUI 5.x.x     │ ──────────────────► │   Avalonia 11.x     │
│   (设计语言上游)      │    MAJOR 严格一致     │   (本项目)            │    运行时依赖         │   (UI 框架上游)      │
│   React 实现         │                      │   .NET/Avalonia 实现  │                     │   .NET 跨平台 UI     │
└─────────────────────┘                       └─────────────────────┘                      └─────────────────────┘
```

### 5.1 AtomUI ↔ Ant Design 版本关系（设计规范锚定）

> 详见 §1.2。此处为总结。

| 关系维度 | 规则 |
|---|---|
| MAJOR | **严格锚定** — `AtomUI MAJOR == Ant Design MAJOR` |
| MINOR / PATCH | **独立管理** — AtomUI 有自己的开发节奏 |
| 设计规范 | AtomUI 每个 MAJOR 系列最终须完整实现对应 AntD 大版本的全部组件和 Design Token |
| 新版本启动 | AntD 发布新 MAJOR 后，AtomUI 在 `release/{新MAJOR}.0` 分支启动对应开发 |

### 5.2 AtomUI ↔ Avalonia 版本关系（技术框架依赖）

> 详见 §1.6。此处为总结。

每个 AtomUI MAJOR 系列绑定一个 Avalonia 大版本作为初始支持版本。Avalonia 大版本升级不触发 AtomUI MAJOR 递增，而是通过 MINOR 升级在当前 MAJOR 系列内新增支持。

| AtomUI 版本 | Avalonia 版本 | .NET 版本 | 说明 |
|---|---|---|---|
| `5.0.x` ~ `5.2.x` | Avalonia 11.x | .NET 8 / .NET 10 | 5.x 系列固定绑定 Avalonia 11.x |
| `6.0.x` (计划) | Avalonia 12.x (计划) | .NET 10+ | 6.x 系列初始绑定 Avalonia 12.x |
| `6.N.x` (假设) | Avalonia 13.x (假设) | .NET 10+ | 若 AntD 仍为 6.x 时 Avalonia 13 发布，MINOR 升级新增支持 |
| `7.0.x` (假设) | Avalonia 13.x+ (假设) | — | 若 AntD 7.0 与 Avalonia 新大版本同期发布 |

**规则：**
- Avalonia 的 **PATCH** 升级 — 更新 `build/Version.props` 中的 `AvaloniaVersion`，**不影响** AtomUI 版本号
- Avalonia 的 **MINOR** 升级 — 通常对应 AtomUI 的 MINOR 或 PATCH 升级，视影响范围而定
- Avalonia 的 **MAJOR** 升级 — **不触发** AtomUI MAJOR 递增；在当前 MAJOR 系列内通过 MINOR 升级新增支持（或在新 AntD MAJOR 同期发布时合并处理）
- 同一 AtomUI MAJOR 系列内，早期版本 **不会** 跨 Avalonia 大版本（如 5.x 不会支持 Avalonia 12.x）

---

## 6. 总结对照表

### 分支命名速查

| 分支类型 | 命名格式 | 示例 |
|---|---|---|
| 归档主干 | `main` | `main` |
| 发布主线 | `release/{MAJOR}.{MINOR}` | `release/5.0`, `release/6.0` |
| 功能分支 | `feature/{name}` | `feature/qrcode` |
| 修复分支 | `bugfix/{name}` | `bugfix/memoryleak` |
| 紧急修复 | `hotfix/{name}` | `hotfix/crash-on-startup` |
| 文档分支 | `docs/{name}` | `docs/api-reference` |

### 版本 Tag 速查

| 类型 | Tag 格式 | 示例 |
|---|---|---|
| 正式版 | `v{MAJOR}.{MINOR}.{PATCH}` | `v5.2.0` |
| Alpha | `v{M}.{m}.{p}-alpha.{N}` | `v5.2.0-alpha.1` |
| Beta | `v{M}.{m}.{p}-beta.{N}` | `v5.2.0-beta.1` |
| RC | `v{M}.{m}.{p}-rc.{N}` | `v5.2.0-rc.1` |
| 内部构建 | `v{M}.{m}.{p}-build.{N}` | `v5.2.0-build.3` |

### 完整版本生命周期

```
feature/xxx ─┐
bugfix/xxx  ─┤
docs/xxx    ─┴──► release/5.0 ──► v5.2.0-build.1
                                   v5.2.0-build.2
                                   v5.2.0-alpha.1
                                   v5.2.0-beta.1
                                   v5.2.0-rc.1
                                   v5.2.0         ──► NuGet (stable)
                                                  ──► merge to main (milestone)
```


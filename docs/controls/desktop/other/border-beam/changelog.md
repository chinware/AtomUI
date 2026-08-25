# BorderBeam Changelog

本文档记录 BorderBeam 控件级设计、API、主题契约、Token 和实现结构的变化。
它不替代仓库根目录 CHANGELOG.md，也不作为正式版本发布说明。

## 2026-08-25

- Fixed
  - Pause BorderBeam's infinite Avalonia animation when the control is effectively invisible.
- Docs
  - Clarify that invisible includes hidden Visual ancestors, not only `BorderBeam.IsVisible=false`.

## 2026-07-14

- Theme
  - 取消默认主题对 `SharedToken.EnableMotion` 的绑定，使 BorderBeam 流光默认不受全局 motion 开关影响。
  - 保留实例级 `IsMotionEnabled=false` 关闭流光的行为。
- Docs
  - 更新 Gallery 和控件文档中关于 motion 默认语义的说明。

## 2026-06-26

- Docs
  - Add LLMS metadata, semantic parts and export source mapping for `BorderBeam`.
  - Align generated output paths with `controls/border-beam/index-cn.md` and `controls/border-beam/semantic-cn.md`.

## 2026-06-19

- Docs
  - 新增 `implementation.md`，记录 BorderBeam 边界感知、颜色归一、动画生命周期、连续流光渲染和维护不变量。
  - 将 `overview.md` 收敛为装饰控件定位、公共契约、状态模型、视觉主题模型和验证入口。
  - 在 Other 分类入口中登记 BorderBeam 实现原理文档。

## 2026-06-18

- Docs
  - 新增 BorderBeam 桌面版架构设计文档。
  - 新增 BorderBeam Token 设计文档。
  - 将 BorderBeam 归入桌面控件 `Other` 分类，并在 Gallery 中使用 `Other` 顶层分类承载。
- API
  - 确立 `BorderBeam` 独立包装控件模型。
  - 确立 `IBorderBeamAwareControl` 与 `BorderBeamGeometry` 作为被装饰控件边界感知契约。
  - 确立 `ColorStops` 使用 `DirectProperty` 暴露实例级可变集合，避免共享默认集合。
- Theme
  - 确立 `PART_ContentPresenter` 与 `PART_BeamPresenter` 的模板结构。
  - 确立 BorderBeam 默认不使用全局 AdornerLayer，不修改被装饰控件模板。
  - 落地 `BorderBeamTheme.axaml`，将默认颜色、边框、圆角和动效参数映射到 SharedToken 与 BorderBeamToken。
- Token
  - 确立 BorderBeamToken 只承载 beam 自有尺寸、透明度、动效时长和渐变映射参数。
  - 确立颜色、线宽、圆角和 motion 开关优先复用 SharedToken。

# 渲染系统

本目录定义跨 Control、跨 VisualRoot 的稳定渲染契约。具体 Control 的模板与绘制实现仍由对应 Control 文档拥有；
本目录只维护共享层模型、宿主选择、物理像素适配、生命周期和验证不变量。

- [边框渲染](border-rendering.md)：设计厚度、render scale 换算、圆角绘制、缓存和验证规则。
- [视觉层](visual-layers.md)：Popup、Overlay、Adorner、反馈层和作用域层的职责与选择规则。
- [整体架构](../../index.md)

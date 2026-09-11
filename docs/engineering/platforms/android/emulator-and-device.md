# Android Emulator 与真机

本文定义 AVD、Emulator 和 physical device 的发现、部署、启动、日志、截图与场景验证责任。当前没有已执行 Host 命令，
所以只描述必须证明的操作和证据，不提供假定可运行的命令块。

## 操作责任

| 操作 | 必须输出的证据 |
| --- | --- |
| AVD/device discovery | Device identity、API/runtime、ABI、屏幕/密度、状态和连接方式 |
| Emulator boot | Boot 完成状态、system image、冷/热启动定义和日志位置 |
| Package install | 构建配置、Application ID、artifact identity、安装结果和覆盖/卸载策略 |
| App launch | Activity/Host 启动结果、process/log identity 和首帧场景 |
| Log capture | 时间范围、process/tag 过滤、exception/ANR 和 adapter lifecycle 事件 |
| Screenshot/video | Device、方向、主题、字体缩放、system bars、IME 与场景名称 |

真实 Host 落地后，项目文档可以补充已执行的 device/emulator 工具命令；在此之前不能把工具名称推导成验证通过。

## Emulator 场景

至少覆盖：

- Portrait/landscape、display density 和字体缩放。
- Status/navigation bars、edge-to-edge 与 Safe Area/available bounds 投射。
- System back 与 Overlay-before-page-back 优先级。
- IME 显示/隐藏、组合输入、遮挡、焦点和 return action。
- 前后台、process/Activity recreation 和恢复顺序。
- Multi-window 或可调整窗口下的 viewport snapshot。
- TalkBack 基础可达性，但最终屏幕阅读器状态仍需真机证据。

AVD 选择要对应具体风险，例如 API/runtime 差异、system bar 模式、ABI、屏幕尺寸或重建场景；不能把单一 Emulator 结果写成
全部 Android 设备支持。

## 真机场景

真机证据至少包含：

- 触摸、nested scroll、velocity/snap、多指与 cancel。
- 真实 IME、中文/日文组合输入、emoji、secure input 和键盘切换。
- System back、gesture navigation 与三键导航的适用差异。
- Status/navigation bars、cutout、旋转、字体缩放和厂商系统差异。
- Foreground/background、锁屏、process death 和 Activity recreation。
- Haptic 可用/不可用退化。
- TalkBack 阅读顺序、焦点、role/name/value/state/action 和 modal focus trap。

## Activity Recreation

重建验证必须证明：

1. 新 Host/TopLevel 创建新的 Runtime scope 和 adapter subscription。
2. 旧 scope、Overlay session、pointer chain、timer 和 owner 引用被释放。
3. Navigation/Overlay 只恢复批准的稳定状态，不重复 push、opened event 或 Task completion。
4. Viewport、system bars 和 input pane snapshot 在恢复交互前刷新。
5. Theme、Localization 和结构化 Gallery 状态保持一致。

## 证据状态

Emulator、physical device、TalkBack 和 Activity recreation 分别记录。当前这些状态均为 `未验证`；Headless tests、iOS 设备或
普通 Android compile 不能填充任何一列。

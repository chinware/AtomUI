# 架构基础

本目录保存对整个 AtomUI 解决方案生效的基础架构边界。

- [项目依赖关系](dependency-graph.md)：解决方案项目、主要项目引用和内部可见性。
- [运行平台策略](runtime-platforms.md)：Desktop、Browser、Native 和 Mobile 的能力边界。
- [启动与注册链路](startup-and-registration.md)：AppBuilder、Application、Builder、Provider 和生成池的注册顺序。
- [构建与打包](build-and-packaging.md)：Target Framework、版本、Analyzer、生成输出和 NuGet 包边界。
- [AOT 与裁剪架构](aot-and-trimming.md)：linked publish、Application Plan、Package Core、安全 fallback 和体积验证契约。
- [AOT Registration Unit 粒度](aot-registration-unit-granularity.md)：Package/Directory 粒度、资源归属、第一方包策略和第三方接入边界。

具体跨模块业务系统进入 `architecture/systems/`；单个项目的源码组织进入 `modules/`。

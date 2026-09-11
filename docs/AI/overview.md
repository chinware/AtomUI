# AI 文档

本目录承载面向 AI 工具的文档消费层、生成配置和生成产物。它不拥有 AtomUI 的架构、Control 或工程契约；
内容不足或不一致时，应回到对应正式文档、结构化输入或 Generator 修正。

- [Generated Documentation](generated/overview.md)：当前 AtomUI Desktop LLMS 配置、聚合索引和单 Control 生成产物。

未来 AtomUI Mobile 使用独立配置和 `docs/AI/generated/mobile-llms/` 输出，不能与 Desktop 同名 Control 共享目录。该能力属于
Mobile Foundation，当前未实现，也未创建配置或生成产物；隔离契约见
[LLMS Generator 设计](../engineering/tooling/llms-generator-design.md#51-mobile-llms-隔离目标)。

# Generated Documentation

本目录承载由工具生成或配置驱动的文档。当前配置和生成集属于 AtomUI Desktop。生成产物不直接编辑；内容不足或过期时，
应修改 Desktop 源文档、结构化输入、Generator 或配置后重新生成。

- [Desktop LLMS 配置](llms.config.json)
- [Desktop LLMS 输出](llms/llms.txt)
- [LLMS Generator 设计](../../engineering/tooling/llms-generator-design.md)

```bash
dotnet run --project tools/AtomUI.Docs.LLMsGenerator/AtomUI.Docs.LLMsGenerator.csproj -- generate --config docs/AI/generated/llms.config.json
dotnet run --project tools/AtomUI.Docs.LLMsGenerator/AtomUI.Docs.LLMsGenerator.csproj -- verify --config docs/AI/generated/llms.config.json
```

未来 Mobile 使用独立的 `docs/AI/generated/mobile-llms.config.json` 和 `docs/AI/generated/mobile-llms/` ownership。它们当前不存在，
本页不提供不存在路径的链接，也不允许手工创建空配置或输出；Foundation 实现要求见
[Mobile LLMS 隔离目标](../../engineering/tooling/llms-generator-design.md#51-mobile-llms-隔离目标)。

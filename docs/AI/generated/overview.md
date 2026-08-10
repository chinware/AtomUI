# Generated Documentation

本目录承载由工具生成或配置驱动的文档。生成产物不直接编辑；内容不足或过期时，应修改源文档、结构化输入、
Generator 或配置后重新生成。

- [LLMS 配置](llms.config.json)
- [LLMS 输出](llms/llms.txt)
- [LLMS Generator 设计](../../engineering/tooling/llms-generator-design.md)

```bash
dotnet run --project tools/AtomUI.Docs.LLMsGenerator/AtomUI.Docs.LLMsGenerator.csproj -- generate --config docs/AI/generated/llms.config.json
dotnet run --project tools/AtomUI.Docs.LLMsGenerator/AtomUI.Docs.LLMsGenerator.csproj -- verify --config docs/AI/generated/llms.config.json
```

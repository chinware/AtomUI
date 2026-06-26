using AtomUI.Docs.LLMsGenerator.Markdown;
using Shouldly;
using Xunit;

namespace AtomUI.Docs.LLMsGenerator.Tests;

public class MarkdownSectionParserTests
{
    [Fact]
    public void ParserExtractsHeadingsAndIgnoresFencedCodeBlocks()
    {
        const string markdown = """
        # Button

        intro

        ## 概述

        overview body

        ```md
        ## Not A Heading
        ```

        ## 状态模型

        state body
        """;

        var document = MarkdownSectionParser.Parse(markdown, "button.md");

        document.Title.ShouldBe("Button");
        document.Sections.Select(section => section.Heading).ShouldBe(["概述", "状态模型"]);
        document.GetRequiredSection("概述").Content.Trim().ShouldBe("overview body\r\n\r\n```md\r\n## Not A Heading\r\n```".ReplaceLineEndings());
        document.GetRequiredSection("状态模型").Content.Trim().ShouldBe("state body");
    }

    [Fact]
    public void RequiredSectionReportsFilePathAndHeading()
    {
        var document = MarkdownSectionParser.Parse("# Button\n\n## 概述\n\nbody", "docs/button.md");

        var exception = Should.Throw<MarkdownSectionMissingException>(() =>
            document.GetRequiredSection("状态模型"));

        exception.Message.ShouldContain("docs/button.md");
        exception.Message.ShouldContain("状态模型");
    }
}

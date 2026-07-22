using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.BorderBeam;

[LanguageProvider(LanguageCode.zh_TW, BorderBeamShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string ScenarioExamples = "範例";
    public const string ComponentCategory = "其他";
    public const string ComponentIntroducedVersion = "v6.0.5";
    public const string PageSubtitle = "沿容器邊界繪製動態高光。";
    public const string PageDescription = "BorderBeam 包裹一個內容控制項，並在其邊界上渲染不參與互動的流光層。它只承擔裝飾性強調，預設不受全域動效設定影響，並支援單色或漸變停靠點。";
    public const string BasicTitle = "基礎";
    public const string BasicDescription = "包裹卡片以強調重要的工作台概覽，同時不改變卡片自身互動模型。";
    public const string CustomizedColorTitle = "自訂顏色";
    public const string CustomizedColorDescription = "使用多個顏色停靠點建立不同流光預設，對齊 Ant Design 的自訂顏色範例。";
    public const string CustomizedColorCardDescription = "分段選擇器會切換流光使用的顏色停靠點集合。";
    public const string NonUniformRadiusTitle = "非統一圓角";
    public const string NonUniformRadiusDescription = "被裝飾容器自行裁剪圓角時，可將 Outset 設定為 0。";
    public const string NonUniformRadiusCardDescription = "頂部圓角保持較大半徑，底部角保持直角。";

}

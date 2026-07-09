using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.BorderBeam;

[LanguageProvider(LanguageCode.zh_TW, BorderBeamShowCase.LanguageId)]
internal partial class zh_TW
{
    public const string ScenarioExamples = "範例";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "設計變數";
    public const string ComponentCategory = "其他";
    public const string ComponentIntroducedVersion = "v6.0.5";
    public const string PageSubtitle = "沿容器邊界繪製動態高光。";
    public const string PageDescription = "BorderBeam 包裹一個內容控制項，並在其邊界上渲染不參與互動的流光層。它只承擔裝飾性強調，遵守主題動效設定，並支援單色或漸變停靠點。";
    public const string BasicTitle = "基礎";
    public const string BasicDescription = "包裹卡片以強調重要的工作台概覽，同時不改變卡片自身互動模型。";
    public const string CustomizedColorTitle = "自訂顏色";
    public const string CustomizedColorDescription = "使用多個顏色停靠點建立不同流光預設，對齊 Ant Design 的自訂顏色範例。";
    public const string CustomizedColorCardDescription = "分段選擇器會切換流光使用的顏色停靠點集合。";
    public const string NonUniformRadiusTitle = "非統一圓角";
    public const string NonUniformRadiusDescription = "被裝飾容器自行裁剪圓角時，可將 Outset 設定為 0。";
    public const string NonUniformRadiusCardDescription = "頂部圓角保持較大半徑，底部角保持直角。";
    public const string ApiColumnProperty = "屬性";
    public const string ApiColumnDescription = "說明";
    public const string ApiColumnType = "型別";
    public const string ApiColumnDefault = "預設值";
    public const string ApiPropertyColor = "單色流光顏色。ColorStops 非空時優先使用 ColorStops。";
    public const string ApiPropertyColorStops = "漸變停靠點集合。Percent 使用公開的 0-100 區間，並映射到可見流光段。";
    public const string ApiPropertyOutset = "流光相對有效邊界的外擴距離。為 null 時使用有效邊框厚度。";
    public const string ApiPropertyBorderThickness = "內容未暴露 BorderBeam 幾何時使用的兜底邊框厚度。";
    public const string ApiPropertyCornerRadius = "內容未暴露 BorderBeam 幾何時使用的兜底圓角。";
    public const string ApiPropertyIsMotionEnabled = "控制流光動畫是否啟用。";
    public const string ApiPropertyDuration = "流光完成一周運動的時長。";
    public const string ApiPropertyBeamSize = "移動高光段的基準尺寸。";
    public const string TokenColumnToken = "變數";
    public const string TokenColumnDescription = "說明";
    public const string TokenColumnScope = "作用域";
    public const string TokenColumnStatus = "狀態";
    public const string TokenNameBeamSize = "移動高光段的預設尺寸。";
    public const string TokenNameBeamOpacity = "流光層預設透明度。";
    public const string TokenNameMotionDuration = "流光完成一周運動的預設時長。";
    public const string TokenNameMaxVisibleStopPercent = "用於為透明尾跡保留空間的最大可見停靠點百分比。";
    public const string TokenScopeComponent = "組件";
    public const string TokenStatusStable = "穩定";

}

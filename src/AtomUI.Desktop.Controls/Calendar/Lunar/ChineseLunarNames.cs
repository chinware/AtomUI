namespace AtomUI.Desktop.Controls.Internal.Calendar.Lunar;

internal static class ChineseLunarNames
{
    internal static IReadOnlyList<string> SolarTerms { get; } =
    [
        "小寒", "大寒", "立春", "雨水", "惊蛰", "春分", "清明", "谷雨", "立夏", "小满", "芒种", "夏至",
        "小暑", "大暑", "立秋", "处暑", "白露", "秋分", "寒露", "霜降", "立冬", "小雪", "大雪", "冬至"
    ];

    internal static IReadOnlyList<string> Festivals { get; } =
    [
        "春节", "元宵节", "龙抬头", "端午节", "七夕", "中元节", "中秋节", "重阳节", "腊八节", "除夕", "清明"
    ];

    internal static IReadOnlyList<string> Zodiacs { get; } =
        ["鼠", "牛", "虎", "兔", "龙", "蛇", "马", "羊", "猴", "鸡", "狗", "猪"];

    internal static IReadOnlyList<string> HeavenlyStems { get; } =
        ["甲", "乙", "丙", "丁", "戊", "己", "庚", "辛", "壬", "癸"];

    internal static IReadOnlyList<string> EarthlyBranches { get; } =
        ["子", "丑", "寅", "卯", "辰", "巳", "午", "未", "申", "酉", "戌", "亥"];

    internal static IReadOnlyList<string> LunarMonths { get; } =
        ["正", "二", "三", "四", "五", "六", "七", "八", "九", "十", "冬", "腊"];

    internal static IReadOnlyList<string> LunarDays { get; } =
    [
        "初一", "初二", "初三", "初四", "初五", "初六", "初七", "初八", "初九", "初十",
        "十一", "十二", "十三", "十四", "十五", "十六", "十七", "十八", "十九", "二十",
        "廿一", "廿二", "廿三", "廿四", "廿五", "廿六", "廿七", "廿八", "廿九", "三十"
    ];

    internal const string LeapMonthPrefix = "闰";
    internal const string LunarMonthSuffix = "月";
}

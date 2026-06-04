namespace AtomUI.Theme.Language;

public enum LanguageCode
{
    zh_CN,
    zh_TW,
    en_US,
}

public static class LanguageCodeExtensions
{
    public static string ToHyphenString(this LanguageCode code)
    {
        return code switch
        {
            LanguageCode.zh_CN => "zh-CN",
            LanguageCode.zh_TW => "zh-TW",
            LanguageCode.en_US => "en-US",
            _ => code.ToString().Replace('_', '-')
        };
    }
}

namespace AtomUI.Theme.Language;

public enum LanguageCode
{
    zh_CN,
    en_US,
}

public static class LanguageCodeExtensions
{
    public static string ToHyphenString(this LanguageCode code)
    {
        return code switch
        {
            LanguageCode.zh_CN => "zh-CN",
            LanguageCode.en_US => "en-US",
            _ => code.ToString().Replace('_', '-')
        };
    }
}

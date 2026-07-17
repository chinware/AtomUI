namespace AtomUI.Desktop.Controls;

public enum StepsStatus
{
    Wait,
    Process,
    Finish,
    Error
}

public enum StepsType
{
    Default,
    Dot,
    Navigation,
    Inline
}

internal enum StepsItemLayoutRole
{
    Indicator,
    Header,
    SubHeader,
    Connector,
    Content,
    NavigationArrow,
    NavigationActiveIndicator
}

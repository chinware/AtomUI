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
    OutlineDot,
    Navigation,
    Inline
}

internal enum StepsItemLayoutRole
{
    ItemWrapper,
    Indicator,
    Header,
    SubHeader,
    Connector,
    Content,
    NavigationArrow,
    NavigationActiveIndicator
}

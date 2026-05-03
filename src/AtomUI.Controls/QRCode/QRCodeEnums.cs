namespace AtomUI.Controls;

/// <summary>
/// 二维码纠错等级
/// </summary>
public enum QRCodeEccLevel
{
    L = 0,
    M = 1,
    Q = 2,
    H = 3
}

/// <summary>
/// QRCode状态
/// </summary>
public enum QRCodeStatus
{
    Active = 0,
    Expired,
    Loading,
    Scanned,
}
using AtomUI.Controls.Commons;
using AtomUI.Theme;

namespace AtomUI.Desktop.Controls;

public class QRCode : AbstractQRCode
{
    public QRCode()
    {
        this.RegisterTokenResourceScope(QRCodeToken.ScopeProvider);
    }
}
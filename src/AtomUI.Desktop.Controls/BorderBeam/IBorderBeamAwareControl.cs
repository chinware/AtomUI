using System;

namespace AtomUI.Desktop.Controls;

public interface IBorderBeamAwareControl
{
    event EventHandler? BorderBeamGeometryChanged;

    BorderBeamGeometry GetBorderBeamGeometry();
}

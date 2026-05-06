using Avalonia;
using Avalonia.Controls.Primitives.PopupPositioning;

namespace AtomUI.Desktop.Controls;

public delegate void CustomPlacementCallback(CustomPopupPlacement parameters,
                                             Thickness shadowThickness,
                                             double marginToAnchor,
                                             bool isUseOverlayHost, 
                                             bool isHorizontalFlipped, 
                                             bool isVerticalFlipped);
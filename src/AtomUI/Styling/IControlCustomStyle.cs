using Avalonia;

namespace AtomUI.Styling;

internal interface IControlCustomStyle
{
   void InitOnConstruct() {}
   void SetupUi();
   void AfterUiStructureReady() {}
   void SetupTransitions() {}
   void CollectStyleState() {}
   void ApplyVariableStyleConfig() {}
   void ApplyFixedStyleConfig() {}
   void ApplyRenderScalingAwareStyleConfig() {}
   void ApplySizeTypeStyleConfig() {}
   void HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e) {}
}
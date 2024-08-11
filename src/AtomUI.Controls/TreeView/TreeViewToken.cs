using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class TreeViewToken : AbstractControlDesignToken
{
   public const string ID = "TreeView";
   
   public TreeViewToken()
      : base(ID)
   {
   }
   
   /// <summary>
   /// 节点标题高度
   /// </summary>
   public double TitleHeight { get; set; }
   
   /// <summary>
   /// 节点悬浮态背景色
   /// </summary>
   public Color NodeHoverBg { get; set; }
   
   /// <summary>
   /// 节点选中态背景色
   /// </summary>
   public Color NodeSelectedBg { get; set; }

   #region 内部 Token 定义

   /// <summary>
   /// 目录树节点选中文字颜色
   /// </summary>
   public Color DirectoryNodeSelectedColor { get; set; }
   
   /// <summary>
   /// 目录树节点选中背景色
   /// </summary>
   public Color DirectoryNodeSelectedBg { get; set; }
   
   #endregion

   internal override void CalculateFromAlias()
   {
      base.CalculateFromAlias();
      TitleHeight = _globalToken.HeightToken.ControlHeightSM;
      NodeHoverBg = _globalToken.ControlItemBgHover;
      NodeSelectedBg = _globalToken.ControlItemBgActive;

      DirectoryNodeSelectedColor = _globalToken.ColorTextLightSolid;
      DirectoryNodeSelectedBg = _globalToken.ColorToken.ColorPrimaryToken.ColorPrimary;
   }
}
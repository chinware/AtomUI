namespace AtomUI.TokenSystem;

[GlobalDesignToken]
public class SizeMapDesignToken : AbstractDesignToken
{
   /// <summary>
   /// XXL
   /// </summary>
   public int SizeXXL { get; set; } = 48;

   /// <summary>
   /// XL
   /// </summary>
   public int SizeXL { get; set; } = 32;

   /// <summary>
   /// LG
   /// </summary>
   public int SizeLG { get; set; } = 24;

   /// <summary>
   /// MD
   /// </summary>
   public int SizeMD { get; set; } = 20;

   /// <summary>
   ///  Same as size by default, but could be larger in compact mode
   /// </summary>
   public int SizeMS { get; set; }

   /// <summary>
   /// 默认
   /// 默认尺寸
   /// </summary>
   public int Size { get; set; } = 16;

   /// <summary>
   /// SM
   /// </summary>
   public int SizeSM { get; set; } = 12;

   /// <summary>
   /// XS
   /// </summary>
   public int SizeXS { get; set; } = 8;

   /// <summary>
   /// XXS
   /// </summary>
   public int SizeXXS { get; set; } = 4;
}

[GlobalDesignToken]
public class HeightMapDesignToken : AbstractDesignToken
{
   /// <summary>
   /// 更小的组件高度
   /// </summary>
   public int ControlHeightXS { get; set; }
   
   /// <summary>
   /// 较小的组件高度
   /// </summary>
   public int ControlHeightSM { get; set; }
   
   /// <summary>
   /// 较高的组件高度
   /// </summary>
   public int ControlHeightLG { get; set; }
}
using System.Xml;

namespace AtomUI.Icon.Generators;

public record PathInfo
{
   public string Data { get; set; }
   public string? FillColor { get; set; }

   public PathInfo(string data, string? fillColor = null)
   {
      Data = data;
      FillColor = fillColor;
   }
}

public struct ViewBox
{
   public int X { get; set; }
   public int Y { get; set; }
   public int Width { get; set; }
   public int Height { get; set; }
}

public record SvgParsedInfo
{
   public List<PathInfo> PathInfos { get; set; } = new List<PathInfo>();
   public ViewBox ViewBox { get; set; }
}

public class SvgParser
{
   private const string SvgElementName = "svg";
   private const string PathElementName = "path";
   private const string FillAttrName = "fill";
   private const string DataAttrName = "d";
   private const string ViewBoxAttrName = "viewBox";

   private List<PathInfo>? _pathInfos;
   private ViewBox _viewBox;
   private bool _parseFinished = false;
   
   // 上下文信息
   private Stack<string>? _currentElementNames;
   
   public SvgParsedInfo Parse(string svg)
   {
      try {
         _pathInfos = new List<PathInfo>();
         _parseFinished = false;
         _currentElementNames = new Stack<string>();
         XmlReaderSettings settings = new XmlReaderSettings
         {
            DtdProcessing = DtdProcessing.Ignore,
         };
         using (XmlReader xmlReader = XmlReader.Create(new StringReader(svg), settings)) {
            while (xmlReader.Read() && !_parseFinished) {
               switch (xmlReader.NodeType) {
                  case XmlNodeType.Element:
                     HandleStartElement(xmlReader);
                     break;
                  case XmlNodeType.EndElement:
                     HandleEndElement(xmlReader.Name);
                     break;
               }
            }
         }

         return new SvgParsedInfo()
         {
            PathInfos = _pathInfos,
            ViewBox = _viewBox
         };
      } finally {
         _currentElementNames = null;
      }
   }
   
   private bool HandleStartElement(XmlReader reader)
   {
      var name = reader.Name;
      _currentElementNames!.Push(name);
      if (name == SvgElementName) {
         return HandleStartSvgElement(reader);
      } else if (name == PathElementName) {
         return HandleStartPathElement(reader);
      }
      return false;
   }
   
   private bool HandleEndElement(string name)
   {
      _currentElementNames!.Pop();
      return name == SvgElementName;
   }

   private bool HandleStartSvgElement(XmlReader reader)
   {
      var viewBox = reader.GetAttribute(ViewBoxAttrName);
      if (viewBox is not null) {
         var parts = viewBox.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
         // 暂时没有进行错误处理
         _viewBox.X = int.Parse(parts[0]);
         _viewBox.Y = int.Parse(parts[1]);
         _viewBox.Width = int.Parse(parts[2]);
         _viewBox.Height = int.Parse(parts[3]);
      }
      return false;
   }

   private bool HandleStartPathElement(XmlReader reader)
   {
      var data = reader.GetAttribute(DataAttrName);
      var fillColor = reader.GetAttribute(FillAttrName);
      var pathInfo = new PathInfo(data!, fillColor);
      _pathInfos!.Add(pathInfo);
      return false;
   }
}
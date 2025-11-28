using System.Xml;

namespace AtomUI.Icons;

public class SvgParser
{
    private const string SvgElementName = "svg";
    private const string PathElementName = "path";
    private const string RectElementName = "rect";
    private const string CircleElementName = "circle";
    private const string EllipseElementName = "ellipse";
    private const string LineElementName = "line";
    private const string PolygonElementName = "polygon";
    private const string PolylineElementName = "polyline";
    private const string GroupElementName = "g";
    private const string FillAttrName = "fill";
    private const string DataAttrName = "d";
    private const string TransformAttrName = "transform";
    private const string ViewBoxAttrName = "viewBox";
    
    // svg 图形元素的属性名称
    private const string XAttrName = "x";
    private const string YAttrName = "y";
    private const string WidthAttrName = "width";
    private const string HeightAttrName = "height";
    private const string RXAttrName = "rx";
    private const string RYAttrName = "ry";
    private const string CXAttrName = "cx";
    private const string CYAttrName = "cy";
    private const string PointsAttrName = "points";
    private const string X1AttrName = "x1";
    private const string Y1AttrName = "y1";
    private const string X2AttrName = "x2";
    private const string Y2AttrName = "y2";
    
    private List<SvgGraphicElement>? _graphicElements;
    private ViewBox _viewBox = new ();
    private bool _parseFinished;
    // 暂时我只支持解析一层 g 标签
    
    private Stack<GroupElement>? _groupInfoStack;

    // 上下文信息
    private Stack<string>? _currentElementNames;

    public SvgParsedInfo Parse(string svg)
    {
        try
        {
            _graphicElements     = new List<SvgGraphicElement>();
            _parseFinished       = false;
            _currentElementNames = new Stack<string>();
            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore
            };
            using (var xmlReader = XmlReader.Create(new StringReader(svg), settings))
            {
                while (xmlReader.Read() && !_parseFinished)
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            HandleStartElement(xmlReader);
                            break;
                        case XmlNodeType.EndElement:
                            HandleEndElement(xmlReader.Name);
                            break;
                    }
                }
            }

            return new SvgParsedInfo
            {
                GraphicElements = _graphicElements,
                ViewBox   = _viewBox
            };
        }
        finally
        {
            _currentElementNames = null;
        }
    }

    private bool HandleStartElement(XmlReader reader)
    {
        var name = reader.Name;
        _currentElementNames!.Push(name);
        if (name == SvgElementName)
        {
            return HandleStartSvgElement(reader);
        }

        if (name == PathElementName)
        {
            return HandleStartPathElement(reader);
        }

        if (name == GroupElementName)
        {
            return HandleStartGroupElement(reader);
        }

        if (name == RectElementName)
        {
            return HandleStartRectElement(reader);
        }

        return false;
    }

    private bool HandleEndElement(string name)
    {
        _currentElementNames!.Pop();
        if (name == GroupElementName)
        {
            HandleEndGroupElement();
        }
        return name == SvgElementName;
    }

    private bool HandleStartSvgElement(XmlReader reader)
    {
        var viewBox = reader.GetAttribute(ViewBoxAttrName);
        if (viewBox is not null)
        {
            var parts = viewBox.Split([' '], StringSplitOptions.RemoveEmptyEntries);
            // 暂时没有进行错误处理
            _viewBox.X      = int.Parse(parts[0]);
            _viewBox.Y      = int.Parse(parts[1]);
            _viewBox.Width  = int.Parse(parts[2]);
            _viewBox.Height = int.Parse(parts[3]);
        }

        return false;
    }

    private bool HandleStartPathElement(XmlReader reader)
    {
        var data      = reader.GetAttribute(DataAttrName);
        var fillColor = reader.GetAttribute(FillAttrName);
        var pathElement  = new PathElement(data!, fillColor);
        if (_groupInfoStack?.Count > 0)
        {
            var groupInfo = _groupInfoStack.Peek();
            pathElement.Transform = groupInfo.Transform;
        }
        // 暂时自己的优先级大
        var transform = reader.GetAttribute(TransformAttrName);
        if (!string.IsNullOrEmpty(transform))
        {
            pathElement.Transform = transform;
        }
        _graphicElements!.Add(pathElement);
        return false;
    }
    
    private bool HandleStartGroupElement(XmlReader reader)
    {
        _groupInfoStack ??= new Stack<GroupElement>();
        var groupInfo = new GroupElement();
        var transform = reader.GetAttribute(TransformAttrName);
        groupInfo.Transform = transform;
        _groupInfoStack.Push(groupInfo);
        return false;
    }

    private bool HandleStartRectElement(XmlReader reader)
    {
        var rectElement = new RectElement();
        if (_groupInfoStack?.Count > 0)
        {
            var groupInfo = _groupInfoStack.Peek();
            rectElement.Transform = groupInfo.Transform;
        }
        // 暂时自己的优先级大
        var transform = reader.GetAttribute(TransformAttrName);
        if (!string.IsNullOrEmpty(transform))
        {
            rectElement.Transform = transform;
        }
        _graphicElements!.Add(rectElement);
        return false;
    }

    private void HandleEndGroupElement()
    {
        _groupInfoStack?.Pop();
    }
}
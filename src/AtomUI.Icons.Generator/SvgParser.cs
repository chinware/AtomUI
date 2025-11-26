using System.Xml;

namespace AtomUI.Icons.Generator;

internal record PathInfo
{
    public string Data { get; set; }
    public string? Transform { get; set; }
    public string? FillColor { get; set; }

    public PathInfo(string data, string? fillColor = null)
    {
        Data      = data;
        FillColor = fillColor;
    }
}

internal struct ViewBox
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

internal record GroupInfo
{
    public string? Transform { get; set; }
}

internal record SvgParsedInfo
{
    public List<PathInfo> PathInfos { get; set; } = new();
    public ViewBox ViewBox { get; set; }
}

internal class SvgParser
{
    private const string SvgElementName = "svg";
    private const string PathElementName = "path";
    private const string GroupElementName = "g";
    private const string FillAttrName = "fill";
    private const string DataAttrName = "d";
    private const string TransformAttrName = "transform";
    private const string ViewBoxAttrName = "viewBox";

    private List<PathInfo>? _pathInfos;
    private ViewBox _viewBox;
    private bool _parseFinished;
    // 暂时我只支持解析一层 g 标签
    
    private Stack<GroupInfo>? _groupInfoStack;

    // 上下文信息
    private Stack<string>? _currentElementNames;

    public SvgParsedInfo Parse(string svg)
    {
        try
        {
            _pathInfos           = new List<PathInfo>();
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
                PathInfos = _pathInfos,
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
        var pathInfo  = new PathInfo(data!, fillColor);
        if (_groupInfoStack?.Count > 0)
        {
            var groupInfo = _groupInfoStack.Peek();
            pathInfo.Transform = groupInfo.Transform;
        }
        // 暂时自己的优先级大
        var transform = reader.GetAttribute(TransformAttrName);
        if (!string.IsNullOrEmpty(transform))
        {
            pathInfo.Transform = transform;
        }
        _pathInfos!.Add(pathInfo);
        return false;
    }
    
    private bool HandleStartGroupElement(XmlReader reader)
    {
        _groupInfoStack ??= new Stack<GroupInfo>();
        var groupInfo = new GroupInfo();
        var transform      = reader.GetAttribute(TransformAttrName);
        groupInfo.Transform = transform;
        _groupInfoStack.Push(groupInfo);
        return false;
    }

    private void HandleEndGroupElement()
    {
        _groupInfoStack?.Pop();
    }
}
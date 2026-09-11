using System.Globalization;
using System.Text;
using System.Xml;

namespace AtomUI.Controls;

internal sealed class SvgContentValidator
{
    private const string SvgNamespace = "http://www.w3.org/2000/svg";
    private const string XmlNamespace = "http://www.w3.org/XML/1998/namespace";
    private static readonly HashSet<string> s_unsafeElements = new(StringComparer.OrdinalIgnoreCase)
    {
        "script",
        "foreignObject"
    };

    private readonly ImageLoadingOptions _options;
    private readonly SvgCssReferenceValidator _cssValidator = new();
    private readonly SvgDataImageValidator _dataImageValidator;

    internal SvgContentValidator(
        ImageLoadingOptions options,
        Func<byte[], string, ImageSource, ImageProbeResult> rasterValidator)
    {
        _options = options;
        _dataImageValidator = new SvgDataImageValidator(rasterValidator);
    }

    internal SvgContentMetadata Validate(
        ReadOnlyMemory<byte> bytes,
        ImageSource source,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (bytes.Length > _options.Svg.MaxDocumentBytes)
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.ResponseTooLarge,
                "SVG document exceeds the configured byte limit.",
                source.DisplayName);
        }

        var state = new ValidationState(_options, _cssValidator, _dataImageValidator, source);
        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            MaxCharactersInDocument = _options.Svg.MaxXmlCharacters,
            MaxCharactersFromEntities = 0,
            IgnoreComments = true,
            IgnoreProcessingInstructions = false,
            CloseInput = false
        };

        try
        {
            using var stream = new MemoryStream(bytes.ToArray(), writable: false);
            using var reader = XmlReader.Create(stream, settings);
            while (reader.Read())
            {
                cancellationToken.ThrowIfCancellationRequested();
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        state.ReadElement(reader);
                        break;
                    case XmlNodeType.EndElement:
                        state.EndElement(reader);
                        break;
                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                    case XmlNodeType.SignificantWhitespace:
                    case XmlNodeType.Whitespace:
                        state.ReadText(reader.Value);
                        break;
                    case XmlNodeType.ProcessingInstruction:
                    case XmlNodeType.DocumentType:
                    case XmlNodeType.EntityReference:
                        throw Unsafe("SVG processing instructions, DTDs, and entities are not allowed.", source);
                }
            }
        }
        catch (ImageLoadFailureException)
        {
            throw;
        }
        catch (XmlException exception) when (IsDtdFailure(exception))
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.UnsafeVectorContent,
                "SVG DTDs and entities are not allowed.",
                source.DisplayName,
                exception);
        }
        catch (XmlException exception) when (IsCharacterLimitFailure(exception))
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.VectorComplexityLimitExceeded,
                "SVG XML characters exceed the configured limit.",
                source.DisplayName,
                exception);
        }
        catch (XmlException exception)
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.InvalidImageData,
                "SVG XML is invalid.",
                source.DisplayName,
                exception);
        }
        return state.Complete(bytes.Length);
    }

    private static bool IsDtdFailure(XmlException exception) =>
        exception.Message.Contains("DTD", StringComparison.OrdinalIgnoreCase) ||
        exception.Message.Contains("entity", StringComparison.OrdinalIgnoreCase);

    private static bool IsCharacterLimitFailure(XmlException exception) =>
        exception.Message.Contains("MaxCharactersInDocument", StringComparison.OrdinalIgnoreCase) ||
        exception.Message.Contains("maximum number of characters", StringComparison.OrdinalIgnoreCase);

    private static ImageLoadFailureException Unsafe(string message, ImageSource source) =>
        ImageSourceReadHelpers.Failure(ImageLoadErrorCode.UnsafeVectorContent, message, source.DisplayName);

    private sealed class ValidationState
    {
        private readonly ImageLoadingOptions _options;
        private readonly SvgCssReferenceValidator _cssValidator;
        private readonly SvgDataImageValidator _dataImageValidator;
        private readonly ImageSource _source;
        private readonly Stack<int> _elementNodes = new();
        private readonly Dictionary<int, List<int>> _children = new();
        private readonly Dictionary<int, HashSet<string>> _references = new();
        private readonly Dictionary<string, int> _firstIdNodes = new(StringComparer.Ordinal);
        private StringBuilder? _styleText;
        private int _styleDepth = -1;
        private bool _rootSeen;
        private bool _hasDuplicateId;
        private int _elementCount;
        private int _attributeCount;
        private int _maxElementDepth;
        private long _pathDataCharacters;
        private int _embeddedImageCount;
        private long _embeddedImageBytes;
        private long _embeddedImageDecodedBytes;
        private double? _intrinsicWidth;
        private double? _intrinsicHeight;
        private double? _viewBoxWidth;
        private double? _viewBoxHeight;

        internal ValidationState(
            ImageLoadingOptions options,
            SvgCssReferenceValidator cssValidator,
            SvgDataImageValidator dataImageValidator,
            ImageSource source)
        {
            _options = options;
            _cssValidator = cssValidator;
            _dataImageValidator = dataImageValidator;
            _source = source;
        }

        internal void ReadElement(XmlReader reader)
        {
            var depth = reader.Depth + 1;
            _elementCount++;
            _maxElementDepth = Math.Max(_maxElementDepth, depth);
            if (_elementCount > _options.Svg.MaxElementCount || depth > _options.Svg.MaxElementDepth)
            {
                throw Complexity("SVG element count or depth exceeds the configured limit.");
            }
            if (!_rootSeen)
            {
                _rootSeen = true;
                if (reader.Depth != 0 || reader.LocalName != "svg" || reader.NamespaceURI != SvgNamespace)
                {
                    throw Invalid("SVG root element or namespace is invalid.");
                }
            }
            if (s_unsafeElements.Contains(reader.LocalName))
            {
                throw Unsafe("SVG contains an unsafe element.", _source);
            }

            var attributes = ReadAttributes(reader);
            var elementId = attributes.FirstOrDefault(attribute =>
                attribute.LocalName == "id" && attribute.NamespaceUri.Length == 0).Value;
            var nodeId = _elementCount - 1;
            if (_elementNodes.Count > 0)
            {
                AddChild(_elementNodes.Peek(), nodeId);
            }
            if (!string.IsNullOrEmpty(elementId))
            {
                if (!_firstIdNodes.TryAdd(elementId, nodeId))
                {
                    _hasDuplicateId = true;
                }
            }
            if (reader.Depth == 0)
            {
                ReadRootDimensions(attributes);
            }
            foreach (var attribute in attributes)
            {
                ValidateAttribute(reader.LocalName, nodeId, attribute);
            }

            if (reader.LocalName.Equals("style", StringComparison.OrdinalIgnoreCase))
            {
                _styleDepth = reader.Depth;
                _styleText = new StringBuilder();
            }
            if (!reader.IsEmptyElement)
            {
                _elementNodes.Push(nodeId);
            }
            else if (_styleDepth == reader.Depth)
            {
                ValidateStyle(nodeId);
            }
        }

        internal void EndElement(XmlReader reader)
        {
            if (_styleDepth == reader.Depth)
            {
                ValidateStyle(_elementNodes.Peek());
            }
            if (_elementNodes.Count > 0)
            {
                _elementNodes.Pop();
            }
        }

        internal void ReadText(string value)
        {
            _styleText?.Append(value);
        }

        internal SvgContentMetadata Complete(int documentBytes)
        {
            if (!_rootSeen)
            {
                throw Invalid("SVG document has no root element.");
            }
            var maxReferenceDepth = ValidateReferenceGraph();
            if (_hasDuplicateId && _options.Svg.ConformanceMode == SvgConformanceMode.Strict)
            {
                throw Invalid("SVG contains a duplicate id.");
            }
            var referenceCount = _references.Sum(pair => pair.Value.Count);
            long estimatedCost;
            try
            {
                estimatedCost = checked(
                    documentBytes +
                    _embeddedImageDecodedBytes +
                    (long)_elementCount * 128 +
                    (long)_attributeCount * 64 +
                    _pathDataCharacters * 2 +
                    (long)referenceCount * 32);
            }
            catch (OverflowException)
            {
                throw Complexity("SVG model complexity exceeds the supported range.");
            }
            return new SvgContentMetadata(
                _elementCount,
                _attributeCount,
                _maxElementDepth,
                _pathDataCharacters,
                _embeddedImageCount,
                _embeddedImageBytes,
                _embeddedImageDecodedBytes,
                maxReferenceDepth,
                _intrinsicWidth,
                _intrinsicHeight,
                _viewBoxWidth,
                _viewBoxHeight,
                Math.Max(1, estimatedCost));
        }

        private List<SvgAttribute> ReadAttributes(XmlReader reader)
        {
            var attributes = new List<SvgAttribute>(reader.AttributeCount);
            if (!reader.HasAttributes)
            {
                return attributes;
            }
            while (reader.MoveToNextAttribute())
            {
                _attributeCount++;
                if (_attributeCount > _options.Svg.MaxAttributeCount)
                {
                    throw Complexity("SVG attribute count exceeds the configured limit.");
                }
                attributes.Add(new SvgAttribute(reader.LocalName, reader.NamespaceURI, reader.Value));
            }
            reader.MoveToElement();
            return attributes;
        }

        private void ValidateAttribute(string elementName, int sourceNodeId, SvgAttribute attribute)
        {
            if (attribute.LocalName.StartsWith("on", StringComparison.OrdinalIgnoreCase))
            {
                throw Unsafe("SVG event attributes are not allowed.", _source);
            }
            if (attribute.LocalName == "base" && attribute.NamespaceUri == XmlNamespace)
            {
                throw Unsafe("SVG xml:base is not allowed.", _source);
            }
            if (attribute.LocalName == "d")
            {
                try
                {
                    _pathDataCharacters = checked(_pathDataCharacters + attribute.Value.Length);
                }
                catch (OverflowException)
                {
                    throw Complexity("SVG path data exceeds the configured limit.");
                }
                if (_pathDataCharacters > _options.Svg.MaxPathDataCharacters)
                {
                    throw Complexity("SVG path data exceeds the configured limit.");
                }
            }
            if (attribute.LocalName == "href")
            {
                ProcessReference(attribute.Value, elementName, sourceNodeId);
                return;
            }
            _cssValidator.Validate(
                attribute.Value,
                value => ProcessReference(value, elementName, sourceNodeId),
                _source);
        }

        private void ValidateStyle(int sourceNodeId)
        {
            var css = _styleText?.ToString() ?? string.Empty;
            _styleText = null;
            _styleDepth = -1;
            _cssValidator.Validate(
                css,
                value => ProcessReference(value, "style", sourceNodeId),
                _source);
        }

        private void ProcessReference(string value, string elementName, int sourceNodeId)
        {
            var reference = value.Trim();
            if (reference.StartsWith('#') && reference.Length > 1)
            {
                AddReference(sourceNodeId, reference[1..]);
                return;
            }
            if (reference.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
            {
                if (!elementName.Equals("image", StringComparison.OrdinalIgnoreCase) &&
                    !elementName.Equals("feImage", StringComparison.OrdinalIgnoreCase) &&
                    !elementName.Equals("style", StringComparison.OrdinalIgnoreCase))
                {
                    throw Unsafe("SVG data images are not valid for this reference.", _source);
                }
                ValidateDataImage(reference);
                return;
            }
            throw Unsafe("SVG external resources are not allowed.", _source);
        }

        private void ValidateDataImage(string value)
        {
            if (_embeddedImageCount >= _options.Svg.MaxEmbeddedImageCount)
            {
                throw EmbeddedLimit();
            }
            var remaining = _options.Svg.MaxEmbeddedImageBytes - _embeddedImageBytes;
            var result = _dataImageValidator.Validate(value, remaining, _source);
            _embeddedImageCount++;
            try
            {
                _embeddedImageBytes = checked(_embeddedImageBytes + result.EncodedBytes);
                _embeddedImageDecodedBytes = checked(_embeddedImageDecodedBytes + result.DecodedBytes);
            }
            catch (OverflowException)
            {
                throw EmbeddedLimit();
            }
            if (_embeddedImageBytes > _options.Svg.MaxEmbeddedImageBytes)
            {
                throw EmbeddedLimit();
            }
            if (_embeddedImageDecodedBytes > _options.MaxDecodedImageBytes)
            {
                throw ImageSourceReadHelpers.Failure(
                    ImageLoadErrorCode.DecodedByteLimitExceeded,
                    "SVG embedded image decoded bytes exceed the configured limit.",
                    _source.DisplayName);
            }
        }

        private void AddReference(int sourceNodeId, string targetId)
        {
            if (!_references.TryGetValue(sourceNodeId, out var targets))
            {
                targets = new HashSet<string>(StringComparer.Ordinal);
                _references.Add(sourceNodeId, targets);
            }
            targets.Add(targetId);
        }

        private void AddChild(int parentNodeId, int childNodeId)
        {
            if (!_children.TryGetValue(parentNodeId, out var children))
            {
                children = new List<int>();
                _children.Add(parentNodeId, children);
            }
            children.Add(childNodeId);
        }

        private int ValidateReferenceGraph()
        {
            var states = new Dictionary<int, byte>();
            var depths = new Dictionary<int, int>();
            var maxDepth = 0;
            for (var nodeId = 0; nodeId < _elementCount; nodeId++)
            {
                maxDepth = Math.Max(maxDepth, Visit(nodeId, states, depths));
            }
            return maxDepth;
        }

        private int Visit(
            int nodeId,
            Dictionary<int, byte> states,
            Dictionary<int, int> depths)
        {
            if (depths.TryGetValue(nodeId, out var knownDepth))
            {
                return knownDepth;
            }
            if (states.TryGetValue(nodeId, out var state) && state == 1)
            {
                throw Complexity("SVG local reference graph contains a cycle.");
            }
            states[nodeId] = 1;
            var depth = 0;
            if (_children.TryGetValue(nodeId, out var children))
            {
                foreach (var childNodeId in children)
                {
                    depth = Math.Max(depth, Visit(childNodeId, states, depths));
                }
            }
            if (_references.TryGetValue(nodeId, out var targets))
            {
                foreach (var target in targets)
                {
                    if (!_firstIdNodes.TryGetValue(target, out var targetNodeId))
                    {
                        continue;
                    }
                    depth = Math.Max(depth, checked(Visit(targetNodeId, states, depths) + 1));
                    if (depth > _options.Svg.MaxReferenceDepth)
                    {
                        throw Complexity("SVG local reference depth exceeds the configured limit.");
                    }
                }
            }
            states[nodeId] = 2;
            depths[nodeId] = depth;
            return depth;
        }

        private void ReadRootDimensions(IReadOnlyList<SvgAttribute> attributes)
        {
            foreach (var attribute in attributes)
            {
                switch (attribute.LocalName)
                {
                    case "width":
                        _intrinsicWidth = ParseLength(attribute.Value);
                        break;
                    case "height":
                        _intrinsicHeight = ParseLength(attribute.Value);
                        break;
                    case "viewBox":
                        ParseViewBox(attribute.Value, out _viewBoxWidth, out _viewBoxHeight);
                        break;
                }
            }
        }

        private static double? ParseLength(string value)
        {
            var span = value.AsSpan().Trim();
            if (span.EndsWith("px", StringComparison.OrdinalIgnoreCase))
            {
                span = span[..^2].TrimEnd();
            }
            else if (span.Length > 0 && (char.IsLetter(span[^1]) || span[^1] == '%'))
            {
                return null;
            }
            return double.TryParse(span, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
                ? result
                : null;
        }

        private void ParseViewBox(string value, out double? width, out double? height)
        {
            width = null;
            height = null;
            var parts = value.Split([' ', '\t', '\r', '\n', ','], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 4 ||
                !double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedWidth) ||
                !double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedHeight))
            {
                throw Invalid("SVG viewBox is invalid.");
            }
            width = parsedWidth;
            height = parsedHeight;
        }

        private ImageLoadFailureException Invalid(string message) =>
            ImageSourceReadHelpers.Failure(ImageLoadErrorCode.InvalidImageData, message, _source.DisplayName);

        private ImageLoadFailureException Complexity(string message) =>
            ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.VectorComplexityLimitExceeded,
                message,
                _source.DisplayName);

        private ImageLoadFailureException EmbeddedLimit() =>
            ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.EmbeddedResourceLimitExceeded,
                "SVG embedded resources exceed the configured limit.",
                _source.DisplayName);

        private readonly record struct SvgAttribute(string LocalName, string NamespaceUri, string Value);
    }
}

using Avalonia.Media;

namespace AtomUI.Theme.TokenSystem;

[TokenValueConverter]
internal class StringTokenValueConverter : ITokenValueConverter
{
    public Type TargetType()
    {
        return typeof(string);
    }

    public object Convert(string value)
    {
        return value;
    }
}

[TokenValueConverter]
internal class IntegerTokenValueConverter : ITokenValueConverter
{
    public Type TargetType()
    {
        return typeof(int);
    }

    public object Convert(string value)
    {
        try
        {
            return int.Parse(value);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Convert {value} to int failed.", exception);
        }
    }
}

[TokenValueConverter]
internal class BoolTokenValueConverter : ITokenValueConverter
{
    public Type TargetType()
    {
        return typeof(bool);
    }

    public object Convert(string value)
    {
        var isTrue  = string.Compare(value, "true", StringComparison.InvariantCultureIgnoreCase) == 0;
        var isFalse = string.Compare(value, "false", StringComparison.InvariantCultureIgnoreCase) == 0;
        if (!isTrue && !isFalse)
        {
            throw new InvalidOperationException($"Convert {value} to bool failed.");
        }

        if (isTrue)
        {
            return true;
        }

        return false;
    }
}

[TokenValueConverter]
internal class ColorTokenValueConverter : ITokenValueConverter
{
    public Type TargetType()
    {
        return typeof(Color);
    }

    public object Convert(string value)
    {
        try
        {
            return Color.Parse(value);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Convert {value} to Color failed.", exception);
        }
    }
}

[TokenValueConverter]
internal class BoxShadowTokenValueConverter : ITokenValueConverter
{
    public Type TargetType()
    {
        return typeof(BoxShadows);
    }

    public object Convert(string value)
    {
        try
        {
            return BoxShadows.Parse(value);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Convert {value} to BoxShadows failed.", exception);
        }
    }
}

[TokenValueConverter]
internal class TextDecorationTokenValueConverter : ITokenValueConverter
{
    public Type TargetType()
    {
        return typeof(TextDecorationInfo);
    }

    public object Convert(string value)
    {
        try
        {
            if (value.IndexOf("none", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                return new TextDecorationInfo
                {
                    LineType  = TextDecorationLine.None,
                    Thickness = 0
                };
            }

            var textDecoration = new TextDecorationInfo();
            if (ContainStr(value, "underline"))
            {
                textDecoration.LineType = TextDecorationLine.Underline;
            }
            else if (ContainStr(value, "overline"))
            {
                textDecoration.LineType = TextDecorationLine.Overline;
            }
            else if (ContainStr(value, "line-through"))
            {
                textDecoration.LineType = TextDecorationLine.LineThrough;
            }
            else
            {
                throw new InvalidOperationException($"Unsupported line type in value expression {value}.");
            }

            if (ContainStr(value, "solid"))
            {
                textDecoration.LineStyle = LineStyle.Solid;
            }
            else if (ContainStr(value, "double"))
            {
                textDecoration.LineStyle = LineStyle.Double;
            }
            else if (ContainStr(value, "dotted"))
            {
                textDecoration.LineStyle = LineStyle.Dotted;
            }
            else if (ContainStr(value, "dashed"))
            {
                textDecoration.LineStyle = LineStyle.Dashed;
            }
            else if (ContainStr(value, "Wavy"))
            {
                textDecoration.LineStyle = LineStyle.Wavy;
            }
            else
            {
                throw new InvalidOperationException($"Unsupported line style in value expression {value}.");
            }

            var colorExpr = ExtraColorExpr(value);

            if (!string.IsNullOrWhiteSpace(colorExpr))
            {
                textDecoration.Color = Color.Parse(colorExpr);
            }

            value = value.Replace(colorExpr, "");
            var alreadySeeNum = false;
            var numExpr       = string.Empty;
            for (var i = 0; i < value.Length; ++i)
            {
                var cur = value[i];
                if (alreadySeeNum && !char.IsDigit(cur))
                {
                    break;
                }

                if (char.IsDigit(cur))
                {
                    alreadySeeNum =  true;
                    numExpr       += cur;
                }
            }

            if (numExpr.Length > 0)
            {
                // 肯定合法
                textDecoration.Thickness = int.Parse(numExpr);
            }

            return textDecoration;
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Convert {value} to TextDecorationInfo failed.", exception);
        }
    }

    protected string ExtraColorExpr(string valueExpr)
    {
        var colorExpr = string.Empty;
        var count     = valueExpr.Length;
        if (ContainStr(valueExpr, "#"))
        {
            var pos    = valueExpr.IndexOf('#');
            var endPos = pos;
            while (endPos < count && !char.IsWhiteSpace(valueExpr[endPos]))
            {
                endPos++;
            }

            return valueExpr.Substring(pos, endPos - pos);
        }

        if (ContainStr(valueExpr, "rgb"))
        {
            var pos    = valueExpr.IndexOf("rgb", StringComparison.CurrentCultureIgnoreCase);
            var endPos = pos;
            while (endPos < count && valueExpr[endPos] != ')')
            {
                endPos++;
            }

            return valueExpr.Substring(pos, endPos - pos + 1);
        }

        return colorExpr;
    }

    protected bool ContainStr(string expr, string searched)
    {
        return expr.IndexOf(searched, StringComparison.InvariantCultureIgnoreCase) != -1;
    }
}

[TokenValueConverter]
internal class LineStyleTokenValueConverter : ITokenValueConverter
{
    public Type TargetType()
    {
        return typeof(LineStyle);
    }

    public object Convert(string value)
    {
        var lineStyle = LineStyle.Solid;
        if (ContainStr(value, "solid"))
        {
            lineStyle = LineStyle.Solid;
        }
        else if (ContainStr(value, "double"))
        {
            lineStyle = LineStyle.Double;
        }
        else if (ContainStr(value, "dotted"))
        {
            lineStyle = LineStyle.Dotted;
        }
        else if (ContainStr(value, "dashed"))
        {
            lineStyle = LineStyle.Dashed;
        }
        else if (ContainStr(value, "wavy"))
        {
            lineStyle = LineStyle.Wavy;
        }
        else
        {
            throw new InvalidOperationException($"Unsupported line style in value expression {value}.");
        }

        return lineStyle;
    }

    protected bool ContainStr(string expr, string searched)
    {
        return expr.IndexOf(searched, StringComparison.InvariantCultureIgnoreCase) != -1;
    }
}
# Icon 包制作指导手册

本文档指导如何基于 AtomUI Icon 基础设施创建自定义 Icon 包。

---

## 1. 前提条件

- 准备好 SVG 图标源文件，按主题分目录存放（如 `Filled/`、`Outlined/`、`TwoTone/`）
- SVG 文件命名使用 **kebab-case**（如 `check-circle.svg`），生成器会自动转为 PascalCase 类名
- SVG 中应使用标准图形元素：`<path>`、`<rect>`、`<circle>`、`<ellipse>`、`<line>`、`<polyline>`、`<polygon>`，避免使用 `<use>`、`<symbol>`、`<defs>` 等引用元素

---

## 2. 创建步骤

### 步骤 1: 创建 Icon 包项目（运行时类库）

创建一个新的类库项目，如 `MyCompany.Icons.MyIconSet`：

```xml
<!-- MyCompany.Icons.MyIconSet.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFrameworks>$(AtomUITargetFrameworks)</TargetFrameworks>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
        <LangVersion>latest</LangVersion>
        <RootNamespace>MyCompany.Icons.MyIconSet</RootNamespace>
        <OutputType>library</OutputType>
    </PropertyGroup>
    <ItemGroup>
        <ProjectReference Include="../AtomUI.Core/AtomUI.Core.csproj" />
        <!-- 或者使用 NuGet 包引用 -->
        <!-- <PackageReference Include="AtomUI.Core" /> -->
    </ItemGroup>
</Project>
```

### 步骤 2: 创建包级 Icon 基类

如果你的图标包需要特殊的渲染逻辑（如 Ant Design 的 ZoomToFit），创建一个包级基类；否则可以直接继承 `Icon`。

```csharp
// MyIconSetIcon.cs
using AtomUI.Controls;
using Avalonia;

namespace MyCompany.Icons.MyIconSet;

public class MyIconSetIcon : Icon
{
    // 如果需要自定义全局几何变换，覆写此方法
    // 否则使用默认的 Matrix.Identity
    protected override Matrix CalculateGlobalGeometryMatrix()
    {
        // 可参考 AntDesignIcon 的 ZoomToFit 实现
        return base.CalculateGlobalGeometryMatrix();
    }
}
```

### 步骤 3: 创建 IconProvider

```csharp
// MyIconSetIconProvider.cs
using System.Reflection;
using AtomUI.Controls;

namespace MyCompany.Icons.MyIconSet;

public class MyIconSetIconProvider : IconProvider<MyIconSetIconKind>
{
    public MyIconSetIconProvider()
    {
    }

    public MyIconSetIconProvider(MyIconSetIconKind kind)
        : base(kind)
    {
    }

    protected override Type GetTypeForKind(MyIconSetIconKind kind)
    {
        var typeName = $"MyCompany.Icons.MyIconSet.{kind.ToString()}";
        var type = Type.GetType(typeName)
                   ?? Assembly.GetExecutingAssembly().GetType(typeName);
        if (type == null)
        {
            throw new InvalidOperationException($"Type {typeName} does not exist");
        }
        return type;
    }
}
```

### 步骤 4: 注册 AXAML 命名空间

创建 `Properties/AssemblyInfo.cs`：

```csharp
using Avalonia.Metadata;

[assembly: XmlnsPrefix("https://mycompany.com/icons/myiconset", "myicons")]
[assembly: XmlnsDefinition("https://mycompany.com/icons/myiconset", "MyCompany.Icons.MyIconSet")]
```

### 步骤 5: 创建生成器项目

创建一个控制台项目，如 `MyCompany.Icons.MyIconSet.Generator`：

```xml
<!-- MyCompany.Icons.MyIconSet.Generator.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <TargetFrameworks>$(AtomUITargetFrameworks)</TargetFrameworks>
        <OutputType>Exe</OutputType>
    </PropertyGroup>
    <ItemGroup>
        <ProjectReference Include="../AtomUI.Icons.Shared/AtomUI.Icons.Shared.csproj" />
        <!-- 或者使用 NuGet 包引用 -->
        <!-- <PackageReference Include="AtomUI.Icons.Shared" /> -->
    </ItemGroup>
</Project>
```

### 步骤 6: 实现生成器

```csharp
// MyIconSetGenerator.cs
using System.Text;
using AtomUI.Controls;
using AtomUI.Icons;

namespace MyCompany.Icons.MyIconSet.Generator;

public class MyIconSetGenerator : DefaultIconPackageGenerator
{
    public MyIconSetGenerator(string sourcePath, string targetPath)
        : base(sourcePath, targetPath)
    {
        PackageName      = "MyIconSet";
        PackageNamespace = "MyCompany.Icons.MyIconSet";
    }

    public static async Task<int> Main(string[] args)
    {
        try
        {
            var targetProjectPath = Path.GetFullPath(
                Path.Combine(Directory.GetCurrentDirectory(),
                "../../../../src/MyCompany.Icons.MyIconSet"));
            var sourceProjectPath = Path.GetFullPath(
                Path.Combine(Directory.GetCurrentDirectory(),
                "../../../../src/MyCompany.Icons.MyIconSet.Generator"));
            var sourcePath = Path.Combine(sourceProjectPath, "Assets/Svg");
            var generator  = new MyIconSetGenerator(sourcePath, targetProjectPath);
            await generator.GenerateAsync();
            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Generate error: {e.Message}");
            return 1;
        }
    }

    protected override async Task GenerateIconPackageClass(
        IconFileInfo iconFileInfo, Stream output)
    {
        var sourceText = new StringBuilder();
        sourceText.AppendLine("// This code is auto generated. Do not modify.");
        sourceText.AppendLine("using Avalonia;");
        sourceText.AppendLine("using System;");
        sourceText.AppendLine("using Avalonia.Media;");
        sourceText.AppendLine("using AtomUI.Controls;");
        sourceText.AppendLine("using AtomUI.Media;");
        sourceText.AppendLine($"namespace {PackageNamespace};");
        sourceText.AppendLine();

        var svgSource     = await File.ReadAllTextAsync(iconFileInfo.FilePath);
        var svgParsedInfo = SvgParser.Parse(svgSource);
        var viewBox       = svgParsedInfo.ViewBox;
        var className     = $"{iconFileInfo.Name}{iconFileInfo.ThemeType}";

        // 类声明 - 继承你的包级基类
        sourceText.AppendLine($"public class {className} : MyIconSetIcon");
        sourceText.AppendLine("{");

        // 构造函数
        sourceText.AppendLine($"    public {className}()");
        sourceText.AppendLine("    {");
        sourceText.AppendLine($"        IconTheme = IconThemeType.{iconFileInfo.ThemeType};");
        sourceText.AppendLine($"        ViewBox = new Rect({viewBox.X}, {viewBox.Y}, {viewBox.Width}, {viewBox.Height});");
        sourceText.AppendLine("    }");
        sourceText.AppendLine();

        // 绘图指令
        sourceText.AppendLine("    private static readonly DrawingInstruction[] StaticInstructions = [");

        for (var i = 0; i < svgParsedInfo.GraphicElements.Count; i++)
        {
            var element = svgParsedInfo.GraphicElements[i];
            if (element is PathElement pathElement)
            {
                sourceText.AppendLine("        new PathDrawingInstruction()");
                sourceText.AppendLine("        {");
                sourceText.AppendLine($"            Data = StreamGeometry.Parse(\"{pathElement.Data}\"),");

                // 根据主题类型决定画刷
                switch (iconFileInfo.ThemeType)
                {
                    case IconThemeType.Filled:
                        sourceText.AppendLine("            FillBrush = IconBrushType.Fill,");
                        break;
                    case IconThemeType.Outlined:
                        sourceText.AppendLine("            FillBrush = IconBrushType.Stroke,");
                        break;
                    default:
                        sourceText.AppendLine("            FillBrush = IconBrushType.Fill,");
                        break;
                }

                if (!string.IsNullOrEmpty(pathElement.Transform))
                {
                    sourceText.AppendLine($"            Transform = TransformParser.Parse(\"{pathElement.Transform}\").Value");
                }

                sourceText.AppendLine("        }");
            }

            // 类似地处理 RectElement, CircleElement 等...

            if (i != svgParsedInfo.GraphicElements.Count - 1)
            {
                sourceText.Append(", ");
            }
        }

        sourceText.AppendLine("    ];");
        sourceText.AppendLine();
        sourceText.AppendLine("    protected override IList<DrawingInstruction> DrawingInstructions => StaticInstructions;");
        sourceText.AppendLine("}");

        await output.WriteAsync(Encoding.UTF8.GetBytes(sourceText.ToString()));
    }
}
```

### 步骤 7: 放置 SVG 资源

```
MyCompany.Icons.MyIconSet.Generator/
└── Assets/
    └── Svg/
        ├── Filled/
        │   ├── home.svg
        │   ├── user.svg
        │   └── ...
        ├── Outlined/
        │   ├── home.svg
        │   ├── user.svg
        │   └── ...
        └── TwoTone/    (可选)
            └── ...
```

### 步骤 8: 运行生成器

```bash
cd src/MyCompany.Icons.MyIconSet.Generator
dotnet run
```

生成器会：
1. 扫描 `Assets/Svg/` 下所有 `.svg` 文件
2. 在 `src/MyCompany.Icons.MyIconSet/GeneratedIcons/` 下生成：
   - `MyIconSetIconKind.g.cs` — 枚举
   - `HomeFilled.g.cs`、`HomeOutlined.g.cs`、`UserFilled.g.cs` ... — 图标类

---

## 3. 项目最终结构

```
MyCompany.Icons.MyIconSet/
├── MyIconSetIcon.cs                      ← 包级基类
├── MyIconSetIconProvider.cs              ← MarkupExtension
├── MyCompany.Icons.MyIconSet.csproj
├── Properties/
│   └── AssemblyInfo.cs                   ← AXAML 命名空间注册
└── GeneratedIcons/                       ← 自动生成（不手动编辑）
    ├── MyIconSetIconKind.g.cs
    ├── HomeFilled.g.cs
    ├── HomeOutlined.g.cs
    └── ...

MyCompany.Icons.MyIconSet.Generator/
├── MyIconSetGenerator.cs
├── MyCompany.Icons.MyIconSet.Generator.csproj
└── Assets/
    └── Svg/
        ├── Filled/
        ├── Outlined/
        └── TwoTone/
```

---

## 4. 注意事项与最佳实践

### 4.1 SVG 文件要求

- **ViewBox**：SVG 根元素必须包含 `viewBox` 属性（如 `viewBox="0 0 1024 1024"`）
- **图形元素**：只使用 `<path>`、`<rect>`、`<circle>`、`<ellipse>`、`<line>`、`<polyline>`、`<polygon>`
- **避免的元素**：`<use>`、`<symbol>`、`<defs>`、`<filter>`、`<mask>`、`<clipPath>`（clipPath 会被跳过）
- **`<g>` 分组**：支持一层 `<g>` 分组，子元素会继承 `<g>` 的 `fill`、`stroke`、`transform` 等属性
- **命名规范**：文件名使用 `kebab-case`（如 `check-circle.svg`），自动转为 `CheckCircle` 类名

### 4.2 主题目录命名

SVG 文件的父目录名必须与 `IconThemeType` 枚举值匹配（不区分大小写）：

| 目录名 | IconThemeType |
|---|---|
| `Filled/` | `Filled` |
| `Outlined/` | `Outlined` |
| `TwoTone/` | `TwoTone` |
| `Rounded/` | `Rounded` |
| `Sharp/` | `Sharp` |
| `MultiColor/` | `MultiColor` |

如果目录名不匹配任何枚举值，默认为 `Filled`。

### 4.3 生成代码管理

- `GeneratedIcons/` 目录中的文件全部为自动生成，**请勿手动编辑**
- 每次运行生成器会**完全清空**并重新生成该目录
- 建议将生成代码纳入版本管理（而非 `.gitignore`），以便 CI/CD 环境无需运行生成器

### 4.4 枚举值命名

生成的枚举值名称 = `{PascalCaseName}{ThemeType}`，必须与生成的类名一致，`IconProvider` 通过枚举名查找类型。

### 4.5 非 `<path>` 元素处理

`DefaultIconPackageGenerator` 默认只生成了枚举和扫描逻辑，具体图形元素的代码生成由 `GenerateIconPackageClass` 方法负责。如果你的图标包含非 `<path>` 元素（`<rect>`、`<circle>` 等），需要在 `GenerateIconPackageClass` 中添加对应的代码生成逻辑：

```csharp
// 处理 <rect> 元素示例
if (element is RectElement rectElement)
{
    sourceText.AppendLine("        new RectDrawingInstruction()");
    sourceText.AppendLine("        {");
    sourceText.AppendLine($"            Rect = new Rect({rectElement.X}, {rectElement.Y}, {rectElement.Width}, {rectElement.Height}),");
    sourceText.AppendLine($"            RadiusX = {rectElement.RadiusX},");
    sourceText.AppendLine($"            RadiusY = {rectElement.RadiusY},");
    sourceText.AppendLine("            FillBrush = IconBrushType.Fill,");
    sourceText.AppendLine("        }");
}

// 处理 <circle> 元素示例
if (element is CircleElement circleElement)
{
    sourceText.AppendLine("        new CircleDrawingInstruction()");
    sourceText.AppendLine("        {");
    sourceText.AppendLine($"            Radius = {circleElement.Radius},");
    sourceText.AppendLine($"            Center = new Point({circleElement.CenterX}, {circleElement.CenterY}),");
    sourceText.AppendLine("            FillBrush = IconBrushType.Fill,");
    sourceText.AppendLine("        }");
}
```

### 4.6 自定义 TwoTone 颜色判断

如果你的图标包有 TwoTone 图标，需要根据图标源的颜色约定实现主/次色判断逻辑。参考 `AntDesignGenerator` 中的实现：通过维护一个次色模板颜色列表来区分主色和次色区域。

### 4.7 使用已发布的 NuGet 包

如果是在 AtomUI 仓库外部创建 Icon 包，将项目引用替换为 NuGet 包引用：

```xml
<!-- 生成器项目 -->
<PackageReference Include="AtomUI.Icons.Shared" Version="x.x.x" />

<!-- Icon 包项目 -->
<PackageReference Include="AtomUI.Core" Version="x.x.x" />
```


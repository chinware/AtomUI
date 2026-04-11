# AntDesign Icon 包实例解析

本文档以 `AtomUI.Icons.AntDesign` 和 `AtomUI.Icons.AntDesign.Generator` 为例，详细阐述从基础模块到具体 Icon 包的完整生成流程。

---

## 1. 整体结构

```
src/AtomUI.Icons.AntDesign.Generator/     ← 构建时生成器（控制台程序）
├── AntDesignGenerator.cs                  ← 继承 DefaultIconPackageGenerator
├── Assets/Svg/                            ← 原始 SVG 资源
│   ├── Filled/                            ← Filled 主题图标
│   │   ├── account-book.svg
│   │   ├── alert.svg
│   │   └── ...
│   ├── Outlined/                          ← Outlined 主题图标
│   │   ├── aim.svg
│   │   ├── alert.svg
│   │   └── ...
│   └── TwoTone/                           ← TwoTone 主题图标
│       ├── account-book.svg
│       ├── alert.svg
│       └── ...
└── AtomUI.Icons.AntDesign.Generator.csproj  ← OutputType=Exe

src/AtomUI.Icons.AntDesign/                ← 运行时 Icon 包（类库）
├── AntDesignIcon.cs                       ← 包级基类
├── AntDesignIconProvider.cs               ← MarkupExtension
├── Properties/
│   └── AssemblyInfo.cs                    ← AXAML 命名空间注册
├── GeneratedIcons/                        ← 自动生成代码目录
│   ├── AntDesignIconKind.g.cs             ← 枚举
│   ├── LoadingOutlined.g.cs               ← 单个图标类
│   ├── CheckCircleFilled.g.cs
│   ├── SettingTwoTone.g.cs
│   └── ...（约 800+ 个文件）
└── AtomUI.Icons.AntDesign.csproj          ← 仅依赖 AtomUI.Core
```

---

## 2. 生成流程

### 2.1 启动生成器

`AntDesignGenerator` 是一个控制台程序，通过 `Main()` 入口启动：

```csharp
public static async Task<int> Main(string[] args)
{
    var targetProjectPath = Path.GetFullPath(
        Path.Combine(Directory.GetCurrentDirectory(),
        "../../../../src/AtomUI.Icons.AntDesign"));
    var sourceProjectPath = Path.GetFullPath(
        Path.Combine(Directory.GetCurrentDirectory(),
        "../../../../src/AtomUI.Icons.AntDesign.Generator"));
    var sourcePath = Path.Combine(sourceProjectPath, "Assets/Svg");
    var generator = new AntDesignGenerator(sourcePath, targetProjectPath);
    await generator.GenerateAsync();
}
```

运行方式：

```bash
cd src/AtomUI.Icons.AntDesign.Generator
dotnet run
```

### 2.2 生成器配置

```csharp
public AntDesignGenerator(string sourcePath, string targetPath)
    : base(sourcePath, targetPath)
{
    PackageName      = "AntDesign";           // 生成 AntDesignIconKind 枚举
    PackageNamespace = "AtomUI.Icons.AntDesign";  // 生成代码的命名空间

    // TwoTone 图标的主/次色判断依据
    _twoToneTplPrimaryColors   = ["#333"];
    _twoToneTplSecondaryColors = ["#E6E6E6", "#D9D9D9", "#D8D8D8"];
}
```

### 2.3 完整生成流程

```
GenerateAsync()
│
├─ 1. PrepareEnvironment()
│     验证源路径和目标路径存在
│     删除并重建 GeneratedIcons/ 目录
│
├─ 2. ScanIconFiles()  →  ScanIconFilesRecursively()
│     递归遍历 Assets/Svg/ 下所有 .svg 文件
│     从父目录名推断 IconThemeType（Filled/Outlined/TwoTone）
│     文件名 kebab-case → PascalCase（如 account-book → AccountBook）
│     收集为 List<IconFileInfo>
│
├─ 3. GenerateIconPackageKindAsync()
│     生成 AntDesignIconKind.g.cs
│     每个图标 → 一个枚举值（如 AccountBookFilled = 1）
│
└─ 4. GenerateIconPackageClassesAsync()
      对每个 IconFileInfo 调用 GenerateIconPackageClass()
      │
      ├─ 读取 SVG 文件内容
      ├─ SvgParser.Parse() → SvgParsedInfo
      │   ├─ 提取 ViewBox
      │   └─ 提取图形元素列表
      │
      └─ 生成 C# 类代码
          ├─ 类名 = {Name}{ThemeType}（如 AccountBookFilled）
          ├─ 继承 AntDesignIcon
          ├─ 构造函数设置 IconTheme 和 ViewBox
          ├─ 将图形元素转换为 DrawingInstruction
          │   └─ 根据 ThemeType 和颜色判断 FillBrush 类型：
          │       Filled   → IconBrushType.Fill
          │       Outlined → IconBrushType.Stroke
          │       TwoTone  → 主色 Stroke / 次色 Fill
          └─ 输出为 static readonly DrawingInstruction[] StaticInstructions
```

### 2.4 TwoTone 图标的颜色判断逻辑

TwoTone 图标包含主色和次色两种 path。生成器通过 SVG 中 `fill` 属性的颜色值判断：

```csharp
// 如果 path 的 fill 颜色在次色模板列表中 → 次色（FillBrush = IconBrushType.Fill）
// 否则 → 主色（FillBrush = IconBrushType.Stroke）
var isPrimary = !(pathElement.FillColor != null &&
                  _twoToneTplSecondaryColors.Contains(pathElement.FillColor));
```

这使得运行时可以通过设置 `StrokeBrush`（主色）和 `FillBrush`（次色）来自由控制 TwoTone 图标的双色搭配。

---

## 3. 生成代码示例

### 3.1 Outlined 图标（单路径描边型）

```csharp
// LoadingOutlined.g.cs
public class LoadingOutlined : AntDesignIcon
{
    public LoadingOutlined()
    {
        IconTheme = IconThemeType.Outlined;
        ViewBox = new Rect(0, 0, 1024, 1024);
    }

    private static readonly DrawingInstruction[] StaticInstructions = [
        new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M988 548c-19.9 0-36-16.1-36-36 ..."),
            FillBrush = IconBrushType.Stroke,  // Outlined 使用描边画刷
        }
    ];

    protected override IList<DrawingInstruction> DrawingInstructions => StaticInstructions;
}
```

### 3.2 Filled 图标（单路径填充型）

```csharp
// CheckCircleFilled.g.cs
public class CheckCircleFilled : AntDesignIcon
{
    public CheckCircleFilled()
    {
        IconTheme = IconThemeType.Filled;
        ViewBox = new Rect(0, 0, 1024, 1024);
    }

    private static readonly DrawingInstruction[] StaticInstructions = [
        new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M512 64C264.6 64 64 264.6 ..."),
            FillBrush = IconBrushType.Fill,  // Filled 使用填充画刷
        }
    ];

    protected override IList<DrawingInstruction> DrawingInstructions => StaticInstructions;
}
```

### 3.3 TwoTone 图标（多路径双色型）

```csharp
// SettingTwoTone.g.cs
public class SettingTwoTone : AntDesignIcon
{
    public SettingTwoTone()
    {
        IconTheme = IconThemeType.TwoTone;
        ViewBox = new Rect(0, 0, 1024, 1024);
    }

    private static readonly DrawingInstruction[] StaticInstructions = [
        new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M859.3 569.7l.2.1c3.1-18.9 ..."),
            FillBrush = IconBrushType.Fill,     // 次色区域
        },
        new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M935.8 646.6c.5 4.7 0 9.5 ..."),
            FillBrush = IconBrushType.Fill,     // 次色区域
        },
        new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M688 502c0-30.3-7.7-58.9 ..."),
            FillBrush = IconBrushType.Stroke,   // 主色区域
        },
        new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M594.1 952.2a32.05 32.05 ..."),
            FillBrush = IconBrushType.Stroke,   // 主色区域
        }
    ];

    protected override IList<DrawingInstruction> DrawingInstructions => StaticInstructions;
}
```

### 3.4 枚举文件

```csharp
// AntDesignIconKind.g.cs
namespace AtomUI.Icons.AntDesign;
public enum AntDesignIconKind
{
    LikeTwoTone = 1,
    PauseCircleTwoTone = 2,
    CheckSquareTwoTone = 3,
    // ... 约 800+ 个枚举值
    ZoomOutOutlined = 848,
}
```

---

## 4. AntDesignIcon 包级基类

`AntDesignIcon` 继承 `Icon`，提供 Ant Design 图标特有的 **ZoomToFit** 几何变换逻辑：

```csharp
public class AntDesignIcon : Icon
{
    private Rect? _geometryBounds;

    protected override Matrix CalculateGlobalGeometryMatrix()
    {
        _geometryBounds ??= CalculateGeometryBounds();
        return CalculateZoomToFit(ViewBox, _geometryBounds ?? default);
    }
}
```

**ZoomToFit 算法**：

1. 计算所有 `DrawingInstruction` 的联合几何边界
2. 计算图标内容到 ViewBox 边界的最小间距
3. 以 ViewBox 中心为基准，计算最大缩放比例使内容填满 ViewBox
4. 生成平移→缩放→平移的组合矩阵

这确保 Ant Design 图标在不同 ViewBox 下都能居中且最大化显示。

---

## 5. AntDesignIconProvider

```csharp
public class AntDesignIconProvider : IconProvider<AntDesignIconKind>
{
    protected override Type GetTypeForKind(AntDesignIconKind kind)
    {
        var typeName = $"AtomUI.Icons.AntDesign.{kind.ToString()}";
        var type = Type.GetType(typeName)
                   ?? Assembly.GetExecutingAssembly().GetType(typeName);
        if (type == null)
            throw new InvalidOperationException($"Type {typeName} does not exist");
        return type;
    }
}
```

映射逻辑非常直接：枚举值名称 == 类名。例如 `AntDesignIconKind.CheckCircleFilled` → `AtomUI.Icons.AntDesign.CheckCircleFilled` 类。

---

## 6. AXAML 命名空间注册

```csharp
// Properties/AssemblyInfo.cs
[assembly: XmlnsPrefix("https://atomui.net/icons/antdesign", "antd-icons")]
[assembly: XmlnsDefinition("https://atomui.net/icons/antdesign", "AtomUI.Icons.AntDesign")]
```

这使得在 AXAML 中可以通过以下方式使用：

```xml
xmlns:antdicons="https://atomui.net/icons/antdesign"

<!-- 作为 XML 元素 -->
<antdicons:LoadingOutlined LoadingAnimation="Spin"/>

<!-- 作为 MarkupExtension -->
Icon="{antdicons:AntDesignIconProvider Kind=CheckCircleFilled}"
```

---

## 7. 依赖关系总结

```
构建时依赖：
  AtomUI.Icons.AntDesign.Generator
  ├── AtomUI.Icons.Shared（SvgParser, DefaultIconPackageGenerator）
  └── AtomUI.Icons.Shared
      └── AtomUI.Core（Icon, DrawingInstruction, IconThemeType 等枚举）

运行时依赖：
  AtomUI.Icons.AntDesign
  └── AtomUI.Core（Icon, IconProvider<T>, DrawingInstruction 等）
```

> `AtomUI.Icons.AntDesign` 运行时**不依赖** `AtomUI.Icons.Shared`，后者的职责纯粹是构建时代码生成。


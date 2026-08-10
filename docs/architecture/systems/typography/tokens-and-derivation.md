# 字体 Token 与派生算法

整个文本尺寸体系只有一个可调输入：`FontSize`。其余 21 个字号、行高与文本高度 Token 都从它算出来。本文讲这些
Token 怎么分级、阶梯算法长什么样、以及三个算法变体各自做了什么——其中 Compact 的行为与 Ant Design 不一致。

字体族的解析是另一条线，见 [font-family-resolution.md](font-family-resolution.md)；Token 怎么被消费见
[consumption.md](consumption.md)。

## Token 分级

字体相关共 23 个 Token，分 Seed、Map、Alias 三级。分级由 `[DesignTokenKind]` 声明、Generator 写进
`TokenDescriptor`，决定一个 Token 会不会被算法重算。

Seed 只有两个，是唯一的外部输入：

| Token | 类型 | 默认值 | 语义 |
|---|---|---|---|
| `FontFamily` | `FontFamily?` | 见 [默认链](font-family-resolution.md) | 全局字体族 |
| `FontSize` | `double` | `14` | 基准字号，整条阶梯的派生基数 |

`FontSize` 有个容易看错的地方：它既是 Seed，又会被派生算法重新写一遍。算法把它设为阶梯的第 1 档，而第 1 档被
强制等于基数本身，所以这次写回是幂等的，不会每轮编译漂移一点。Ant Design 里 `fontSize` 同时属于 `SeedToken`
和 `MapToken`，模型是一致的。

Map 级 19 项全部由 `CalculateFontMapTokenValues` 从 `FontSize` 派生：

| 分组 | Token | 语义 |
|---|---|---|
| 字号 | `FontSizeSM` / `FontSizeLG` / `FontSizeXL` | 小、大、超大号字号 |
| 标题字号 | `FontSizeHeading1` … `FontSizeHeading5` | 一至五级标题字号 |
| 相对行高 | `RelativeLineHeight` / `RelativeLineHeightSM` / `RelativeLineHeightLG` | 行高**比值**，不是像素 |
| 标题相对行高 | `RelativeLineHeightHeading1` … `RelativeLineHeightHeading5` | 一至五级标题行高比值 |
| 文本高度 | `FontHeight` / `FontHeightSM` / `FontHeightLG` | `round(相对行高 × 字号)`，像素 |

命名上 AtomUI 用 `RelativeLineHeight*`，对应 Ant Design 的 `lineHeight*`。这个改名是刻意的：Avalonia 的
`TextBlock.LineHeight` 是绝对像素，沿用 `LineHeight` 会和框架属性撞语义，让人以为可以直接赋值。`Relative` 前缀
把"这是个比值、必须乘字号"写进了名字里。那次乘法归谁做，见 [consumption.md](consumption.md)。

Alias 级两项在 `DesignToken.CalculateAliasTokenValues` 里赋值，是固定关系而不是独立参数：

| Token | 类型 | 派生 | 语义 |
|---|---|---|---|
| `FontSizeIcon` | `double` | `= FontSizeSM` | 选择器、级联选择器等操作图标字号 |
| `FontWeightStrong` | `FontWeight` | `= FontWeight.SemiBold` | 标题与选中项字重 |

要改它们应该覆盖 Token 本身，而不是去动派生关系——后者会影响所有依赖这条关系的控件。

## 阶梯算法

`CalculatorUtils.CalculateFontSize` 生成 10 档字号，第 `index` 档（`i = index - 1`）是：

```text
baseSize = base × e^(i / 5)
intSize  = index > 1 ? floor(baseSize) : ceil(baseSize)
size     = floor(intSize / 2) × 2          // 取偶
size[1]  = base                            // 第 1 档强制等于基数
```

行高反向递减，字号越小相对行距越宽松：

```text
relativeLineHeight(fontSize) = (fontSize + 8) / fontSize
```

这两个公式与 Ant Design 的 `genFontSizes` / `getLineHeight` 逐步等价。一处实现差异：AtomUI 把自然常数硬编码成
`2.71828` 而不是用 `Math.E`。在常用基数（12–20）上取偶运算把精度差吃掉了，两边输出完全一致，但这个常量本身是
隐患，不该扩散到新代码。

### 档位怎么映射到 Token

```text
下标      0        1       2         3        4    5    6    7-9
字号   SizeSM   Size   SizeLG    SizeXL    H3   H2   H1   未使用
                       Heading5  Heading4
```

`FontSizeLG` 和 `FontSizeHeading5` 共用第 2 档，`FontSizeXL` 和 `FontSizeHeading4` 共用第 3 档。第 7 到 9 档算了
但没有映射到任何 Token，是为对齐上游算法保留的冗余档位。

### 基准值展开

`FontSize = 14` 时的完整输出：

| Token | 值 | Token | 值 |
|---|---|---|---|
| `FontSizeSM` | 12 | `RelativeLineHeightSM` | 1.6667 |
| `FontSize` | 14 | `RelativeLineHeight` | 1.5714 |
| `FontSizeLG` | 16 | `RelativeLineHeightLG` | 1.5 |
| `FontSizeXL` | 20 | `FontHeight` | 22 |
| `FontSizeHeading1` | 38 | `FontHeightSM` | 20 |
| `FontSizeHeading2` | 30 | `FontHeightLG` | 24 |
| `FontSizeHeading3` | 24 | | |
| `FontSizeHeading4` | 20 | | |
| `FontSizeHeading5` | 16 | | |

标题相对行高随级别递减：`H1 1.2105`、`H2 1.2667`、`H3 1.3333`、`H4 1.4`、`H5 1.5`。

`DesignToken.FontMap.cs` 里 `FontSizeHeading1..5` 带着 `38 / 30 / 24 / 20 / 16` 的字段初始值。这些数字正是
`FontSize = 14` 的派生结果，派生跑完会被同值覆盖。它们的作用是让类型在主题还没编译时也持有合理默认，不是第二处
真源，改基准字号时不需要跟着改。

## 三个算法变体

| 算法 | 是否重算字体 Map Token | 派生基数 |
|---|---|---|
| `DefaultThemeVariantCalculator` | 是 | `designToken.FontSize` |
| `CompactThemeVariantCalculator` | 是 | `designToken.FontSize` |
| `DarkThemeVariantCalculator` | 否 | 不适用 |

Dark 完全不碰字体，这是对的——外观变化只该影响色彩，不该改排版度量。Dark 与 Default 或 Compact 组合时，字体
Token 由后者决定。

### Compact 实际上没有压缩字号

`CompactThemeVariantCalculator.Evaluate` 里字体那部分是：

```csharp
CalculateCompactSizeMapTokenValues(nextMap);
CalculatorUtils.CalculateFontMapTokenValues(nextMap);   // 基数 = nextMap.FontSize
nextMap.ControlHeight = controlHeight;                  // controlHeight - 4
CalculatorUtils.CalculateControlHeightMapTokenValues(nextMap);
```

`CalculateFontMapTokenValues` 内部拿的是 `designToken.FontSize`，而这个值在此之前已经被 Default 算法写成了 Seed
基数本身。加上第 1 档强制等于基数，这次重新派生是幂等的——算完和没算一样。

结论是 **Compact 只压缩尺寸阶梯和控件高度，字号与行高原封不动**。`FontSize = 14` 时 Compact 与 Default 的全部
字体 Token 完全相同：`FontSize 14`、`FontSizeSM 12`、`FontSizeLG 16`、`FontSizeXL 20`、`FontSizeHeading1 38`。

### 与 Ant Design 的差异

上游 compact 算法用 `fontSizeSM` 而不是 `fontSize` 当基数：

```ts
const fontSize = mergedMapToken.fontSizeSM; // Smaller size font-size as base
return { ...mergedMapToken, ...genCompactSizeMapToken(...), ...genFontMapToken(fontSize), ... };
```

同样输入 `fontSize = 14`，两边的 Compact 输出不同：

| Token | Ant Design Compact | AtomUI Compact | Default（两者一致） |
|---|---|---|---|
| `FontSize` | 12 | 14 | 14 |
| `FontSizeSM` | 10 | 12 | 12 |
| `FontSizeLG` | 14 | 16 | 16 |
| `FontSizeXL` | 16 | 20 | 20 |
| `FontSizeHeading1` | 32 | 38 | 38 |

有一条线索表明这是疏漏而非取舍：`CalculateFontMapTokenValues` 里留着上游那句注释 `// Smaller size font-size as
base`。它在 Ant Design 中位于 compact 算法内部、专门解释为什么取 `fontSizeSM`；搬进 AtomUI 的共享派生函数之后就
不再成立了，因为这个函数同时服务 Default 和 Compact，基数始终是 `FontSize`。注释与代码说的是两件事。

修正方向是让 Compact 以 `FontSizeSM` 为基数派生字体 Map Token，同时不改变 Default 路径的输出。这属于视觉破坏性
变更，会影响所有启用 Compact 的应用，须按 [verification.md](verification.md) 的兼容性矩阵评估。

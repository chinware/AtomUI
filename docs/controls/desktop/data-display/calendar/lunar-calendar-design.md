# LunarCalendar 农历能力设计

本文档定义 `LunarCalendar` 的稳定模型、Public API、显示模式、历法算法、节假日扩展、主题集成和验证边界。Calendar 家族总览见 [Calendar 桌面版架构设计](overview.md)，共享实现见 [Calendar 桌面版实现原理](implementation.md)，行为基础见 [Calendar 行为设计](behavior-design.md)，Token 语义见 [Calendar Token 设计](token.md)，变化记录见 [Calendar Changelog](changelog.md)。

## 1. 设计定位

`LunarCalendar` 是 `Calendar` 家族中的中国农历日历控件，位于 `AtomUI.Desktop.Controls` 包和 `AtomUI.Desktop.Controls` 命名空间。它继承 `Calendar` 的公历面板、单值选择、Header、范围限制、禁用规则、周序号、模板、范围条、焦点导航、Automation 和事件顺序，在同一日期网格中增加农历日期、二十四节气、传统节日、周末以及应用提供的法定节假日与调休工作日语义。

`LunarCalendar` 同时支持：

- `Fullscreen=true` 的大日历布局。
- `Fullscreen=false` 的卡片日历布局。
- `CalendarMode.Month` 的 6×7 日期网格。
- `CalendarMode.Year` 的 3×4 公历月份网格。

全部运行时实现归属 `src/AtomUI.Desktop.Controls/Calendar`。控件不进入 `AtomUI.Desktop.Controls.Extras`，不建立独立 NuGet 包，也不把农历算法放入 Gallery、应用层或第三方运行时依赖。

## 2. 设计原则

1. `Calendar.Value` 是唯一选中日期和面板锚点；农历日期不是第二份可写选择状态。
2. 公历日期负责导航、选择、范围和事件，农历数据是对公历日期的确定性投影。
3. 大日历与卡片日历共享同一 model、历法面板数据和容器池，只由专用 Cell 主题改变布局密度。
4. 农历算法、节气和传统节日为内置纯计算；法定节假日和调休工作日只通过公开 Provider 接入。
5. Cell 和 Header 的农历视觉由 Calendar 模块内部的专用 presenter/子控件负责，不允许从 `LunarCalendar` ControlTheme 穿透其他控件模板内部设置样式。
6. 内置语义使用枚举和数值表达；内置农历显示名称固定为简体中文，Provider 的节假日名称由应用自行决定。
7. 支持范围、算法数据、失效条件和退化规则必须确定，不依赖机器时区、当前平台或网络状态。
8. Calendar 原有 Public API、事件顺序、模板优先级、键盘和 Automation 行为保持不变。

## 3. 模型与 Public API

### 3.1 LunarCalendar

```csharp
public class LunarCalendar : Calendar
{
    public static CalendarDateRange SupportedRange { get; }

    public bool ShowSolarTerms { get; set; }
    public bool ShowTraditionalFestivals { get; set; }
    public bool ShowHolidays { get; set; }
    public bool HighlightWeekends { get; set; }

    public ILunarCalendarHolidayProvider? HolidayProvider { get; set; }

    public LunarCalendarDateInfo SelectedLunarDateInfo { get; }

    public void RefreshHolidayData();
}
```

| API | 默认值 | 语义 |
| --- | --- | --- |
| `SupportedRange` | `1900-01-01` 至 `2100-12-31` | 农历、节气和传统节日保证正确的公历日期范围，两端包含。 |
| `ShowSolarTerms` | `true` | 是否允许二十四节气参与 Cell 次级文案。 |
| `ShowTraditionalFestivals` | `true` | 是否允许内置传统节日参与 Cell 次级文案。 |
| `ShowHolidays` | `true` | 是否查询并显示 `HolidayProvider` 提供的节假日和调休工作日。 |
| `HighlightWeekends` | `true` | 是否使用周末文本状态；调休工作日覆盖普通周末状态。 |
| `HolidayProvider` | `null` | 应用提供的法定节假日和调休工作日数据来源。 |
| `SelectedLunarDateInfo` | 当前 `Value` 的农历投影 | 只读 DirectProperty；随有效 `Value` 和语言无关的历法数据更新。 |

`Value` 在 `LunarCalendar` 上继续规范化到 `.Date`，并收敛到 `SupportedRange`。程序设值只更新属性和渲染，不触发 `PanelChanged`、`ValueChanged` 或 `Selected`。`DisabledDate` 不参与程序设值收敛，保持 Calendar 原有契约。

有效选择约束为 `SupportedRange`、`ValidRange` 和 `DisabledDate` 的组合。`ValidRange` 的公开值不被改写；内部使用它与 `SupportedRange` 的交集。两者无交集时 Header 年/月选项不可用、全部业务 Cell 禁用，Month/Year 模式切换仍然可用。

### 3.2 历法语义类型

`LunarCalendarDateInfo` 是不可变的日期语义模型：

```csharp
public sealed record LunarCalendarDateInfo(
    DateTime SolarDate,
    int LunarYear,
    int LunarMonth,
    int LunarDay,
    bool IsLeapMonth,
    ChineseHeavenlyStem HeavenlyStem,
    ChineseEarthlyBranch EarthlyBranch,
    ChineseZodiac Zodiac,
    ChineseSolarTerm? SolarTerm,
    IReadOnlyList<ChineseTraditionalFestival> TraditionalFestivals);
```

`TraditionalFestivals` 对外暴露只读不可变集合。`SolarDate` 规范化到 `.Date`，数值月份范围为 1-12，日期范围由对应农历月实际天数决定。

`ChineseSolarTerm` 固定定义 `MinorCold`、`MajorCold`、`StartOfSpring`、`RainWater`、`AwakeningOfInsects`、`SpringEquinox`、`PureBrightness`、`GrainRain`、`StartOfSummer`、`GrainBuds`、`GrainInEar`、`SummerSolstice`、`MinorHeat`、`MajorHeat`、`StartOfAutumn`、`EndOfHeat`、`WhiteDew`、`AutumnEquinox`、`ColdDew`、`FrostDescent`、`StartOfWinter`、`MinorSnow`、`MajorSnow`、`WinterSolstice`。

`ChineseZodiac` 固定定义 `Rat`、`Ox`、`Tiger`、`Rabbit`、`Dragon`、`Snake`、`Horse`、`Goat`、`Monkey`、`Rooster`、`Dog`、`Pig`。`ChineseHeavenlyStem` 和 `ChineseEarthlyBranch` 分别定义十天干和十二地支。算法层不使用“清明”“春节”“龙”等字符串判断业务状态。

`ChineseTraditionalFestival` 的内置集合为：

- `SpringFestival`、`LanternFestival`、`DragonHeadFestival`、`DragonBoatFestival`、`QixiFestival`、`ZhongyuanFestival`。
- `MidAutumnFestival`、`DoubleNinthFestival`、`LabaFestival`、`LunarNewYearsEve`。
- 由 `ChineseSolarTerm.PureBrightness` 派生的 `QingmingFestival`。

普通农历节日不在闰月重复触发。除夕按农历年的实际最后一天判断，不固定写成腊月三十。区域性小年和地方节日不属于内置集合。

### 3.3 模板上下文

`CalendarCellContext` 保持原有成员并允许继承。`LunarCalendarCellContext` 在其基础上增加：

- 日期 Cell 的 `LunarCalendarDateInfo`。
- 月份 Cell 实际相交的只读 `LunarCalendarMonthInfo` 序列；每项包含 `LunarYear`、`LunarMonth` 和 `IsLeapMonth`。
- 当前日期的 `LunarCalendarHoliday`。
- `SecondaryText` 和 `LunarCalendarSecondaryContentKind`。
- `IsWeekend`、`IsHoliday` 和 `IsAdjustedWorkday`。

`LunarCalendarSecondaryContentKind` 固定定义 `LunarDay`、`SolarTerm`、`TraditionalFestival`、`Holiday` 和 `Workday`，使自定义模板可以根据语义选择视觉而不解析字符串。

Week Cell 继续不创建日期/月模板上下文。超出 `SupportedRange` 的补位日期保留公历显示但没有农历上下文，并保持禁用。

`CellTemplate` 替换内置农历次级内容区，保留默认公历日期/月值；`FullCellTemplate` 替换完整 Cell 内容并优先于 `CellTemplate`。两者的 DataContext 都是 `LunarCalendarCellContext`，但都不能替换容器的选择、禁用、焦点、命中测试或 Automation 语义。

`CalendarHeaderContext` 保持原有命令和状态成员并允许继承。`LunarCalendarHeaderContext` 增加当前 `LunarCalendarDateInfo`、`SupportedRange`、当前公历年份的农历年份显示文本以及当前公历月份的农历月份范围文本。`HeaderTemplate` 的 DataContext 是 `LunarCalendarHeaderContext`；提交值和模式仍使用继承自 CalendarHeaderContext 的命令，不向模板暴露 LunarCalendar 实例。

### 3.4 节假日 Provider

```csharp
public interface ILunarCalendarHolidayProvider
{
    bool TryGetHolidays(
        CalendarDateRange visibleRange,
        CultureInfo culture,
        out IReadOnlyList<LunarCalendarHoliday> holidays);
}

public sealed record LunarCalendarHoliday(
    DateTime Date,
    string Name,
    LunarCalendarHolidayKind Kind);

public enum LunarCalendarHolidayKind
{
    Holiday,
    Workday
}
```

Provider 是同步的面板级数据接口，不承担网络请求、缓存下载或后台任务。应用先在控件外加载数据，再更新 Provider 自身缓存并调用 `RefreshHolidayData()`。

Provider 契约：

- 只有 `ShowHolidays=true` 且 `HolidayProvider` 非 null 时才查询。仅 Month 日期面板查询 Provider，范围是完整 42 Cell 可见日期与 `SupportedRange` 的交集；Year 月份面板不展示逐日节假日，因此不查询 Provider。
- 同一份面板数据最多调用 Provider 一次，不按 Cell 重复调用。
- `false` 表示当前没有可用数据；`true` 必须返回非 null 的只读集合，空集合表示查询成功但没有条目。
- 控件立即复制返回条目，不保留 Provider 的集合引用。日期规范化到 `.Date`；查询范围外和支持范围外条目忽略；同一天重复条目以后出现的条目为准。
- `Holiday`/`Workday` 以外的非法枚举值属于 Provider 契约错误并抛出 `ArgumentOutOfRangeException`。`Name` 可以为空；空名称仍保留状态标记，但次级文案继续按传统节日、节气、农历日回退。
- Provider 对可预期的暂不可用状态返回 `false`；Provider 抛出的异常不被静默吞掉。
- 周末只是视觉状态，不自动生成 `Holiday` 条目；法定节假日和调休语义完全由 Provider 决定。

## 4. 显示模式与视觉语义

### 4.1 模式矩阵

| 布局 | Month 模式 | Year 模式 |
| --- | --- | --- |
| 大日历 | 42 个日期 Cell；公历日和农历次级文案在右上内容区右对齐，节假日/调休标记使用独立状态。 | 12 个公历月份 Cell；显示公历月份和该月实际相交的农历月份序列。 |
| 卡片日历 | Token 派生的双行紧凑 Cell；第一行为公历日，第二行为农历次级文案。 | 3×4 双行月份 Cell；第一行为公历月份，第二行为精简农历月份范围。 |

`Fullscreen` 只改变布局、尺寸和对齐，不改变历法结果、选择值、事件或 Provider 查询语义。`ShowWeek` 继续只影响 Month 日期网格；周序号列不显示农历内容。

卡片模式不声明固定根宽度，Header 和 7 列日期网格共同参与期望尺寸计算。宿主需要保留紧凑基线时应设置 `MinWidth`，不能使用固定 `Width` 截断 Header；当年份、月份或模式切换内容需要更多空间时，卡片宽度随内容自动增长。

卡片内容高度同样属于呈现适配器的布局契约。普通 Calendar 保持 256；LunarCalendar 使用 `FontHeightSM + (MiniDateCellSize + MarginXS) * 6`，让 WeekHeader 和六行双行 Cell 共享一个可预测的测量结果，并在相邻周行之间保留 `MarginXS`。该间距不能通过选中 Cell 的 Margin、日期特判或 Gallery 外层高度补丁实现。

### 4.2 日期 Cell

日期 Cell 的默认次级文案优先级固定为：

```text
HolidayProvider annotation
  > traditional festival
  > solar term
  > lunar day
```

Provider 的 `Holiday` 和 `Workday` 都属于最高优先级注记；两者通过不同标记和状态区分。传统节日与节气同时存在时显示传统节日。清明同时属于节气和传统节日时只显示一次传统节日文案。

相邻月份但仍在 `SupportedRange` 内的补位日期继续显示农历次级文案，并整体使用 outside 状态。超出支持范围的补位日期只显示公历日，不调用农历、节气、节日或 Provider 算法。

周六、周日由公历 `DayOfWeek` 判断。`HighlightWeekends=true` 时，普通周末的公历日使用周末文本色；同日存在 `Workday` 时取消周末状态并显示调休工作日标记。`Holiday` 不要求日期本身是周末。

### 4.3 月份 Cell 与 Header

Year 模式的每个公历月份扫描该月全部支持日期，按首次出现顺序收集实际相交的农历月份。显示不能只读取公历月第一天的农历月份；跨农历年、闰月或一个公历月相交三个农历月时仍保留完整语义序列。卡片布局可以把可见文本压缩为首尾范围，但模板上下文和 Automation 名称保留完整序列。

默认 Header 继续使用 Year Select、Month Select 和 `OptionButtonGroup`。农历呈现适配器只负责选项文字：

- 公历年份选项以该公历年春节进入的农历年生成干支和生肖说明。
- 公历月份选项使用该月实际相交的农历月份范围。
- Header 的目标值提交、日期收敛、模式切换和事件顺序仍由 `Calendar` 处理。

## 5. 架构、源码边界与职责

```text
src/AtomUI.Desktop.Controls/Calendar/
├── Calendar.cs                         # Calendar 家族唯一选择/模式/事件 owner
├── LunarCalendar.cs                    # 农历公开属性、支持范围和面板数据失效入口
├── LunarCalendarDateInfo.cs            # 公开不可变历法语义
├── LunarCalendarCellContext.cs         # 农历模板上下文
├── LunarCalendarHoliday.cs             # 节假日公共模型和 Provider 契约
├── LunarCalendarEnums.cs               # 节气、节日、生肖、干支等枚举
├── LunarCalendarToken.cs               # 农历专属视觉 Token
├── Lunar/                               # 纯计算、数据表、格式化和面板数据
├── Internal/                            # Calendar 呈现适配器与专用 Cell
├── Themes/                              # Calendar/LunarCalendar 精确 ControlTheme
└── Localization/                        # Calendar 家族语言资源
```

稳定职责：

| 单元 | 职责 | 不负责 |
| --- | --- | --- |
| `Calendar` | `Value`、`Mode`、事件顺序、模板 part、语言和范围条生命周期。 | 农历计算和农历文案。 |
| `LunarCalendar` | 农历功能开关、Provider、支持范围、Selected 农历投影和呈现失效。 | 第二份选中值或独立导航。 |
| `ICalendarPresentationAdapter` | CalendarView Cell 工厂、Cell context、Automation 文案、Header 选项文本和布局 metrics 的 internal 协作。 | Public 扩展或运行时插件发现。 |
| `LunarCalendarPresentationAdapter` | 构建一个可见面板的农历/节假日数据，并把数据投影到专用 Cell/Header。 | 修改 Calendar 事件或选择规则。 |
| `ChineseLunarCalendarEngine` | 公历/农历转换和农历年月日结构。 | 本地化和 UI 状态。 |
| `SolarTermResolver` | 查询 UTC+08:00 民用日期对应的节气枚举。 | 传统节日和法定节假日。 |
| `TraditionalFestivalResolver` | 从农历日期和节气生成内置传统节日枚举。 | 地方节日或调休。 |
| `LunarCalendarViewCell` | 双行内容、标记、专用属性和自身主题。 | 穿透修改其他控件模板。 |

`ICalendarPresentationAdapter` 是 internal 的 Calendar 家族协作契约，不作为第三方历法插件 API。普通 `Calendar` 使用无状态默认适配器，输出当前完全一致的 `CalendarViewCell`、`CalendarCellContext`、Header 文本、Automation 名称和 Mini 高度；`LunarCalendar` 使用实例级适配器、有界面板数据和农历专用 Mini/Fullscreen 布局资源。

## 6. Template 与主题集成

`LunarCalendar` 拥有精确 Control identity 和 `LunarCalendarTheme`。根主题基于 Calendar 的根布局，不复制选择、Header、View 或范围条状态机。`CalendarView` 通过 internal adapter 创建 `LunarCalendarViewCell`，后者拥有独立 `LunarCalendarViewCellTheme` 和强类型呈现属性。

主题约束：

- 不在 `LunarCalendarTheme` 中使用 `/template/` 进入 `CalendarHeader`、`CalendarView`、`ComboBox`、`OptionButtonGroup` 或普通 `CalendarViewCell` 的内部。
- Header 农历标签通过 adapter 数据进入现有 Header 自身逻辑，不通过外层 selector 修改内部项。
- `LunarCalendarViewCellTheme` 只设置 `LunarCalendarViewCell` 自己的模板和 part。
- `CellTemplate` 和 `FullCellTemplate` 继续由 Cell 容器消费，不要求应用了解内部 presenter。
- Light/Dark、selected、today、outside、disabled、focused 和 pointerover 状态仍由容器伪类驱动。

Calendar 呈现适配器提供 effective `MiniContentHeight`、`FullCellMinHeight` 和 `RangeBarTopOffset`。根 Calendar 解析 adapter 对应的 DynamicResource，再通过强属性把前两项传给 `CalendarView`；`CalendarViewTheme` 只消费自身属性，不知道 LunarCalendar，也不需要被 Lunar 根主题穿透。Fullscreen Month 中，农历次级行位于公历值区域与范围条 lane 区域之间；LunarCalendar 把范围条起点下移一个次级文本行和间距，保证 `RangeBars` 不覆盖农历文案。卡片模式和 Year 模式不渲染范围条 overlay。

## 7. 核心算法、数据流与生命周期

### 7.1 公历与农历转换

输入是 `DateTime.Date` 表示的公历民用日期；`DateTime.Kind` 不参与换算，也不调用 `ToUniversalTime()`。支持范围固定为 `1900-01-01` 至 `2100-12-31`。为覆盖 1900 年 1 月，农历年数据从 1899 年开始。

转换引擎使用不可变的压缩农历年数据描述闰月、月份大小和农历年总天数。公历转农历以已验证 epoch 为基准按日偏移定位农历年、月和日；农历转公历执行反向累计。所有加减法在进入 `DateTime` 前检查范围，支持范围内不得发生溢出。

### 7.2 二十四节气

二十四节气采用 `1900-2100` 的确定性 UTC+08:00 民用日期数据表。运行时通过年份和 `ChineseSolarTerm` 索引读取日期，不使用可能在边界年份产生一天偏差的近似分钟公式，也不依赖操作系统时区数据库。

### 7.3 面板数据

每个 `LunarCalendar` 最多保留当前可见面板的一份预计算数据：

- Month 模式键：锚定公历年/月、Culture、四个显示开关、Provider 引用和 Provider revision。
- Year 模式键：锚定公历年和 Culture；月份相交语义不依赖 HolidayProvider 或逐日显示开关。
- 面板数据包含日期历法信息、月份相交信息、格式化结果和 Provider 日期索引。

同一 Month 网格内仅改变选中日期时，复用历法和 Provider 面板数据，只更新 selected/focus/context 投影；跨月、跨年、模式变化、Culture 变化、Provider/显示开关变化或 `RefreshHolidayData()` 才使对应面板数据失效。`Fullscreen` 变化只更新布局与 Cell 主题状态，不重新计算历法或查询 Provider。

### 7.4 历法文本与界面本地化

历法计算结果只包含数值和枚举。`LunarCalendarFormatter` 将农历月日、二十四节气、传统节日、天干地支和生肖固定格式化为简体中文，不随 Calendar Culture 或界面语言翻译。公历年份、月份、星期、Month/Year 模式按钮和 Provider 提供的 `Name` 仍遵循各自原有的本地化边界；控件不得改写应用提供的 Provider 文案。

### 7.5 生命周期

- adapter 由 `LunarCalendar` 持有，与控件生命周期一致，不注册全局静态事件。
- CalendarView template reapply 或 adapter 变化时释放旧池化 Cell 的 owner、model、context 和专用面板数据引用，再按当前状态重建。
- Provider 不被控件订阅；应用通过显式 `RefreshHolidayData()` 建立失效边界。
- LanguageManager attach/detach、RangeBars resource host 和 Header/View part 释放继续使用 Calendar 现有成对生命周期。
- 静态历法和节气数据只读且无 owner 引用，不需要释放。

## 8. 资源、性能与 AOT 边界

- 农历、节气、传统节日和月份相交算法只在面板数据失效时运行，不进入 Measure、Arrange、Render 或 pointer 热路径。
- Month 面板数据最多处理 42 个日期；Year 面板数据最多处理一个公历年的支持日期。Provider 每份面板数据最多调用一次。
- 面板数据、日期索引和模板上下文均有界，不建立跨控件的无上限静态缓存。
- 内置运行时不依赖 `lunar-csharp`、反射、动态 assembly 扫描、`Activator.CreateInstance`、字符串属性绑定或运行时代码生成。
- 枚举格式化、Control identity、Token、主题和语言资源使用显式代码或现有 Source Generator 注册路径。
- AXAML 使用强类型属性、TemplateBinding 和生成资源；用户模板的数据上下文类型在文档和 Gallery 中明确。
- NativeAOT analyzer 通过后仍需执行 Gallery 的真实 NativeAOT publish 验证。

## 9. Token、兼容性与定制边界

`LunarCalendarToken` 只增加农历视觉语义，不复制 Calendar 的根背景、Header 宽度、普通选中态和 RangeBars 基础 Token。专属语义包括：

- 卡片日期 Cell 尺寸和 Year 模式月份 Cell 宽度。
- 卡片内容高度；由双行 Cell 尺寸、WeekHeader 行高和共享纵向间距派生。
- 次级文本字号、行高和文本色。
- 周末、Holiday 标记和 Workday 标记颜色。
- Fullscreen Cell 最小高度和范围条有效顶部偏移。

精确名称、默认派生和消费位置见 [Calendar Token 设计](token.md)。实例状态、节气类型、节日类型、Provider 数据和文本优先级不能写入 Token。

稳定兼容边界：

- `LunarCalendar` 保持 `Calendar` 的属性、事件和模板优先级；新增 API 只扩展农历能力。
- 普通 Calendar 的默认 adapter 必须保持当前 model 数量、容器类型、视觉、Header 文案、Automation 和性能行为。
- `SupportedRange` 是算法契约；扩大范围必须同时提供完整农历年数据、节气数据和全量验证，不允许只放宽 UI 限制。
- 应用可以替换 Cell/Header 模板和 HolidayProvider，但不能依赖 internal adapter、内部 Cell 类型或压缩数据格式。
- 内置传统节日是稳定枚举集合；新增节日通过扩展枚举和 resolver 完成，不能以未定义字符串旁路。
- 法定节假日、调休日期、地方节日和实时政策数据由应用负责，AtomUI 不内置会随年份政策变化的数据。

## 10. 验证要求

### 10.1 纯算法

- 对 `1900-01-01` 至 `2100-12-31` 的每一天执行公历→农历→公历往返。
- 覆盖 epoch、支持范围首尾、闰月首尾、大小月、农历跨年和除夕动态判断。
- 验证 201 年 × 24 个节气日期，并覆盖清明节派生。
- 在 .NET `ChineseLunisolarCalendar` 可用的重叠范围内进行交叉校验；它只作为测试参照，不进入运行时实现。
- 验证闰月不触发普通固定节日、公历月份完整相交农历月份序列以及跨农历年的 Year Cell。

### 10.2 控件行为

- 覆盖 Fullscreen/Card × Month/Year 四种组合和运行时切换。
- 验证 Value 收敛、SupportedRange/ValidRange 空交集、DisabledDate、outside 补位和事件顺序。
- 验证 Provider 仅在 Month 模式查询、查询范围、单面板数据调用次数、false/空集合、重复日期、越界数据、刷新和异常传播。
- 验证同面板选中变化不重查 Provider，跨面板和显式刷新会失效。
- 验证 `CellTemplate`、`FullCellTemplate`、`HeaderTemplate`、ShowWeek 和 RangeBars 共存。

### 10.3 主题、生命周期与发布

- Light/Dark 下检查 today、selected、outside、disabled、focused、weekend、Holiday 和 Workday 状态。
- 检查卡片双行 Cell 不溢出、不改变 6×7/3×4 拓扑，相邻周行至少保留 `MarginXS`，普通 Calendar Mini 仍为 256；Fullscreen 的农历次级文案不与 RangeBars 重叠。
- 主题契约测试禁止 LunarCalendar root selector 穿透其他控件模板内部。
- 验证 template reapply、Mode/ShowWeek/Fullscreen/Language 切换、detach/reattach 和容器回收不保留旧 context 或 Provider 面板数据。
- 同步 Gallery API/Token 表和大日历、卡片日历示例，运行 Desktop 控件测试、Gallery 测试、`git diff --check` 和真实 NativeAOT publish。

# Form 自定义样式指南

Form 的视觉表现通过 `ControlTheme` + Design Token 系统控制。表单系统包含多个控件（`Form`、`FormItem`、`SubmitButton`、`ResetButton`、`FormItemDecorator`），每个控件都有独立的 ControlTheme。

---

## 1. 通过属性直接控制

Form 系统提供了丰富的属性，覆盖了绝大多数自定义场景：

### 表单级属性

```xml
<!-- 布局模式：水平 / 垂直 / 内联 -->
<atom:Form FormLayout="Horizontal" />
<atom:Form FormLayout="Vertical" />
<atom:Form FormLayout="Inline" />

<!-- 标签宽度比例 -->
<atom:Form LabelColInfo="8*" WrapperColInfo="16*" />

<!-- 固定标签宽度 -->
<atom:Form LabelColInfo="120" WrapperColInfo="*" />

<!-- 标签对齐方式 -->
<atom:Form LabelAlign="Left" />    <!-- 默认 Right -->

<!-- 必填标记模式 -->
<atom:Form RequiredMark="Default" />     <!-- 红色星号 -->
<atom:Form RequiredMark="Optional" />    <!-- 显示 "(optional)" -->
<atom:Form RequiredMark="Hidden" />      <!-- 隐藏标记 -->
<atom:Form RequiredMark="Customize" />   <!-- 自定义标记 -->

<!-- 统一尺寸 -->
<atom:Form SizeType="Large" />
<atom:Form SizeType="Middle" />
<atom:Form SizeType="Small" />

<!-- 统一样式变体 -->
<atom:Form StyleVariant="Outline" />
<atom:Form StyleVariant="Filled" />
<atom:Form StyleVariant="Borderless" />
<atom:Form StyleVariant="Underlined" />

<!-- 标签换行 -->
<atom:Form LabelWrapping="Wrap" />

<!-- 隐藏标签 -->
<atom:Form IsHideItemLabel="True" />

<!-- 显示冒号 -->
<atom:Form IsShowColon="False" />

<!-- 验证反馈图标 -->
<atom:Form IsValidateFeedbackEnabled="True" />
```

### 字段级属性

```xml
<!-- 独立布局覆盖 -->
<atom:FormItem Layout="Vertical" />

<!-- 独立标签宽度覆盖 -->
<atom:FormItem LabelColInfo="4*" WrapperColInfo="20*" />

<!-- 帮助文本 -->
<atom:FormItem Help="Should be combination of numbers and alphabets" />

<!-- 工具提示 -->
<atom:FormItem Tooltip="What do you want others to call you?" />

<!-- 自定义提示图标 -->
<atom:FormItem TooltipIcon="{antdicons:AntDesignIconProvider InfoCircleOutlined}" />

<!-- 单字段验证反馈 -->
<atom:FormItem IsValidateFeedbackEnabled="True" />

<!-- 独立验证触发 -->
<atom:FormItem ValidateTrigger="OnBlur" />

<!-- 验证防抖 -->
<atom:FormItem ValidateDebounce="00:00:01" />
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/FormShowCase.axaml`

---

## 2. 通过 Style 覆盖样式

### 全局 Form 样式

```xml
<Window.Styles>
    <!-- 所有表单统一内边距 -->
    <Style Selector="atom|Form">
        <Setter Property="Padding" Value="16" />
    </Style>
</Window.Styles>
```

### 修改 FormItem 标签样式

```xml
<!-- 修改标签字体大小和颜色 -->
<Style Selector="atom|FormItem /template/ TextBlock#PART_Label">
    <Setter Property="FontSize" Value="16" />
    <Setter Property="FontWeight" Value="SemiBold" />
</Style>

<!-- 修改必填星号颜色 -->
<Style Selector="atom|FormItem /template/ TextBlock#PART_DefaultRequireMark">
    <Setter Property="Foreground" Value="Orange" />
</Style>

<!-- 修改冒号间距 -->
<Style Selector="atom|FormItem /template/ TextBlock#PART_Colon">
    <Setter Property="Margin" Value="4,0,8,0" />
</Style>
```

### 按验证状态定制样式

```xml
<!-- 错误状态下的错误消息颜色 -->
<Style Selector="atom|FormItem[ValidateStatus=Error] /template/ TextBlock#PART_ErrorMsg">
    <Setter Property="Foreground" Value="Red" />
    <Setter Property="FontWeight" Value="Bold" />
</Style>

<!-- 警告状态下的帮助文本颜色 -->
<Style Selector="atom|FormItem[ValidateStatus=Warning] /template/ TextBlock#HelpText">
    <Setter Property="Foreground" Value="DarkOrange" />
</Style>
```

### 按布局定制样式

```xml
<!-- 垂直布局时标签左对齐 + 自定义间距 -->
<Style Selector="atom|FormItem[Layout=Vertical] /template/ Panel#PART_LabelLayout">
    <Setter Property="HorizontalAlignment" Value="Left" />
    <Setter Property="Margin" Value="0,0,0,8" />
</Style>
```

### 按尺寸定制样式

```xml
<!-- 大号尺寸时标签最小高度 -->
<Style Selector="atom|FormItem[SizeType=Large] /template/ Panel#PART_LabelLayout">
    <Setter Property="MinHeight" Value="48" />
</Style>

<!-- 小号尺寸时标签最小高度 -->
<Style Selector="atom|FormItem[SizeType=Small] /template/ Panel#PART_LabelLayout">
    <Setter Property="MinHeight" Value="28" />
</Style>
```

---

## 3. 自定义必填标记

### 使用 Tag 作为自定义标记

```xml
<atom:Form RequiredMark="Customize">
    <atom:Form.CustomRequireMark>
        <atom:Tag TagColor="Error">required</atom:Tag>
    </atom:Form.CustomRequireMark>
    <atom:Form.CustomOptionalMark>
        <atom:Tag TagColor="Warning">optional</atom:Tag>
    </atom:Form.CustomOptionalMark>
    
    <!-- FormItems... -->
</atom:Form>
```

### 使用 DataTemplate 自定义标记

```xml
<atom:Form RequiredMark="Customize">
    <atom:Form.CustomRequireMarkTemplate>
        <DataTemplate>
            <TextBlock Text="★" Foreground="Red" FontSize="12" />
        </DataTemplate>
    </atom:Form.CustomRequireMarkTemplate>
    
    <!-- FormItems... -->
</atom:Form>
```

---

## 4. 自定义验证反馈图标

Form 的验证反馈图标可以通过 FeedbackTemplate 及各状态的模板属性进行替换：

```xml
<atom:Form IsValidateFeedbackEnabled="True">
    <!-- 替换成功图标 -->
    <atom:Form.SuccessFeedbackTemplate>
        <DataTemplate>
            <antdicons:CheckCircleFilled
                FillBrush="{atom:SharedTokenResource ColorSuccess}"
                Width="{atom:SharedTokenResource IconSize}"
                Height="{atom:SharedTokenResource IconSize}"/>
        </DataTemplate>
    </atom:Form.SuccessFeedbackTemplate>
    
    <!-- 替换错误图标 -->
    <atom:Form.ErrorFeedbackTemplate>
        <DataTemplate>
            <antdicons:CloseCircleFilled
                FillBrush="{atom:SharedTokenResource ColorError}"
                Width="{atom:SharedTokenResource IconSize}"
                Height="{atom:SharedTokenResource IconSize}"/>
        </DataTemplate>
    </atom:Form.ErrorFeedbackTemplate>
    
    <!-- 替换警告图标 -->
    <atom:Form.WarningFeedbackTemplate>
        <DataTemplate>
            <antdicons:ExclamationCircleFilled
                FillBrush="{atom:SharedTokenResource ColorWarning}"/>
        </DataTemplate>
    </atom:Form.WarningFeedbackTemplate>
    
    <!-- 替换验证中图标（旋转加载） -->
    <atom:Form.ValidatingFeedbackTemplate>
        <DataTemplate>
            <antdicons:LoadingOutlined
                LoadingAnimation="Spin"
                StrokeBrush="{atom:SharedTokenResource ColorIcon}"/>
        </DataTemplate>
    </atom:Form.ValidatingFeedbackTemplate>
    
    <!-- FormItems... -->
</atom:Form>
```

---

## 5. 自定义错误/警告消息颜色

```xml
<atom:Form ErrorMessageForeground="DarkRed"
           WarningMessageForeground="DarkOrange">
    <!-- FormItems... -->
</atom:Form>
```

**提示**：默认情况下这些颜色由 Design Token 控制（`ColorErrorText` / `ColorWarningText`），仅在需要覆盖全局主题时才手动设置。

---

## 6. 自定义删除按钮图标

动态表单中的删除按钮图标可以替换：

```xml
<atom:Form IsShowItemDeleteButton="True"
           MinItemCount="1">
    <atom:Form.ItemDeleteButtonIcon>
        <atom:IconTemplate>
            <antdicons:DeleteOutlined/>
        </atom:IconTemplate>
    </atom:Form.ItemDeleteButtonIcon>
    
    <!-- FormItems... -->
</atom:Form>
```

---

## 7. 通过 ControlTheme 完全替换主题

如果需要彻底替换 FormItem 的模板，可以定义自定义 `ControlTheme`：

```xml
<ControlTheme x:Key="MyCustomFormItem" TargetType="atom:FormItem">
    <Setter Property="Template">
        <ControlTemplate>
            <!-- 自定义模板... -->
        </ControlTemplate>
    </Setter>
</ControlTheme>

<!-- 使用 -->
<atom:FormItem Theme="{StaticResource MyCustomFormItem}" LabelText="Custom" FieldName="custom">
    <atom:LineEdit />
</atom:FormItem>
```

> ⚠️ 注意：完全替换 ControlTheme 将丢失内置的验证状态展示、必填标记、响应式布局等功能。建议优先使用 Style 覆盖或属性控制。

---

## 样式选择器速查

> 说明：Avalonia 样式选择器中使用 `atom|Form` 语法引用 `atom` XML 命名空间下的控件类型，其中 `|` 是命名空间分隔符。

### Form 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|Form` | 匹配所有 Form 实例 |
| `atom\|Form[FormLayout=Horizontal]` | 匹配水平布局的表单 |
| `atom\|Form[FormLayout=Vertical]` | 匹配垂直布局的表单 |
| `atom\|Form[FormLayout=Inline]` | 匹配内联布局的表单 |
| `atom\|Form[SizeType=Large]` | 匹配大号尺寸的表单 |
| `atom\|Form[SizeType=Small]` | 匹配小号尺寸的表单 |
| `atom\|Form:disabled` | 匹配禁用状态的表单 |

### FormItem 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|FormItem` | 匹配所有 FormItem 实例 |
| `atom\|FormItem[Layout=Horizontal]` | 匹配水平布局的字段项 |
| `atom\|FormItem[Layout=Vertical]` | 匹配垂直布局的字段项 |
| `atom\|FormItem[IsRequired=True]` | 匹配必填的字段项 |
| `atom\|FormItem[ValidateStatus=Error]` | 匹配验证错误状态的字段项 |
| `atom\|FormItem[ValidateStatus=Warning]` | 匹配验证警告状态的字段项 |
| `atom\|FormItem[ValidateStatus=Success]` | 匹配验证成功状态的字段项 |
| `atom\|FormItem[ValidateStatus=Validating]` | 匹配验证中状态的字段项 |
| `atom\|FormItem[ValidateStatus=Default]` | 匹配未验证状态的字段项 |
| `atom\|FormItem[SizeType=Large]` | 匹配大号尺寸的字段项 |
| `atom\|FormItem[SizeType=Middle]` | 匹配中号尺寸的字段项 |
| `atom\|FormItem[SizeType=Small]` | 匹配小号尺寸的字段项 |
| `atom\|FormItem[RequiredMark=Default]` | 匹配使用默认必填标记的字段项 |
| `atom\|FormItem[RequiredMark=Optional]` | 匹配使用可选标记模式的字段项 |
| `atom\|FormItem[RequiredMark=Customize]` | 匹配使用自定义标记的字段项 |
| `atom\|FormItem[HasErrorOrWarningMsg=True]` | 匹配有错误/警告消息的字段项 |
| `atom\|FormItem[HasErrorOrWarningMsg=False]` | 匹配无错误/警告消息的字段项 |
| `atom\|FormItem[IsValueItem=True]` | 匹配数据项（非 FormActionsItem） |
| `atom\|FormItem[IsShowItemDeleteButton=True]` | 匹配显示删除按钮的字段项 |

### FormItem 模板部件

| 选择器 | 部件说明 |
|---|---|
| `atom\|FormItem /template/ DockPanel#PART_RootLayout` | 根布局面板 |
| `atom\|FormItem /template/ Panel#PART_LabelLayout` | 标签区域容器 |
| `atom\|FormItem /template/ TextBlock#PART_Label` | 标签文本 |
| `atom\|FormItem /template/ TextBlock#PART_Colon` | 冒号文本 |
| `atom\|FormItem /template/ TextBlock#PART_DefaultRequireMark` | 红色星号（*）必填标记 |
| `atom\|FormItem /template/ Grid#PART_BodyLayout` | 主体布局（标签 + 内容） |
| `atom\|FormItem /template/ StackPanel#PART_ContentLayout` | 内容区域 |
| `atom\|FormItem /template/ TextBlock#PART_ErrorMsg` | 错误/警告消息文本 |
| `atom\|FormItem /template/ TextBlock#HelpText` | 帮助文本 |
| `atom\|FormItem /template/ ContentPresenter#ContentPresenter` | 内容控件展示器 |
| `atom\|FormItem /template/ ContentPresenter#ExtraPresenter` | 额外内容展示器 |
| `atom\|FormItem /template/ ContentPresenter#CustomRequiredMarkPresenter` | 自定义必填标记展示器 |
| `atom\|FormItem /template/ ContentPresenter#CustomOptionalMarkPresenter` | 自定义可选标记展示器 |
| `atom\|FormItem /template/ TextBlock#OptionalMark` | "(optional)" 文本标记 |
| `atom\|FormItem /template/ atom\|IconPresenter#TooltipIconPresenter` | 提示图标 |
| `atom\|FormItem /template/ Panel#ItemDeleteButtonLayout` | 删除按钮布局容器 |
| `atom\|FormItem /template/ atom\|IconButton#ItemDeleteButton` | 删除按钮 |

### SubmitButton / ResetButton 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|SubmitButton` | 匹配所有提交按钮（继承自 Button，支持所有 Button 选择器） |
| `atom\|ResetButton` | 匹配所有重置按钮（继承自 Button，支持所有 Button 选择器） |
| `atom\|SubmitButton:disabled` | 匹配禁用状态的提交按钮（如 IsWatchValidateResult 时验证未通过） |

### FormActionsItem 选择器

| 选择器 | 说明 |
|---|---|
| `atom\|FormActionsItem` | 匹配所有操作区域项（继承自 FormItem） |

### 组合选择器示例

| 选择器 | 说明 |
|---|---|
| `atom\|FormItem[ValidateStatus=Error] /template/ TextBlock#PART_ErrorMsg` | 错误状态下的错误消息 |
| `atom\|FormItem[ValidateStatus=Warning] /template/ TextBlock#HelpText` | 警告状态下的帮助文本 |
| `atom\|FormItem[SizeType=Large] /template/ Panel#PART_LabelLayout` | 大号尺寸的标签区域 |
| `atom\|FormItem[RequiredMark=Default][IsRequired=True][IsValueItem=True] /template/ TextBlock#PART_DefaultRequireMark` | 默认模式下必填项的星号标记 |
| `atom\|FormItem[IsValueItem=True][IsEffectiveShowItemDeleteButton=True] /template/ atom\|IconButton#ItemDeleteButton` | 可删除的数据项的删除按钮 |

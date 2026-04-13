# Form 使用文档

本文档介绍 AtomUI Form 控件的各种使用方式，示例代码摘自 Gallery 演示程序。

> 📖 Gallery 源码位置：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/FormShowCase.axaml`

---

## 前置准备

在 AXAML 中使用 Form，需要引入以下命名空间：

```xml
xmlns:atom="https://atomui.net"
xmlns:antdicons="https://atomui.net/icons/antdesign"
```

在 C# 代码中引入：

```csharp
using AtomUI.Desktop.Controls;   // Form, FormItem, SubmitButton, ResetButton 等
using AtomUI.Controls;            // FormLayout, FormValidateTrigger, SizeType 等共享类型
```

---

## 1. 基本用法

最基本的表单包含若干 `FormItem` 和一个 `SubmitButton`。每个 `FormItem` 通过 `LabelText` 设置标签、`FieldName` 指定字段名、`IsRequired` 标记必填，并配置验证器。

```xml
<atom:Form LabelColInfo="8*"
           WrapperColInfo="16*"
           MinWidth="600"
           HorizontalAlignment="Left">
    <atom:FormItem LabelText="Username"
                   FieldName="username"
                   IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please input your username!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit />
    </atom:FormItem>
    <atom:FormItem LabelText="Password"
                   FieldName="password"
                   IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please input your password!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit RevealPassword="False"
                       PasswordChar="•"
                       IsEnableRevealButton="True" />
    </atom:FormItem>
    <atom:FormItem FieldName="remember">
        <atom:CheckBox>Remember me</atom:CheckBox>
    </atom:FormItem>
    
    <atom:FormActionsItem>
        <atom:SubmitButton />
    </atom:FormActionsItem>
</atom:Form>
```

**关键要点**：
- `LabelColInfo="8*"` 和 `WrapperColInfo="16*"` 按 8:16 比例分配标签和内容区域宽度
- `FormActionsItem` 用于放置操作按钮（不参与验证和数据收集）
- `SubmitButton` 点击时自动触发 Form 验证并提交

---

## 2. 表单方法（Submit / Reset）

通过 `SubmitButton` 和 `ResetButton` 组合使用，支持表单的提交与重置操作：

```xml
<atom:Form LabelColInfo="8*"
           WrapperColInfo="16*"
           MinWidth="600"
           HorizontalAlignment="Left">
    <atom:FormItem LabelText="Note"
                   FieldName="note"
                   IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please enter Note" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit />
    </atom:FormItem>
    <atom:FormItem LabelText="Gender"
                   FieldName="gender"
                   IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormNotNullValidator Message="Please enter Gender" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:Select IsAllowClear="True"
                     PlaceholderText="Select a option and change input text above"
                     HorizontalAlignment="Stretch">
            <atom:SelectOption Header="male" Content="male" />
            <atom:SelectOption Header="female" Content="female" />
            <atom:SelectOption Header="other" Content="other" />
        </atom:Select>
    </atom:FormItem>

    <atom:FormActionsItem>
        <StackPanel Orientation="Horizontal" Spacing="10">
            <atom:SubmitButton />
            <atom:ResetButton />
        </StackPanel>
    </atom:FormActionsItem>
</atom:Form>
```

**说明**：`ResetButton` 点击后会自动清除所有字段值并重置验证状态到初始状态。

---

## 3. 表单布局（Horizontal / Vertical / Inline）

通过 `FormLayout` 属性切换三种布局模式。可以在运行时通过数据绑定动态切换。

```xml
<atom:Form LabelColInfo="8*"
           WrapperColInfo="16*"
           FormLayout="{Binding FormLayout}"
           HorizontalAlignment="Left"
           MinWidth="600">
    <atom:FormItem LabelText="Field A" FieldName="FieldA">
        <atom:LineEdit MinWidth="160"/>
    </atom:FormItem>
    <atom:FormItem LabelText="Field B" FieldName="Field B">
        <atom:LineEdit MinWidth="160"/>
    </atom:FormItem>

    <atom:FormActionsItem>
        <atom:SubmitButton />
    </atom:FormActionsItem>
</atom:Form>
```

### FormItem 独立布局覆盖

在同一个 Form 中，不同的 FormItem 可以使用不同的布局方式：

```xml
<atom:Form FormLayout="Horizontal">
    <atom:FormItem LabelText="horizontal" FieldName="horizontal"
                   LabelColInfo="4*"
                   WrapperColInfo="20*"
                   IsRequired="True"
                   Layout="Horizontal">
        <atom:LineEdit/>
    </atom:FormItem>

    <atom:FormItem LabelText="vertical"
                   FieldName="vertical"
                   IsRequired="True"
                   Layout="Vertical">
        <atom:LineEdit/>
    </atom:FormItem>
</atom:Form>
```

**提示**：`FormItem.Layout` 属性可覆盖 `Form.FormLayout` 的全局设置，实现混合布局。

---

## 4. 禁用表单

通过设置 `IsEnabled="False"` 禁用整个表单，所有子控件自动进入禁用状态：

```xml
<atom:CheckBox IsChecked="{Binding IsFormDisabled, Mode=TwoWay}">Form disabled</atom:CheckBox>
<atom:Form LabelColInfo="8*"
           WrapperColInfo="16*"
           MinWidth="600"
           HorizontalAlignment="Left"
           IsEnabled="{Binding IsFormDisabled, Converter={x:Static BoolConverters.Not}}">
    <atom:FormItem LabelText="Checkbox" FieldName="checkbox">
        <atom:CheckBox>Checkbox</atom:CheckBox>
    </atom:FormItem>
    <atom:FormItem LabelText="Input" FieldName="input">
        <atom:LineEdit/>
    </atom:FormItem>
    <atom:FormItem LabelText="Select" FieldName="select">
        <atom:Select HorizontalAlignment="Stretch">
            <atom:SelectOption Header="Demo" Content="demo"/>
        </atom:Select>
    </atom:FormItem>
    <!-- 支持的控件：CheckBox, RadioButton, LineEdit, Select, TreeSelect, Cascader,
         DatePicker, RangeDatePicker, NumericUpDown, TextArea, ToggleSwitch,
         Upload, Slider, ColorPicker, Rate, Mentions, TreeView 等 -->
</atom:Form>
```

---

## 5. 表单样式变体（Outlined / Filled / Borderless / Underlined）

通过 `StyleVariant` 属性统一设置所有子控件的样式变体：

```xml
<atom:Form LabelColInfo="8*"
           WrapperColInfo="16*"
           MinWidth="600"
           HorizontalAlignment="Left"
           StyleVariant="{Binding FormStyleVariant}">
    <atom:FormItem LabelText="Input"
                   FieldName="input"
                   IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please input!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit />
    </atom:FormItem>
    <atom:FormItem LabelText="Select"
                   FieldName="select"
                   IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please input!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:Select HorizontalAlignment="Stretch" />
    </atom:FormItem>

    <atom:FormActionsItem>
        <atom:SubmitButton />
    </atom:FormActionsItem>
</atom:Form>
```

**提示**：`StyleVariant` 支持 `Outline`（默认）、`Filled`、`Borderless`、`Underlined` 四种值，会自动传播到所有支持该属性的子控件。

---

## 6. 必填标记样式（Required Mark）

通过 `RequiredMark` 属性切换必填标记的显示模式，支持 Default、Optional、Hidden、Customize 四种：

```xml
<atom:Form LabelColInfo="8*"
           WrapperColInfo="16*"
           HorizontalAlignment="Left"
           MinWidth="600"
           RequiredMark="{Binding FormRequiredMark}">
    <!-- 自定义必填/可选标记 -->
    <atom:Form.CustomRequireMark>
        <atom:Tag TagColor="Error">required</atom:Tag>
    </atom:Form.CustomRequireMark>
    <atom:Form.CustomOptionalMark>
        <atom:Tag TagColor="Warning">optional</atom:Tag>
    </atom:Form.CustomOptionalMark>
    
    <atom:FormItem IsRequired="True" LabelText="Field A" FieldName="FieldA"
                   Tooltip="This is a required field">
        <atom:LineEdit PlaceholderText="input placeholder"/>
    </atom:FormItem>
    <atom:FormItem LabelText="Field B" FieldName="Field B"
                   Tooltip="Tooltip with customize icon"
                   TooltipIcon="{antdicons:AntDesignIconProvider InfoCircleOutlined}">
        <atom:LineEdit PlaceholderText="input placeholder"/>
    </atom:FormItem>
    <atom:FormActionsItem>
        <atom:SubmitButton/>
    </atom:FormActionsItem>
</atom:Form>
```

**四种标记模式说明**：
- `Default`：必填字段显示红色星号（*）
- `Optional`：非必填字段显示"（optional）"文字
- `Hidden`：隐藏所有必填标记
- `Customize`：使用 `CustomRequireMark` 和 `CustomOptionalMark` 自定义标记内容

---

## 7. 表单尺寸控制（SizeType）

通过 `SizeType` 属性统一控制表单中所有子控件的尺寸：

```xml
<atom:Form LabelColInfo="8*"
           WrapperColInfo="16*"
           MinWidth="600"
           HorizontalAlignment="Left"
           SizeType="{Binding FormSizeType}">
    <atom:FormItem LabelText="Input" FieldName="input">
        <atom:LineEdit />
    </atom:FormItem>
    <atom:FormItem LabelText="InputNumber" FieldName="inputNumber">
        <atom:NumericUpDown Width="200" />
    </atom:FormItem>
    <atom:FormItem LabelText="Switch" FieldName="switch">
        <atom:ToggleSwitch />
    </atom:FormItem>
    <atom:FormItem LabelText="Select" FieldName="select">
        <atom:Select HorizontalAlignment="Stretch" Mode="Multiple">
            <atom:SelectOption Header="BBB"/>
            <atom:SelectOption Header="AAA"/>
        </atom:Select>
    </atom:FormItem>

    <atom:FormActionsItem>
        <atom:SubmitButton />
    </atom:FormActionsItem>
</atom:Form>
```

**说明**：`SizeType` 支持 `Small`、`Middle`（默认）、`Large` 三种值，影响标签高度、控件高度和间距。

---

## 8. 标签换行（Label Wrap）

当标签文本较长时，通过 `LabelWrapping="Wrap"` 允许标签自动换行：

```xml
<atom:Form LabelColInfo="120"
           WrapperColInfo="*"
           MinWidth="600"
           HorizontalAlignment="Left"
           LabelWrapping="Wrap">
    <atom:FormItem LabelText="Normal label"
                   FieldName="username"
                   IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please input your username!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit />
    </atom:FormItem>
    <atom:FormItem LabelText="A super long label text"
                   FieldName="password"
                   IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please input your username!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit />
    </atom:FormItem>
</atom:Form>
```

**提示**：`LabelColInfo="120"` 设置标签区域固定宽度为 120 像素，`WrapperColInfo="*"` 表示内容区占满剩余空间。

---

## 9. 仅警告规则（WarningOnly）

设置 `WarningOnly="True"` 的验证器不阻止表单提交，仅显示警告信息：

```xml
<atom:Form LabelColInfo="8*"
           WrapperColInfo="16*"
           MinWidth="600"
           HorizontalAlignment="Left"
           Submitted="HandleNoBlockFormSubmitted"
           Validated="HandleNoBlockFormValidated">
    <atom:FormItem LabelText="Url"
                   FieldName="url"
                   IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please enter URL!" />
                <atom:FormUrlValidator Message="URL is not a valid url!" WarningOnly="True" />
                <atom:FormStringLengthValidator Message="URL must be at least 6 characters!"
                                                MinimumLength="6"/>
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit />
    </atom:FormItem>

    <atom:FormActionsItem>
       <StackPanel Orientation="Horizontal" Spacing="10">
           <atom:SubmitButton />
           <atom:Button ButtonType="Default" Click="HandleFillClicked">Fill</atom:Button>
       </StackPanel>
    </atom:FormActionsItem>
</atom:Form>
```

**说明**：
- `FormStringNotEmptyValidator` 和 `FormStringLengthValidator` 是错误验证器，会阻止提交
- `FormUrlValidator` 设置了 `WarningOnly="True"`，即使 URL 格式不对也只显示警告，不阻止提交
- 验证按声明顺序执行，配合 `ValidateStrategy` 控制执行方式

---

## 10. 验证触发与防抖（Validate Trigger & Debounce）

可以为不同字段配置不同的验证触发方式、防抖时间和验证策略：

```xml
<atom:Form LabelColInfo="8*"
           WrapperColInfo="16*"
           MinWidth="600"
           HorizontalAlignment="Left"
           IsValidateFeedbackEnabled="True">
    <!-- 失焦时验证 -->
    <atom:FormItem LabelText="Field A"
                   FieldName="fieldA"
                   ValidateTrigger="OnBlur">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringLengthValidator Message="Field A must be up to 3 characters!"
                                                MaximumLength="3"/>
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit PlaceholderText="Validate required onBlur"/>
    </atom:FormItem>

    <!-- 防抖验证（1秒后触发） -->
    <atom:FormItem LabelText="FieldB"
                   FieldName="fieldB"
                   ValidateDebounce="00:00:01">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringLengthValidator Message="Field B must be up to 3 characters!"
                                                MaximumLength="3"/>
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit PlaceholderText="Validate required debounce after 1s"/>
    </atom:FormItem>

    <!-- 短路验证（遇到第一个错误即停） -->
    <atom:FormItem LabelText="FieldC"
                   FieldName="fieldC"
                   ValidateStrategy="StopWhenFirstFailed">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringLengthValidator Message="Field C must be up to 6 characters!"
                                                MaximumLength="6"/>
                <atom:FormStringLengthValidator Message="Continue input to exceed 6 chars!"
                                                MaximumLength="3"/>
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit PlaceholderText="Validate one by one"/>
    </atom:FormItem>
</atom:Form>
```

**验证触发方式**：
- `OnChanged`（默认）：值变化时立即验证
- `OnBlur`：控件失焦时验证
- `OnSubmit`：仅在提交时验证

**验证策略**：
- `StopWhenFirstFailed`（默认）：遇到第一个错误即停止
- `Sequential`：顺序执行所有验证器
- `Parallel`：并行执行所有验证器

---

## 11. 自动禁用提交按钮（Validate Only）

`SubmitButton` 的 `IsWatchValidateResult` 属性可以实时监听表单验证结果，自动启用/禁用按钮：

```xml
<atom:Form LabelColInfo="8*"
           WrapperColInfo="16*"
           MinWidth="600"
           HorizontalAlignment="Left"
           FormLayout="Vertical">
    <atom:FormItem LabelText="Name"
                   FieldName="name"
                   IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please enter Name!"/>
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit />
    </atom:FormItem>

    <atom:FormItem LabelText="Age"
                   FieldName="age"
                   IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please enter Age!"/>
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit />
    </atom:FormItem>

    <atom:FormActionsItem>
        <StackPanel Orientation="Horizontal" Spacing="10">
            <atom:SubmitButton IsWatchValidateResult="True"/>
            <atom:ResetButton />
        </StackPanel>
    </atom:FormActionsItem>
</atom:Form>
```

**说明**：当 `IsWatchValidateResult="True"` 时，只有当所有必填字段验证通过后，提交按钮才变为可用状态。

---

## 12. 动态表单项

支持在运行时动态添加和删除 FormItem：

```xml
<atom:Form LabelColInfo="8*"
           WrapperColInfo="16*"
           MinWidth="600"
           HorizontalAlignment="Left"
           Name="DynamicForm">
    <atom:FormItem LabelText="Passengers"
                   FieldName="passengers">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator
                    Message="Please input passenger's name or delete this field!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit />
    </atom:FormItem>

    <atom:FormActionsItem>
        <atom:Button ButtonType="Dashed"
                     HorizontalAlignment="Stretch"
                     Icon="{antdicons:AntDesignIconProvider PlusOutlined}"
                     Click="HandleAddFormItem">
            Add field
        </atom:Button>
    </atom:FormActionsItem>
    <atom:FormActionsItem>
        <atom:Button ButtonType="Dashed"
                     HorizontalAlignment="Stretch"
                     Icon="{antdicons:AntDesignIconProvider PlusOutlined}"
                     Click="HandleAddFormItemAtHead">
            Add field at head
        </atom:Button>
    </atom:FormActionsItem>
    <atom:FormActionsItem>
        <atom:SubmitButton />
    </atom:FormActionsItem>
</atom:Form>
```

```csharp
// Code-behind：动态添加 FormItem
private int _dynamicFieldIndex;

private void HandleAddFormItem(object? sender, RoutedEventArgs args)
{
    var form = this.FindControl<Form>("DynamicForm")!;
    var formItem = new FormItem
    {
        LabelText = $"Passenger {++_dynamicFieldIndex}",
        FieldName = $"passenger_{_dynamicFieldIndex}",
        Content   = new LineEdit()
    };
    // 插入到最后一个 FormActionsItem 之前
    form.Items.Insert(form.Items.Count - 3, formItem);
}
```

**说明**：
- 使用 `form.Items.Add()` 或 `form.Items.Insert()` 添加新字段
- 使用 `form.DeleteFormItem(item)` 删除字段
- 设置 `IsShowItemDeleteButton="True"` 可在每个数据项旁显示删除按钮
- `MinItemCount` 控制最少保留的数据项数量

---

## 13. 自定义表单控件

第三方或自定义控件需要实现 `IFormItemAware` 接口才能参与表单验证和数据收集：

```xml
<atom:Form LabelColInfo="8*"
           WrapperColInfo="16*"
           MinWidth="600"
           HorizontalAlignment="Left"
           FormLayout="Inline">
    <atom:FormItem LabelText="Price"
                   FieldName="price"
                   Width="350">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <gallery:PriceValidator Message="Price must be greater than zero!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <gallery:PriceInput />
    </atom:FormItem>
    <atom:FormActionsItem>
        <atom:SubmitButton />
    </atom:FormActionsItem>
</atom:Form>
```

**实现 IFormItemAware**：

```csharp
public class PriceInput : UserControl, IFormItemAware
{
    public event EventHandler? ValueChanged;
    
    public void SetFormValue(object? value) { /* 设置值到控件 */ }
    public object? GetFormValue() { /* 返回当前值 */ }
    public void ClearFormValue() { /* 清除值 */ }
    public void NotifyValidateStatus(FormValidateStatus status) { /* 更新状态 */ }
}
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/FormShowCase.axaml` 中 "Customized Form Controls" 示例。

---

## 14. FormItemDecorator 装饰器

当需要在主控件旁边附加额外内容（如单位文本），使用 `FormItemDecorator` 包装：

```xml
<atom:FormItem LabelText="InputNumber" FieldName="inputNumber">
    <atom:FormItemDecorator>
        <atom:NumericUpDown Minimum="1" Maximum="10" Width="120"/>
        <atom:FormItemDecorator.Extra>
            machines
        </atom:FormItemDecorator.Extra>
    </atom:FormItemDecorator>
</atom:FormItem>
```

**说明**：`FormItemDecorator` 会代理子控件的 `IFormItemAware` 接口，确保包装后的控件仍能正常参与表单验证和数据收集。

---

## 15. 内联登录表单

使用 `FormLayout="Inline"` 创建紧凑的导航栏登录表单：

```xml
<atom:Form LabelColInfo="8*"
           WrapperColInfo="16*"
           MinWidth="600"
           HorizontalAlignment="Left"
           FormLayout="Inline">
    <atom:FormItem LabelText="Username"
                   FieldName="username">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please input your username!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit InnerLeftContent="{antdicons:AntDesignIconProvider UserOutlined}"
                       PlaceholderText="Username"
                       Width="200"/>
    </atom:FormItem>
    <atom:FormItem LabelText="Password"
                   FieldName="password">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please input your password!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit InnerLeftContent="{antdicons:AntDesignIconProvider LockOutlined}"
                       PlaceholderText="Password"
                       Width="200"/>
    </atom:FormItem>
    <atom:FormActionsItem>
        <atom:SubmitButton IsWatchValidateResult="True" Content="Log in"/>
    </atom:FormActionsItem>
</atom:Form>
```

---

## 16. 隐藏标签登录表单

使用 `IsHideItemLabel="True"` 隐藏所有 FormItem 的标签区域，常用于独立的登录页面：

```xml
<atom:Form LabelColInfo="8*"
           WrapperColInfo="16*"
           MinWidth="400"
           HorizontalAlignment="Left"
           IsHideItemLabel="True">
    <atom:FormItem FieldName="username">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please input your username!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit InnerLeftContent="{antdicons:AntDesignIconProvider UserOutlined}"
                       PlaceholderText="Username"/>
    </atom:FormItem>
    <atom:FormItem FieldName="password">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please input your password!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit InnerLeftContent="{antdicons:AntDesignIconProvider LockOutlined}"
                       PlaceholderText="Password"/>
    </atom:FormItem>
    <atom:FormItem FieldName="remember">
        <atom:CheckBox IsChecked="True">Remember me</atom:CheckBox>
        <atom:FormItem.Extra>
            <atom:HyperLinkButton>Forgot password</atom:HyperLinkButton>
        </atom:FormItem.Extra>
    </atom:FormItem>
    <atom:FormActionsItem>
        <StackPanel>
            <atom:SubmitButton Content="Log in" HorizontalAlignment="Stretch"/>
            <StackPanel Orientation="Horizontal" Spacing="5">
                <TextBlock>Or</TextBlock>
                <atom:HyperLinkTextBlock>Register now!</atom:HyperLinkTextBlock>
            </StackPanel>
        </StackPanel>
    </atom:FormActionsItem>
</atom:Form>
```

---

## 17. 注册表单（综合示例）

一个包含多种控件类型的完整注册表单：

```xml
<atom:Form LabelColInfo="8*"
           WrapperColInfo="16*"
           MinWidth="600"
           HorizontalAlignment="Left">
    <atom:FormItem LabelText="Email"
                   FieldName="email"
                   IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please input your E-mail!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit />
    </atom:FormItem>
    <atom:FormItem LabelText="Password"
                   FieldName="password"
                   IsRequired="True"
                   IsValidateFeedbackEnabled="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please input your password!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit RevealPassword="False"
                       PasswordChar="•"
                       IsEnableRevealButton="True" />
    </atom:FormItem>
    <atom:FormItem LabelText="Nickname"
                   FieldName="nickname"
                   IsRequired="True"
                   Tooltip="What do you want others to call you?">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please input your nickname!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:LineEdit />
    </atom:FormItem>
    <atom:FormItem LabelText="Habitual Residence"
                   FieldName="residence"
                   IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormNotNullValidator Message="Please select your habitual residence!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:Cascader HorizontalAlignment="Stretch" IsAllowClear="True">
            <atom:CascaderOption Header="Zhejiang" Value="zhejiang">
                <atom:CascaderOption Header="Hangzhou" Value="hangzhou">
                    <atom:CascaderOption Header="West Lake" Value="xihu" />
                </atom:CascaderOption>
            </atom:CascaderOption>
        </atom:Cascader>
    </atom:FormItem>
    <atom:FormItem LabelText="Gender"
                   FieldName="gender"
                   IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormNotNullValidator Message="Please select gender!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:Select DefaultValues="male" HorizontalAlignment="Stretch">
            <atom:SelectOption Header="Male" Content="male"/>
            <atom:SelectOption Header="Female" Content="female"/>
            <atom:SelectOption Header="Other" Content="other"/>
        </atom:Select>
    </atom:FormItem>
    <atom:FormItem LabelText="Intro"
                   FieldName="intro"
                   IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormStringNotEmptyValidator Message="Please input Intro!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:TextArea IsShowCount="True" MaxLength="100"/>
    </atom:FormItem>
    <atom:FormItem FieldName="agreement" IsRequired="True">
        <atom:CheckBox IsChecked="True">
           <StackPanel Orientation="Horizontal" Spacing="5">
               <TextBlock>I have read the</TextBlock>
               <atom:HyperLinkTextBlock>agreement</atom:HyperLinkTextBlock>
           </StackPanel>
        </atom:CheckBox>
    </atom:FormItem>
    <atom:FormActionsItem>
        <atom:SubmitButton Content="Register"/>
    </atom:FormActionsItem>
</atom:Form>
```

---

## 18. 时间类控件

Form 支持所有 AtomUI 的时间选择控件：

```xml
<atom:Form LabelColInfo="200"
           WrapperColInfo="16*"
           MinWidth="600"
           HorizontalAlignment="Left">
    <atom:FormItem LabelText="DatePicker" FieldName="datePicker" IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormNotNullValidator Message="Please select time!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:DatePicker PlaceholderText="Select date!"/>
    </atom:FormItem>

    <atom:FormItem LabelText="DatePicker[showTime]" FieldName="datePickerShowTime" IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormNotNullValidator Message="Please select time!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:DatePicker PlaceholderText="Select date!" IsShowTime="True"/>
    </atom:FormItem>

    <atom:FormItem LabelText="RangePicker" FieldName="rangePicker" IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormNotNullValidator Message="Please select time!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:RangeDatePicker PlaceholderText="Start date"
                              SecondaryPlaceholderText="End date"/>
    </atom:FormItem>

    <atom:FormItem LabelText="TimePicker" FieldName="timePicker" IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormNotNullValidator Message="Please select time!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:TimePicker PlaceholderText="Start time"/>
    </atom:FormItem>

    <atom:FormActionsItem>
        <atom:SubmitButton/>
    </atom:FormActionsItem>
</atom:Form>
```

---

## 19. 自定义验证状态展示

可以使用 `FormAssertValidator` 来强制设置验证状态，配合 `IsValidateFeedbackEnabled` 显示反馈图标：

```xml
<atom:FormItem LabelText="Success"
               FieldName="success"
               IsValidateFeedbackEnabled="True">
    <atom:FormItem.Validators>
        <atom:FormValidatorProvider>
            <atom:FormAssertValidator AssertResult="True"/>
        </atom:FormValidatorProvider>
    </atom:FormItem.Validators>
    <atom:LineEdit PlaceholderText="I'm the content"/>
</atom:FormItem>

<atom:FormItem LabelText="Warning"
               FieldName="warning"
               IsValidateFeedbackEnabled="True">
    <atom:FormItem.Validators>
        <atom:FormValidatorProvider>
            <atom:FormAssertValidator AssertResult="False" WarningOnly="True"/>
        </atom:FormValidatorProvider>
    </atom:FormItem.Validators>
    <atom:LineEdit PlaceholderText="I'm the content"/>
</atom:FormItem>

<atom:FormItem LabelText="Fail"
               FieldName="fail"
               IsValidateFeedbackEnabled="True"
               Help="Should be combination of numbers and alphabets">
    <atom:FormItem.Validators>
        <atom:FormValidatorProvider>
            <atom:FormAssertValidator AssertResult="False"/>
        </atom:FormValidatorProvider>
    </atom:FormItem.Validators>
    <atom:LineEdit PlaceholderText="unavailable choice"/>
</atom:FormItem>

<atom:FormItem LabelText="Validating"
               FieldName="validating"
               Help="The information is being validated"
               IsValidateFeedbackEnabled="True">
    <atom:FormItem.Validators>
        <atom:FormValidatorProvider>
            <atom:FormAssertValidator AssertResult="True" Delay="00:01:00"/>
        </atom:FormValidatorProvider>
    </atom:FormItem.Validators>
    <atom:LineEdit PlaceholderText="I'm the content is being validated"/>
</atom:FormItem>
```

**说明**：
- `IsValidateFeedbackEnabled="True"` 在输入控件内显示验证状态图标（✓ / ⚠ / ✗ / ⏳）
- `Help` 属性显示固定帮助文本，颜色随验证状态变化
- `FormAssertValidator` 的 `Delay` 属性可模拟异步验证延迟

---

## 20. 其他表单控件

Form 支持几乎所有 AtomUI 数据录入和交互控件：

```xml
<atom:Form LabelColInfo="200"
           WrapperColInfo="16*"
           MinWidth="600"
           HorizontalAlignment="Left">
    <!-- 纯文本 -->
    <atom:FormItem LabelText="Plain Text" FieldName="plainText">
        <atom:TextBlock Text="China"/>
    </atom:FormItem>
    
    <!-- 单选 / 多选 Select -->
    <atom:FormItem LabelText="Select" FieldName="select" IsValidateFeedbackEnabled="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormNotNullValidator Message="Please select your country!" />
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:Select PlaceholderText="Please select a country" HorizontalAlignment="Stretch">
            <atom:SelectOption Header="China" Content="china"/>
            <atom:SelectOption Header="U.S.A" Content="usa"/>
        </atom:Select>
    </atom:FormItem>
    
    <!-- FormItemDecorator + NumericUpDown -->
    <atom:FormItem LabelText="InputNumber" FieldName="inputNumber">
       <atom:FormItemDecorator>
           <atom:NumericUpDown Minimum="1" Maximum="10" Width="120"/>
           <atom:FormItemDecorator.Extra>
               machines
           </atom:FormItemDecorator.Extra>
       </atom:FormItemDecorator>
    </atom:FormItem>
    
    <!-- 开关 -->
    <atom:FormItem LabelText="Switch" FieldName="switch">
        <atom:ToggleSwitch/>
    </atom:FormItem>
    
    <!-- 滑块 -->
    <atom:FormItem LabelText="Slider" FieldName="slider">
        <atom:Slider Minimum="0" Maximum="100"/>
    </atom:FormItem>
    
    <!-- 单选按钮组 -->
    <atom:FormItem LabelText="radio-group" FieldName="radioGroup">
        <atom:RadioButtonGroup>
            <atom:RadioButton Tag="a">item 1</atom:RadioButton>
            <atom:RadioButton Tag="b">item 2</atom:RadioButton>
            <atom:RadioButton Tag="c">item 3</atom:RadioButton>
        </atom:RadioButtonGroup>
    </atom:FormItem>
    
    <!-- 复选框组 -->
    <atom:FormItem LabelText="Checkbox.Group" FieldName="checkboxGroup">
        <atom:CheckBoxGroup Width="150" HorizontalAlignment="Left">
            <atom:CheckBox Tag="a">A</atom:CheckBox>
            <atom:CheckBox Tag="b" IsChecked="True" IsEnabled="False">B</atom:CheckBox>
            <atom:CheckBox Tag="c">C</atom:CheckBox>
        </atom:CheckBoxGroup>
    </atom:FormItem>
    
    <!-- 评分 -->
    <atom:FormItem LabelText="Rate" FieldName="rate">
        <atom:Rate/>
    </atom:FormItem>
    
    <!-- 颜色选择器 -->
    <atom:FormItem LabelText="ColorPicker" FieldName="colorPicker" IsRequired="True">
        <atom:FormItem.Validators>
            <atom:FormValidatorProvider>
                <atom:FormNotNullValidator Message="color is required!"/>
            </atom:FormValidatorProvider>
        </atom:FormItem.Validators>
        <atom:ColorPicker/>
    </atom:FormItem>

    <!-- 上传 -->
    <atom:FormItem LabelText="Upload" FieldName="upload">
        <atom:Upload>
            <atom:Button Icon="{antdicons:AntDesignIconProvider UploadOutlined}">
                Click to Upload
            </atom:Button>
        </atom:Upload>
    </atom:FormItem>
    
    <!-- 拖拽上传 -->
    <atom:FormItem LabelText="Dragger" FieldName="dragger">
        <atom:Upload>
            <atom:UploadDefaultDropArea/>
        </atom:Upload>
    </atom:FormItem>
    
    <atom:FormActionsItem>
        <atom:SubmitButton/>
    </atom:FormActionsItem>
</atom:Form>
```

> 📖 完整示例参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataEntry/FormShowCase.axaml` 中 "Other Form Controls" 示例。

---

## 常见组合模式

### 基本提交/重置

```xml
<atom:FormActionsItem>
    <StackPanel Orientation="Horizontal" Spacing="10">
        <atom:SubmitButton />
        <atom:ResetButton />
    </StackPanel>
</atom:FormActionsItem>
```

### 带验证监听的提交按钮

```xml
<atom:FormActionsItem>
    <atom:SubmitButton IsWatchValidateResult="True" Content="Log in"/>
</atom:FormActionsItem>
```

### 动态添加按钮

```xml
<atom:FormActionsItem>
    <atom:Button ButtonType="Dashed"
                 HorizontalAlignment="Stretch"
                 Icon="{antdicons:AntDesignIconProvider PlusOutlined}"
                 Click="HandleAddFormItem">
        Add field
    </atom:Button>
</atom:FormActionsItem>
```

### C# 代码操作表单

```csharp
// 获取 Form 引用
var form = this.FindControl<Form>("MyForm")!;

// 手动触发验证
form.Validate();

// 异步验证并获取结果
var result = await form.ValidateAsync(CancellationToken.None);

// 提交表单
form.Submit();

// 重置表单
form.Reset();

// 批量设置值
form.SetFormValues(new FormValues(new Dictionary<string, object?>
{
    ["username"] = "admin",
    ["password"] = "123456"
}));

// 监听提交事件
form.Submitted += (sender, args) =>
{
    var values = args.Values; // IFormValues
    // 处理提交的数据...
};
```

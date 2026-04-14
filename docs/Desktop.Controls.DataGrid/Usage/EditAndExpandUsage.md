# DataGrid 编辑与展开行用法

## 单元格编辑

```xml
<atom:DataGrid ItemsSource="{Binding Users}" IsReadOnly="False">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" />
        <atom:DataGridNumericColumn Header="年龄" Binding="{Binding Age}" FormatString="0" />
        <atom:DataGridCheckBoxColumn Header="启用" Binding="{Binding IsEnabled}" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

双击单元格进入编辑模式。

---

## 模板列编辑

```xml
<atom:DataGridTemplateColumn Header="状态">
    <atom:DataGridTemplateColumn.CellTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Status}" />
        </DataTemplate>
    </atom:DataGridTemplateColumn.CellTemplate>
    <atom:DataGridTemplateColumn.CellEditingTemplate>
        <DataTemplate>
            <atom:Select ItemsSource="{Binding StatusOptions}" SelectedItem="{Binding Status}" />
        </DataTemplate>
    </atom:DataGridTemplateColumn.CellEditingTemplate>
</atom:DataGridTemplateColumn>
```

---

## 监听编辑事件

```csharp
dataGrid.BeginningEdit += (s, e) => {
    // 可取消编辑
    e.Cancel = false;
};

dataGrid.CellEditEnded += (s, e) => {
    // 编辑结束处理
};
```

---

## 可展开行

```xml
<atom:DataGrid ItemsSource="{Binding Users}" RowDetailsVisibilityMode="Collapsed">
    <atom:DataGrid.Columns>
        <atom:DataGridDetailExpanderColumn />
        <atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" />
        <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" />
    </atom:DataGrid.Columns>
    <atom:DataGrid.RowDetailsTemplate>
        <DataTemplate>
            <StackPanel Margin="16,8" Spacing="4">
                <TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
                <TextBlock Text="{Binding Email}" Foreground="Gray" />
            </StackPanel>
        </DataTemplate>
    </atom:DataGrid.RowDetailsTemplate>
</atom:DataGrid>
```

---

## 选中时展开

```xml
<atom:DataGrid RowDetailsVisibilityMode="VisibleWhenSelected">
    <!-- 选中行时自动展开详情 -->
</atom:DataGrid>
```

---

## 编程式控制展开

```csharp
dataGrid.LoadingRow += (s, e) => {
    if (e.Row.DataContext is UserItem user && user.ShouldExpand)
    {
        e.Row.IsDetailsVisible = true;
    }
};
```

> 📖 Gallery 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml` - Editable Cells / Expandable Row
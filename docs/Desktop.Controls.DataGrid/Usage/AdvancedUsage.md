# DataGrid 高级用法

## 分组表头

```xml
<atom:DataGrid>
    <atom:DataGrid.ColumnGroups>
        <atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" />
        <atom:DataGridColumnGroupItem Header="其他信息">
            <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" />
            <atom:DataGridColumnGroupItem Header="地址">
                <atom:DataGridTextColumn Header="街道" Binding="{Binding Street}" />
                <atom:DataGridTextColumn Header="楼号" Binding="{Binding Building}" />
            </atom:DataGridColumnGroupItem>
        </atom:DataGridColumnGroupItem>
    </atom:DataGrid.ColumnGroups>
</atom:DataGrid>
```

---

## 自定义列模板

```xml
<atom:DataGridTemplateColumn Header="操作">
    <atom:DataGridTemplateColumn.CellTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal" Spacing="8">
                <atom:Button Text="编辑" SizeType="Small" Command="{Binding EditCommand}" />
                <atom:Button Text="删除" SizeType="Small" ButtonType="Primary" Danger="True"
                             Command="{Binding DeleteCommand}" />
            </StackPanel>
        </DataTemplate>
    </atom:DataGridTemplateColumn.CellTemplate>
</atom:DataGridTemplateColumn>
```

---

## 自定义列头模板

```xml
<atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}">
    <atom:DataGridTextColumn.HeaderTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal" Spacing="4">
                <atom:Icon Icon="{x:Static atom:AntDesignIconPackage.UserOutlined}" />
                <TextBlock Text="姓名" />
            </StackPanel>
        </DataTemplate>
    </atom:DataGridTextColumn.HeaderTemplate>
</atom:DataGridTextColumn>
```

---

## 列宽控制

```xml
<!-- 固定宽度 -->
<atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" Width="120" />

<!-- 自动宽度 -->
<atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" Width="Auto" />

<!-- 按比例分配 -->
<atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" Width="2*" />
<atom:DataGridTextColumn Header="地址" Binding="{Binding Address}" Width="3*" />

<!-- 最小/最大宽度 -->
<atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}"
    MinWidth="80" MaxWidth="200" />

<!-- 禁止调整列宽 -->
<atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" CanUserResize="False" />
```

---

## 行加载事件

```csharp
dataGrid.LoadingRow += (s, e) => {
    var row = e.Row;
    // 根据数据设置行样式
    if (row.DataContext is DataItem item)
    {
        if (item.IsDisabled)
            row.Opacity = 0.5;
    }
};

dataGrid.UnloadingRow += (s, e) => {
    // 行被回收时清理资源
};
```

---

## 列显示/隐藏

```xml
<atom:DataGridTextColumn Header="内部ID" Binding="{Binding Id}" IsVisible="False" />
```

编程式控制：

```csharp
column.IsVisible = false;  // 隐藏列
column.IsVisible = true;   // 显示列
```

---

## 数据分组

```xml
<atom:DataGrid ItemsSource="{Binding GroupedUsers}" IsGroupingEnabled="True">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" />
        <atom:DataGridTextColumn Header="部门" Binding="{Binding Department}" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

---

## 完整示例：订单表格

```xml
<atom:DataGrid ItemsSource="{Binding Orders}"
               AutoGenerateColumns="False"
               SizeType="Middle"
               SelectionMode="Extended"
               CanUserSortColumns="True"
               CanUserReorderColumns="True"
               LeftFrozenColumnCount="1"
               RightFrozenColumnCount="1"
               PageSize="20"
               PaginationVisibility="Bottom"
               GridLinesVisibility="Horizontal"
               IsReadOnly="True"
               RowDetailsVisibilityMode="Collapsed">

    <!-- 表头 -->
    <atom:DataGrid.HeaderTemplate>
        <DataTemplate>
            <TextBlock Text="订单管理" FontWeight="Bold" FontSize="16" />
        </DataTemplate>
    </atom:DataGrid.HeaderTemplate>

    <atom:DataGrid.Columns>
        <atom:DataGridSelectionColumn />
        <atom:DataGridTextColumn Header="订单号" Binding="{Binding OrderNo}" CanUserSort="True" />
        <atom:DataGridTextColumn Header="客户" Binding="{Binding Customer}" CanUserSort="True" CanUserFilter="True" />
        <atom:DataGridNumericColumn Header="金额" Binding="{Binding Amount}" FormatString="C2" CanUserSort="True" />
        <atom:DataGridTextColumn Header="状态" Binding="{Binding Status}" CanUserFilter="True" FilterMultiple="True" />
        <atom:DataGridTemplateColumn Header="操作" Width="150">
            <atom:DataGridTemplateColumn.CellTemplate>
                <DataTemplate>
                    <StackPanel Orientation="Horizontal" Spacing="4">
                        <atom:LinkButton Text="查看" Command="{Binding ViewCommand}" />
                        <atom:LinkButton Text="编辑" Command="{Binding EditCommand}" />
                    </StackPanel>
                </DataTemplate>
            </atom:DataGridTemplateColumn.CellTemplate>
        </atom:DataGridTemplateColumn>
    </atom:DataGrid.Columns>

    <!-- 行详情 -->
    <atom:DataGrid.RowDetailsTemplate>
        <DataTemplate>
            <StackPanel Margin="16,8" Spacing="4">
                <TextBlock Text="{Binding Description}" TextWrapping="Wrap" />
            </StackPanel>
        </DataTemplate>
    </atom:DataGrid.RowDetailsTemplate>

    <!-- 表尾 -->
    <atom:DataGrid.FooterTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding TotalText}" />
        </DataTemplate>
    </atom:DataGrid.FooterTemplate>
</atom:DataGrid>
```

> 📖 Gallery 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml` - Grouping table head / Order Specific Column
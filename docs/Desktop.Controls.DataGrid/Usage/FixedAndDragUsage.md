# DataGrid 固定列与拖拽用法

## 左侧固定列

```xml
<atom:DataGrid ItemsSource="{Binding Users}" LeftFrozenColumnCount="2">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" />
        <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" />
        <atom:DataGridTextColumn Header="地址" Binding="{Binding Address}" />
        <atom:DataGridTextColumn Header="电话" Binding="{Binding Phone}" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

---

## 右侧固定列

```xml
<atom:DataGrid ItemsSource="{Binding Users}" RightFrozenColumnCount="1">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" />
        <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" />
        <atom:DataGridTemplateColumn Header="操作">
            <!-- 操作按钮列固定在右侧 -->
        </atom:DataGridTemplateColumn>
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

---

## 固定表头

```xml
<atom:DataGrid ItemsSource="{Binding Users}" Height="400">
    <!-- 数据超出高度时表头自动固定 -->
</atom:DataGrid>
```

---

## 列拖拽排序

```xml
<atom:DataGrid ItemsSource="{Binding Users}" CanUserReorderColumns="True">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" CanUserReorder="True" />
        <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" CanUserReorder="True" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

---

## 行拖拽排序

```xml
<atom:DataGrid ItemsSource="{Binding Users}" CanUserReorderRows="True">
    <atom:DataGrid.Columns>
        <atom:DataGridRowReorderColumn />
        <atom:DataGridTextColumn Header="姓名" Binding="{Binding Name}" />
        <atom:DataGridTextColumn Header="年龄" Binding="{Binding Age}" />
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

---

## 固定列+拖拽组合

```xml
<atom:DataGrid ItemsSource="{Binding Users}" Height="400"
               LeftFrozenColumnCount="2" RightFrozenColumnCount="1"
               CanUserReorderColumns="True">
    <!-- 固定列区域内列只能在固定区域内重排 -->
</atom:DataGrid>
```

> 📖 Gallery 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml` - Fixed Header / Fixed Columns / Drag Column sorting / Drag sorting with handler
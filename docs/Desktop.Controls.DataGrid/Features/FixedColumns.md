# DataGrid 固定列与固定表头

## 概述

DataGrid 支持左右固定列和固定表头，在数据滚动时保持关键列和表头始终可见。固定列区域会显示阴影效果以区分固定区域和滚动区域。

---

## 固定列

### 左侧固定列

通过 `LeftFrozenColumnCount` 设置左侧固定列数：

```xml
<atom:DataGrid LeftFrozenColumnCount="2">
    <atom:DataGrid.Columns>
        <atom:DataGridTextColumn Header="Name" Binding="{Binding Name}" />     <!-- 固定 -->
        <atom:DataGridTextColumn Header="Age" Binding="{Binding Age}" />       <!-- 固定 -->
        <atom:DataGridTextColumn Header="Address" Binding="{Binding Address}" />
        <!-- ... 更多可滚动列 ... -->
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

### 右侧固定列

通过 `RightFrozenColumnCount` 设置右侧固定列数：

```xml
<atom:DataGrid RightFrozenColumnCount="1">
    <atom:DataGrid.Columns>
        <!-- ... 可滚动列 ... -->
        <atom:DataGridTemplateColumn Header="Action">  <!-- 右固定 -->
            <!-- ... -->
        </atom:DataGridTemplateColumn>
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

### 左右同时固定

```xml
<atom:DataGrid LeftFrozenColumnCount="2" RightFrozenColumnCount="1">
    <!-- ... -->
</atom:DataGrid>
```

---

## 固定表头

设置 DataGrid 的 `Height` 属性，当数据行超出高度时表头自动固定：

```xml
<atom:DataGrid Height="400">
    <atom:DataGrid.Columns>
        <!-- ... -->
    </atom:DataGrid.Columns>
</atom:DataGrid>
```

---

## 固定列+固定表头

可同时使用：

```xml
<atom:DataGrid LeftFrozenColumnCount="2" RightFrozenColumnCount="1"
               HeadersVisibility="All" Height="400">
    <!-- ... -->
</atom:DataGrid>
```

---

## 固定列阴影

固定列与滚动区域之间显示阴影效果，由以下 Token 控制：

| Token | 说明 |
|-------|------|
| `LeftFrozenShadows` | 左侧固定列阴影（OffsetX=-10, Blur=8） |
| `RightFrozenShadows` | 右侧固定列阴影（OffsetX=10, Blur=8） |

阴影颜色使用 `SharedToken.ColorSplit`。

---

## 固定列与拖拽排序配合

列拖拽排序可与固定列配合使用，固定列区域内的列只能在固定区域内重排：

```xml
<atom:DataGrid CanUserReorderColumns="True"
               LeftFrozenColumnCount="2" RightFrozenColumnCount="2">
    <!-- ... -->
</atom:DataGrid>
```

---

## 列的固定状态查询

| 属性 | 说明 |
|------|------|
| `IsFrozen` | 是否为固定列 |
| `IsLeftFrozen` | 是否为左侧固定列 |
| `IsRightFrozen` | 是否为右侧固定列 |

> 📖 Gallery 参考：`controlgallery/AtomUIGallery/ShowCases/Views/DataDisplay/DataGridShowCase.axaml` - Fixed Header / Fixed Columns / Fixed Columns And Headers
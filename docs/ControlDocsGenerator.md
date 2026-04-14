请基于以下输入材料，为 AtomUI 的 XX 控件生成完整的设计文档，并按控件类型分别归档至指定目录下。

### 一、需要分析的输入材料
1. **AtomUI 已有原理文档**：`docs/` 目录下关于 AtomUI 基本架构的现有文档。
2. **Avalonia 控件参考**：
    - 文档：`.referenceprojects/avalonia-docs/controls`
    - 源码：`.referenceprojects/Avalonia/src/Avalonia.Controls`
3. **Ant Design 控件规范与示例**：`.referenceprojects/ant-design/components` 目录下的所有组件演示文档。
4. **AtomUI 控件源码实现**：
    - `src/AtomUI.Controls`
    - `src/AtomUI.Desktop.Controls`
5. **AtomUI 内置控件使用范例**：`controlgallery/AtomUIGallery/ShowCases` 目录下的所有演示代码与 AXAML 片段。
6. 你可以参考已经生成的 docs/Desktop.Controls/Button 范例

### 二、需要为每个控件生成的文档内容
针对每个内置控件，请在同一文件夹下生成以下四部分文档（可根据实际内容拆分为独立文件或合并于一个文件中，但需保持逻辑清晰）：

1. **控件设计文档**
    - 说明该控件在 AtomUI 中的设计意图、功能范围以及与 Ant Design 规范的对齐程度。
    - 解释其与 Avalonia 基础控件的继承/组合关系。

2. **API 列举**
    - 以表格或结构化列表形式，列出该控件公开的属性、方法、事件及附加属性。
    - 包含类型、默认值及简要说明。

3. **自定义样式指南**
    - 说明如何通过 `Style` 或 `ControlTheme` 覆盖该控件的视觉表现。
    - 给出具体的 AXAML 样式定义示例（优先引用 Gallery 中已存在的示例）。

4. **支持的 Design Token**
    - 列出该控件响应或消费的主题资源键（如 `ButtonBackground`、`ButtonForeground` 等）。
    - 说明这些 Token 对控件外观的具体影响。

### 三、输出路径要求
- 请按控件名称创建子目录，路径格式为：`docs/Desktop.Controls/<控件名>/`（例如 `docs/Desktop.Controls/Button/`）。
- 若目标文件夹不存在，请先创建后再写入文档。
- 为保持一致性，请对 `src/AtomUI.Controls` 和 `src/AtomUI.Desktop.Controls` 中出现的所有公开控件均生成相应文档。

### 四、补充说明
- 文档语言建议使用**中文**，代码示例及 API 签名可保留英文。
- 在文档中引用 Gallery 中的具体示例路径（如 `controlgallery/AtomUIGallery/ShowCases/Views/ButtonDemo.axaml`）以增强可参考性。
- 因为我们控件是可能分为两部分，一部分是设备无关的定义，放在 AtomUI.Controls 中，一部分是设备相关的，比如桌面，放在 AtomUI.Desktop.Controls 中，如果当前控件有设备无关的控件基类，也需要把基类的信息都列举出来，格式你根据需要自行确定
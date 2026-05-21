# TreeView Regression Matrix

## Functional Matrix

- [x] Default switcher mode: collapsed node shows expand icon, expanded node shows collapse icon.
- [x] Rotation switcher mode: node uses rotation icon and checked state applies rotation transform.
- [x] Loading mode: switcher uses loading icon.
- [x] Leaf mode: leaf icon is visible when `IsShowLeafIcon=true`.
- [x] Leaf hidden mode: switcher presenter is hidden when leaf icon visibility is disabled.
- [ ] CheckBox toggle type: checkbox indicator stays visible and switcher keeps its own icon state.
- [ ] Radio toggle type: radio indicator is shown only for leaf radio items.
- [x] Custom switcher icons: `SwitcherExpandIcon`, `SwitcherCollapseIcon`, `SwitcherRotationIcon`, `SwitcherLoadingIcon`, and `SwitcherLeafIcon` continue to flow into the active presenter.

## Multi-Step Gallery Flows

- [x] `TreeViewShowCase`: first navigation settles with stable visual/logical counts.
- [ ] Basic tree: expand and collapse parent nodes.
- [ ] Custom collapse/expand tree: expand and collapse nodes with rotation icon.
- [ ] Async load tree: request node load and show loading switcher icon.
- [x] Searchable examples: page loads with filter controls and tree headers intact.
- [x] Drag tree: page loads with draggable tree headers and switcher icons intact.
- [x] Context menu tree: page loads with right-click menu resources and switcher icons intact.

## Lifecycle Matrix

- [x] Mount `TreeViewShowCase` -> navigate away -> re-mount via repeated Gallery benchmark.
- [x] Property toggle on/off/on: `IsChecked` switches current icon between expand/collapse.
- [x] Property toggle on/off/on: `IconMode` switches current icon among default/rotation/loading/leaf.
- [x] Property toggle on/off/on: `IsLeafIconVisible` hides and restores leaf presenter visibility.

## Notes

Checked items were verified by `--verify-treeview-states` and the Gallery navigation benchmark. Unchecked interaction flows require manual Gallery walk-through when a future change touches TreeView input, drag/drop, async loading, selection, or context-menu event flow.

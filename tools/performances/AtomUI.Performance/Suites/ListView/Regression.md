# ListView Regression Matrix

## Functional Matrix

- [ ] ItemsSource wrapping keeps `IListCollectionView.Filter` unset when `Filter` or `FilterValue` is inactive.
- [ ] Active `Filter` + `FilterValue` installs the ListView-owned default filter callback, and clearing the condition removes it.
- [ ] Collection item-count changes update `TotalItemCount` and attached pagination `Total`.
- [ ] `PaginationVisibility` changes only pagination visibility and does not mutate pagination `IsMotionEnabled`.
- [ ] Attached pagination keeps `CurrentPage >= 1`, including non-paged collection views whose `PageIndex` is `-1`.
- [ ] Replacing `Selection` unsubscribes the old selection model from `PropertyChanged`, `SelectionChanged`, and `LostSelection`.
- [ ] Selected indicator remains in the static item template and toggles `IsVisible`.
- [ ] Empty indicator remains in the static control template and toggles `IsVisible`.

## Multi-Step User Flows

- [ ] ListView Gallery: open default list, select an item, clear selection, select another item; selected indicator visibility follows selection.
- [ ] ListView Gallery: switch non-empty data to empty data; empty indicator becomes visible without dynamic template child creation.
- [ ] ListView Gallery: attach bottom pagination, change item count, switch pagination visibility between `None` and `Bottom`; page state and motion state remain stable.

## Lifecycle Matrix

- [ ] Mount ListView with plain `ItemsSource`, replace with a new source, and verify the old owned collection view is detached/disposed.
- [ ] Toggle filtering on/off/on and verify only the ListView-owned default callback is added and removed.
- [ ] Replace top and bottom pagination controls and verify old controls no longer receive page-change or binding updates.
- [ ] Replace selection model and verify events from the old model no longer affect active selection.
